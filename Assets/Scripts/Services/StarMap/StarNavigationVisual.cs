using ModestTree;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class StarMapVisualizer : MonoBehaviour {
    [Header("Prefabs")]
    [SerializeField] private StarView _starPrefab;
    [SerializeField] private LineRenderer _linePrefab;

    [Header("Layout")]
    [SerializeField] private Transform _container;
    [SerializeField] private float _layerSpacing = 2.5f;
    [SerializeField] private float _starSpacing = 1.2f;

    [Header("Connections")]
    [SerializeField] private Material _lineMaterial;
    [SerializeField] private float _lineWidth = 0.08f;
    [SerializeField] private Color _lineColor = new Color(0.5f, 0.5f, 0.5f, 0.4f);

    private readonly Dictionary<LayerCoord, StarView> _views = new();
    private readonly List<LineRenderer> _connections = new();

    public StarView CreateStarView(Star star, int layerStarCount, Action<Star> onClick) {
        Vector3 position = CalculatePosition(star.Coord, layerStarCount);

        var view = Instantiate(_starPrefab, position, Quaternion.identity, _container);
        view.gameObject.name = $"Star_{star.Coord}";
        view.Initialize(star, onClick);

        _views[star.Coord] = view;
        return view;
    }

    public void CreateConnection(LayerCoord from, LayerCoord to) {
        if (!_views.TryGetValue(from, out var fromView) ||
            !_views.TryGetValue(to, out var toView))
            return;

        var line = _linePrefab != null
            ? Instantiate(_linePrefab, _container)
            : CreateDefaultLine();

        line.positionCount = 2;
        line.useWorldSpace = true;
        line.SetPosition(0, fromView.transform.position);
        line.SetPosition(1, toView.transform.position);

        if (_lineMaterial != null) line.material = _lineMaterial;
        line.startWidth = line.endWidth = _lineWidth;
        line.startColor = line.endColor = _lineColor;

        _connections.Add(line);
    }

    public bool TryGetView(LayerCoord coord, out StarView view) =>
        _views.TryGetValue(coord, out view);

    public void Clear() {
        foreach (var view in _views.Values) {
            if (view != null) Destroy(view.gameObject);
        }
        foreach (var line in _connections) {
            if (line != null) Destroy(line.gameObject);
        }

        _views.Clear();
        _connections.Clear();
    }

    private Vector3 CalculatePosition(LayerCoord coord, int layerStarCount) {
        float x = coord.Layer * _layerSpacing;
        float y = (coord.Index - (layerStarCount - 1) / 2f) * _starSpacing;
        return new Vector3(x, y, 0);
    }

    private LineRenderer CreateDefaultLine() {
        var obj = new GameObject("Connection");
        obj.transform.SetParent(_container, false);
        return obj.AddComponent<LineRenderer>();
    }
}

public interface IStarNavigationService {
    Star CurrentStar { get; }
    Star SelectedStar { get; }

    void Initialize(LayerCoord startCoord);
    void SelectStar(Star star);
    bool TryTravelTo(Star star);
    bool CanTravelTo(Star star);

    event Action<Star> OnCurrentStarChanged;
    event Action<Star> OnStarSelected;
}

public class StarNavigationService : IStarNavigationService {
    private readonly StarMap _starMap;
    private Star _currentStar;
    private Star _selectedStar;

    public Star CurrentStar => _currentStar;
    public Star SelectedStar => _selectedStar;

    public event Action<Star> OnCurrentStarChanged;
    public event Action<Star> OnStarSelected;

    public StarNavigationService(StarMap starMap) {
        _starMap = starMap ?? throw new ArgumentNullException(nameof(starMap));
    }

    public void Initialize(LayerCoord startCoord) {
        if (_starMap.TryGetStar(startCoord, out var startStar)) {
            SetCurrentStar(startStar);
        }
    }

    public void SelectStar(Star star) {
        if (star == null || star == _selectedStar) return;

        _selectedStar = star;
        OnStarSelected?.Invoke(star);
    }

    public bool TryTravelTo(Star star) {
        if (!CanTravelTo(star)) return false;

        SetCurrentStar(star);
        return true;
    }

    public bool CanTravelTo(Star star) {
        if (star == null || star == _currentStar) return false;

        // Перша зірка завжди доступна
        if (_currentStar == null) return true;

        // Можна подорожувати тільки вперед по з'єднаним зіркам
        return star.Coord.Layer > _currentStar.Coord.Layer &&
               _currentStar.IsConnectedTo(star.Coord);
    }

    private void SetCurrentStar(Star star) {
        // Оновлюємо стан попередньої зірки
        if (_currentStar != null) {
            _currentStar.State.Value = StarState.Visited;
        }

        _currentStar = star;
        _currentStar.State.Value = StarState.Current;

        UpdateAvailableStars();
        OnCurrentStarChanged?.Invoke(_currentStar);
    }

    private void UpdateAvailableStars() {
        // Спочатку блокуємо всі доступні зірки
        foreach (var star in _starMap.Stars.Values) {
            if (star.State.Value == StarState.Available) {
                star.State.Value = StarState.Locked;
            }
        }

        // Потім розблоковуємо наступні з'єднані зірки
        foreach (var coord in _currentStar.GetForwardConnections()) {
            if (_starMap.TryGetStar(coord, out var nextStar)) {
                nextStar.State.Value = StarState.Available;
            }
        }
    }
}

