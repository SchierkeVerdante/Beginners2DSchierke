using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class StarMapController : MonoBehaviour {
    [Header("Dependencies")]
    [SerializeField] private StarMapVisualizer _visualizer;
    [SerializeField] private Spaceship2D _spaceship;
    [SerializeField] private StarInfoPanel _infoPanel;

    [Header("UI")]
    [SerializeField] private Button _generateButton;

    [Header("Generation")]
    [SerializeField] private StarMapGenerationConfig _generationConfig;

    [Inject] private IGameManager _gameManager;

    [Inject] private IStarMapGenerator _generation;
    [Inject] private IStarNavigationService _navigation;
    [Inject] private ILevelProgressService _levelProgress;

    private List<IDisposable> _presenters = new();

    private void Start() {
        InitializePresenters();
        InitializeMap();
        BindUI();
    }

    private void InitializeMap() {
        PlayerProgressData progress = _levelProgress.GetProgress();

        if (progress.IsNewGame) {
            CreateNewMap();
            progress.IsNewGame = false;
            _levelProgress.SetProgress(progress);
        } else if (!_navigation.HasEmptyMap) {
            _navigation.RefreshMap();
        } else {
            CreateNewMap();
        }
    }

    private void CreateNewMap() {
        StarMap starMap = _generation.GenerateNewMap(_generationConfig);
        _navigation.SetNewMap(starMap);

    }

    private void InitializePresenters() {
        _presenters.Add(new NavigationPresenter(_navigation, _infoPanel, _gameManager));
        _presenters.Add(new StarMapPresenter(_spaceship, _navigation, _visualizer));
    }

    private void BindUI() {
        _generateButton?.onClick.AddListener(CreateNewMap);
    }

    
    private void OnDestroy() {
        _generateButton?.onClick.RemoveListener(CreateNewMap);

        foreach (var presenter in _presenters) {
            presenter?.Dispose();
        }
        _presenters.Clear();
    }
}

public class NavigationPresenter : IDisposable {
    private readonly IStarNavigationService _navigation;
    private readonly StarInfoPanel _infoPanel;
    private readonly IGameManager _gameManager;

    public NavigationPresenter(
        IStarNavigationService navigation,
        StarInfoPanel infoPanel,
        IGameManager gameManager) {
        _navigation = navigation;
        _infoPanel = infoPanel;
        _gameManager = gameManager;

        SubscribeToEvents();
    }

    private void SubscribeToEvents() {
        _navigation.OnStarSelected += HandleStarSelected;
        _navigation.OnCurrentStarChanged += HandleCurrentStarChanged;
        _navigation.OnNewMapSet += HandleMapUpdate;
        _infoPanel.OnTravelRequested += HandleTravelRequest;
    }

    private void HandleMapUpdate(StarMap map) {
        _infoPanel.Hide();
    }

    private void HandleStarSelected(Star star) {
        _infoPanel.Show();
        UpdateStarInfo(star);
    }

    private void HandleCurrentStarChanged(Star star) {
        UpdateStarInfo(star);
    }

    private void HandleTravelRequest() {
        var selectedStar = _navigation.SelectedStar;
        if (selectedStar == null) return;

        var interaction = GetInteractionType(selectedStar);

        switch (interaction) {
            case StarInteractionType.Travel:
                _navigation.TryTravelTo(selectedStar);
                break;

            case StarInteractionType.Land:
                _navigation.CurrentStar.IsVisited.Value = true;
                _gameManager.LoadStarExploration(selectedStar);
                break;
        }
    }

    private void UpdateStarInfo(Star star) {
        _infoPanel.SetStarInfo(star);

        var interaction = GetInteractionType(star);
        UpdateInteractionButton(interaction);
    }

    private StarInteractionType GetInteractionType(Star star) {
        if (star == _navigation.CurrentStar && !star.IsVisited.Value) {
            return StarInteractionType.Land;
        }

        if (_navigation.CanTravelTo(star)) {
            return StarInteractionType.Travel;
        }

        return StarInteractionType.Inspect;
    }

