using ModestTree;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StarMapVisualizer : MonoBehaviour {
    [Header("Prefabs")]
    [SerializeField] private StarView _starPrefab;
    [SerializeField] private LineRenderer _linePrefab;

    [Header("Layout")]
    [SerializeField] private Transform _container;
    [SerializeField] private float _layerSpacing = 2.5f;
    [SerializeField] private float _starSpacing = 1.2f;

    [Header("Connections")]
    [SerializeField] private float _lineWidth = 0.08f;
    [SerializeField] private Color _lineColor = new Color(0.5f, 0.5f, 0.5f, 0.4f);

    private readonly Dictionary<LayerCoord, StarView> _views = new();
    private readonly Dictionary<LayersConnection, LineRenderer> _connections = new();

    public StarView CreateStarView(Star star, int layerStarCount) {
        Vector3 position = CalculatePosition(star.Coord, layerStarCount);

        var view = Instantiate(_starPrefab, position, Quaternion.identity, _container);
        view.gameObject.name = $"Star_{star.Coord}";
        view.Initialize(star);

        _views[star.Coord] = view;
        return view;
    }

    public void CreateConnection(LayersConnection layersCon) {
        LayerCoord from = layersCon.from;
        LayerCoord to = layersCon.to;

        if (!_views.TryGetValue(from, out var fromView) ||
            !_views.TryGetValue(to, out var toView))
            return;

        var line = Instantiate(_linePrefab, _container);

        line.positionCount = 2;
        line.useWorldSpace = true;
        line.SetPosition(0, fromView.transform.position);
        line.SetPosition(1, toView.transform.position);

        line.startWidth = line.endWidth = _lineWidth;
        line.startColor = line.endColor = _lineColor;

        _connections.Add(layersCon, line);

        fromView.AddConnectionLine(to, line);
    }

    public bool TryGetView(LayerCoord coord, out StarView view) =>
        _views.TryGetValue(coord, out view);

    public void Clear() {
        foreach (var view in _views.Values) {
            if (view != null) Destroy(view.gameObject);
        }
        foreach (var line in _connections.Values) {
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

    public List<StarView> GetAllViews() {
        return _views.Values.ToList();
    }
}

public struct LayersConnection {
    public LayerCoord to;
    public LayerCoord from;

    public LayersConnection(LayerCoord from, LayerCoord to) : this() {
        this.to = to;
        this.from = from;
    }
}