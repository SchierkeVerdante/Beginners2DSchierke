using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class StarMapController : MonoBehaviour {
    [Header("Dependencies")]
    [SerializeField] private StarMapVisualizer _visualizer;
    [SerializeField] private StarInfoPanel _infoPanel;
    [SerializeField] private Spaceship2D _spaceship;

    [Header("UI")]
    [SerializeField] private Button _generateButton;
    [SerializeField] private Button _travelButton;

    [Header("Generation")]
    [SerializeField] private GraphGenerationConfig _generationConfig;

    [Inject] private IDataRuntimeFactory _dataFactory;

    private StarMap _starMap;
    private IStarNavigationService _navigation;

    private void Start() {
        BindButtons();
        GenerateNewMap();
    }

    private void BindButtons() {
        if (_generateButton != null) {
            _generateButton.onClick.AddListener(GenerateNewMap);
        }
        if (_travelButton != null) {
            _travelButton.onClick.AddListener(HandleTravelClick);
        }
    }

    private void GenerateNewMap() {
        CleanupCurrentMap();

        _starMap = GenerateStarMapFromGraph();
        _navigation = CreateNavigationService();

        BuildVisualization();
        _navigation.Initialize(new LayerCoord(0, 0));

        _infoPanel?.Hide();
    }

    private StarMap GenerateStarMapFromGraph() {
        var generator = _dataFactory.CreateInstance<GraphGenerator>(_generationConfig);
        var graph = generator.GenerateGraph();

        var map = new StarMap(graph.Seed);

        foreach (var layer in graph.Layers) {
            foreach (var node in layer) {
                var star = new Star(node.layer, node.layerIndex);
                map.AddStar(star);

                foreach (var connection in node.GetAllConnections()) {
                    star.ConnectTo(new LayerCoord(connection.layer, connection.layerIndex));
                }
            }
        }

        return map;
    }

    private IStarNavigationService CreateNavigationService() {
        var service = new StarNavigationService(_starMap);
        service.OnCurrentStarChanged += HandleCurrentStarChanged;
        service.OnStarSelected += HandleStarSelected;
        return service;
    }

    private void BuildVisualization() {
        foreach (var layer in _starMap.GetLayers()) {
            var stars = _starMap.GetStarsInLayer(layer);
            foreach (var star in stars) {
                _visualizer.CreateStarView(star, stars.Count, HandleStarClicked);
            }
        }

        foreach (var star in _starMap.Stars.Values) {
            foreach (var connection in star.GetForwardConnections()) {
                _visualizer.CreateConnection(star.Coord, connection);
            }
        }
    }

    private void HandleStarClicked(Star star) {
        _navigation.SelectStar(star);
    }

    private void HandleStarSelected(Star star) {
        UpdateSelectionVisuals(star);
        UpdateInfoPanel(star);
    }

    private void UpdateSelectionVisuals(Star star) {
        if (_navigation.SelectedStar != null && star != _navigation.SelectedStar) {
            if (_visualizer.TryGetView(_navigation.SelectedStar.Coord, out var oldView)) {
                oldView.SetSelected(false);
            }
        }

        // Підсвічуємо нову вибрану зірку
        if (_visualizer.TryGetView(star.Coord, out var newView)) {
            newView.SetSelected(true);
        }
    }

    private void UpdateInfoPanel(Star star) {
        if (_infoPanel == null) return;

        _infoPanel.Show();
        _infoPanel.SetStarInfo(star);
        _infoPanel.SetTravelAvailable(_navigation.CanTravelTo(star));
    }

    private void HandleCurrentStarChanged(Star star) {
        MoveSpaceshipToStar(star);
        _infoPanel?.Hide();
    }

    private void MoveSpaceshipToStar(Star star) {
        if (_spaceship == null) return;

        if (_visualizer.TryGetView(star.Coord, out var view)) {
            _spaceship.SetTarget(view.transform.position, () => _spaceship.StartOrbit());
        }
    }

    private void HandleTravelClick() {
        if (_navigation.SelectedStar != null) {
            _navigation.TryTravelTo(_navigation.SelectedStar);
        }
    }

    private void CleanupCurrentMap() {
        _visualizer?.Clear();

        if (_navigation != null) {
            _navigation.OnCurrentStarChanged -= HandleCurrentStarChanged;
            _navigation.OnStarSelected -= HandleStarSelected;
        }
    }

    private void OnDestroy() {
        if (_generateButton != null) {
            _generateButton.onClick.RemoveListener(GenerateNewMap);
        }
        if (_travelButton != null) {
            _travelButton.onClick.RemoveListener(HandleTravelClick);
        }

        CleanupCurrentMap();
    }
}
