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

    [Inject] private IStarMapGenerator _generation;
    [Inject] private IGameManager _gameManager; 

    [Inject] private IStarNavigationService _navigation;
    private List<IDisposable> _presenters = new();

    private void Start() {
        InitializePresenters();
        UpdateNavigation();
        BindUI();
    }

    private void UpdateNavigation() {
        if (_navigation.HasEmptyMap) {
            StarMap starMap = _generation.GenerateNewMap(_generationConfig);
            _navigation.UpdateMap(starMap);

            LayerCoord beginningCoords = starMap.GetBeginningCoords();
            _navigation.SetCurrentPosition(beginningCoords);
        } else {
            _navigation.RefreshMap();
        }
    }

    private void InitializePresenters() {
        _presenters.Add(new NavigationPresenter(_navigation, _infoPanel, _gameManager));
        _presenters.Add(new StarMapPresenter(_spaceship, _navigation, _visualizer));
        _presenters.Add(new SelectionPresenter(_navigation, _visualizer));
    }

    private void BindUI() {
        _generateButton?.onClick.AddListener(UpdateNavigation);
    }

    
    private void OnDestroy() {
        _generateButton?.onClick.RemoveListener(UpdateNavigation);

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
        _navigation.OnMapUpdated += HandleMapUpdate;
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
        if (star == _navigation.CurrentStar && star.State.Value != StarState.Visited) {
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
        _navigation.OnMapUpdated -= HandleMapUpdate;
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

        _navigation.OnMapUpdated += RebuildMapVisuals;
        _navigation.OnCurrentStarChanged += HandleStarChanged;

        HandleStarChanged(_navigation.CurrentStar);
    }

    private void RebuildMapVisuals(StarMap map) {
        CleanCurrentVisuals();
        BuildVisualization(map);

        Star currentStar = _navigation.CurrentStar;
        if (currentStar == null) return;
        if (_visualizer.TryGetView(currentStar.Coord, out var view)) {
            Vector3 gloablPosition = view.transform.position;
            _spaceship.transform.position = gloablPosition;
        }
        
    }

    private void BuildVisualization(StarMap map) {
        foreach (var layer in map.GetLayers()) {
            var stars = map.GetStarsInLayer(layer);
            foreach (var star in stars) {
                StarView starView = _visualizer.CreateStarView(star, stars.Count);
                starView.OnStarClicked += HandleStarClicked;
            }
        }

        foreach (var star in map.Stars.Values) {
            foreach (var connection in star.GetForwardConnections()) {
                _visualizer.CreateConnection(star.Coord, connection);
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

    private void HandleStarChanged(Star star) {
        MoveSpaceshipToStar(star);
    }

    private void MoveSpaceshipToStar(Star star) {
        if (star == null) return;

        if (_spaceship == null || !_visualizer.TryGetView(star.Coord, out var view))
            return;

        _spaceship.SetTarget(view.transform.position, () => _spaceship.StartOrbit());
    }

    private void UnsubscribeViews() {
        var starViews = _visualizer.GetAllViews();
        foreach (var view in starViews) {
            view.OnStarClicked -= HandleStarClicked;
        }
    }

    public void Dispose() {
        _navigation.OnMapUpdated -= RebuildMapVisuals;
        CleanCurrentVisuals();
        _navigation.OnCurrentStarChanged -= HandleStarChanged;
    }
}

public class SelectionPresenter : IDisposable {
    private readonly IStarNavigationService _navigation;
    private readonly StarMapVisualizer _visualizer;

    public SelectionPresenter(IStarNavigationService navigation, StarMapVisualizer visualizer) {
        _navigation = navigation;
        _visualizer = visualizer;

        _navigation.OnStarSelected += HandleStarSelected;
        HandleStarSelected(_navigation.SelectedStar);
    }

    private void HandleStarSelected(Star star) {
        UpdateSelectionVisuals(star);
    }

    private void UpdateSelectionVisuals(Star star) {
        if (star == null) return;

        if (_navigation.SelectedStar != null && star != _navigation.SelectedStar) {
            if (_visualizer.TryGetView(_navigation.SelectedStar.Coord, out var oldView)) {
                oldView.SetSelected(false);
            }
        }

        if (_visualizer.TryGetView(star.Coord, out var newView)) {
            newView.SetSelected(true);
        }
    }

    public void Dispose() {
        _navigation.OnStarSelected -= HandleStarSelected;
    }
}

public enum StarInteractionType {
    Travel,
    Land,
    Inspect
}