    private void UpdateInteractionButton(StarInteractionType type) {
        switch (type) {
            case StarInteractionType.Travel:
                _infoPanel.SetTravelAvailable(true);
                _infoPanel.SetTravelButtonText("Travel");
                break;

            case StarInteractionType.Land:
                _infoPanel.SetTravelAvailable(true);
                _infoPanel.SetTravelButtonText("Land");
                break;

            case StarInteractionType.Inspect:
                _infoPanel.SetTravelAvailable(false);
                _infoPanel.SetTravelButtonText("Can't Travel");
                break;
        }
    }

    public void Dispose() {
        _navigation.OnStarSelected -= HandleStarSelected;
        _navigation.OnCurrentStarChanged -= HandleCurrentStarChanged;
        _navigation.OnNewMapSet -= HandleMapUpdate;
        _infoPanel.OnTravelRequested -= HandleTravelRequest;
    }
}

public class StarMapPresenter : IDisposable {
    private readonly IStarNavigationService _navigation;
    private readonly StarMapVisualizer _visualizer;
    private readonly Spaceship2D _spaceship;

    public StarMapPresenter(Spaceship2D spaceship, IStarNavigationService navigation, StarMapVisualizer visualizer) {
        _navigation = navigation;
        _visualizer = visualizer;
        _spaceship = spaceship;

        _navigation.OnNewMapSet += RebuildMapVisuals;
        _navigation.OnCurrentStarChanged += HandleCurrentStarChanged;

        UpdateSpaceShipInitialPos();
    }

    private void RebuildMapVisuals(StarMap map) {
        CleanCurrentVisuals();
        BuildVisualization(map);
        UpdateSpaceShipInitialPos();
    }

    private void UpdateSpaceShipInitialPos() {
        Star currentStar = _navigation.CurrentStar;
        if (currentStar == null) return;
        if (_visualizer.TryGetView(currentStar.Coord, out var view)) {
            Vector3 gloablPosition = view.transform.position;
            _spaceship.transform.position = gloablPosition;
        }
        MoveSpaceshipToStar(currentStar);
    }

    private void BuildVisualization(StarMap map) {
        if (map == null) return;

        foreach (var layer in map.GetLayers()) {
            var stars = map.GetStarsInLayer(layer);
            foreach (var star in stars) {
                StarView starView = _visualizer.CreateStarView(star, stars.Count);
                starView.OnStarClicked += HandleStarClicked;
            }
        }

        foreach (var star in map.Stars.Values) {
            foreach (var connection in star.GetForwardConnections()) {
                LayersConnection layersConnection = new LayersConnection(star.Coord, connection);
                _visualizer.CreateConnection(layersConnection);
            }
        }
    }

    private void HandleStarClicked(Star star) {
        _navigation.SelectStar(star);
    }

    private void CleanCurrentVisuals() {
        UnsubscribeViews();
        _visualizer.Clear();
    }

    private void HandleCurrentStarChanged(Star star) {
        MoveSpaceshipToStar(star);
    }

    private void MoveSpaceshipToStar(Star star) {
        if (star == null) return;

        if (_spaceship == null || !_visualizer.TryGetView(star.Coord, out var view))
            return;

        _spaceship.SetTarget(view.transform.position, OnSpaceShipReachTarget);
    }

    private void OnSpaceShipReachTarget() {
        _spaceship.StartOrbit();
    }

    private void UnsubscribeViews() {
        var starViews = _visualizer.GetAllViews();
        foreach (var view in starViews) {
            view.OnStarClicked -= HandleStarClicked;
        }
    }

    public void Dispose() {
        _navigation.OnNewMapSet -= RebuildMapVisuals;
        CleanCurrentVisuals();
        _navigation.OnCurrentStarChanged -= HandleCurrentStarChanged;
    }
}

public enum StarInteractionType {
    Travel,
    Land,
    Inspect
}