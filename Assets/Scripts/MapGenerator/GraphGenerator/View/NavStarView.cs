using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Zenject;

public enum NavStarState {
    Locked,
    Available,
    Visited,
    Current,
    Selected
}

public class NavStarView : MonoBehaviour, IView, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler {
    public Action<NavStarView> OnClicked { get; internal set; }

    [SerializeField] TextMeshPro text;
    [Header("Line settings")]
    [SerializeField] Material lineMaterial;
    [SerializeField] float lineWidth = 0.05f;

    [Header("State Weights")]
    [SerializeField] float lockedWeight = 1f;
    [SerializeField] float unlockedWeight = 0.5f;
    [SerializeField] float visitedWeight = 0.3f;
    [SerializeField] float currentWeight = 0.6f;
    [SerializeField] float selectedWeight = 0.4f;
    [SerializeField] float hoveredWeight = 0.3f;

    [SerializeField] SpriteRenderer _renderer;
    [SerializeField] StarVisualTheme _theme;

    [SerializeField] Transform mainBody;

    private NavStarState _currentState = NavStarState.Locked;

    private void Awake() {
        if (_renderer == null)
            _renderer = GetComponentInChildren<SpriteRenderer>();
        if (mainBody == null) {
            if (_renderer != null) {
                mainBody = _renderer.transform;
            } else {
                mainBody = transform;
            }
        }
    }

    public void RenderState(NavStarState state) {
        _currentState = state;
        UpdateVisuals();
    }

    private void UpdateVisuals() {
        _renderer.color = _theme.GetColor(_currentState);
    }

    public void OnPointerDown(PointerEventData eventData) {
        Debug.Log("Clicked!");
        OnClicked?.Invoke(this);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        mainBody.DOScale(1.2f, 0.5f).SetEase(Ease.Flash);
    }

    public void OnPointerExit(PointerEventData eventData) {
        mainBody.DOScale(1.0f, 0.5f).SetEase(Ease.Flash);
    }

    public void DrawConnectionTo(Vector3 targetPosition) {
        var lineObj = new GameObject("ConnectionLine");
        lineObj.transform.SetParent(transform, false);
        var lr = lineObj.AddComponent<LineRenderer>();
        lr.material = lineMaterial;
        lr.widthMultiplier = lineWidth;
        lr.positionCount = 2;
        lr.useWorldSpace = true;
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, targetPosition);
    }

    private void SetBodyColor(Color newColor) {
        if (_renderer != null)
            _renderer.color = newColor;
    }

    public void SetText(string text) {
        if (this.text != null)
            this.text.text = text;
    }

    private void OnDestroy() {
        mainBody.DOKill();
    }
    public void ToggleSelected(bool isEnabled) {
        if (_renderer == null) return;

        if (isEnabled) {
            _renderer.color = _renderer.color * _theme.SelectedColor;
        } else {
            _renderer.color = _theme.GetColor(_currentState); 
        }
    }
}

public class NavStarPresenter {
    public NavStar Model { get; private set; }
    public NavStarView View { get; private set; }
    private StarNavigator _starNavigation;

    public NavStarPresenter(NavStar model, NavStarView view, StarNavigator starNavigation) {
        Model = model;
        View = view;
        _starNavigation = starNavigation;

        View.SetText($"{model.StarCoord}");
        HandleStateChanged(Model.State.Value);
        HandleSelection(Model.IsSelected.Value);

        View.OnClicked += HandleViewClicked;
        Model.State.OnChanged += HandleStateChanged;
        Model.IsSelected.OnChanged += HandleSelection;
    }

    private void HandleSelection(bool isEnabled) {
        View.ToggleSelected(isEnabled);
    }

    private void HandleStateChanged(NavStarState newState) {
        View.RenderState(newState);
    }

    private void HandleViewClicked(NavStarView view) {
        Debug.Log($"Star clicked: {Model}");

        _starNavigation.SelectStar(Model);
    }

    public void Dispose() {
        View.OnClicked -= HandleViewClicked;
        Model.State.OnChanged -= HandleStateChanged;
    }
}

public readonly struct LayerCoord : IEquatable<LayerCoord> {
    public int Layer { get; }
    public int Index { get; }

    public LayerCoord(int layer, int index) {
        Layer = layer;
        Index = index;
    }

    public bool Equals(LayerCoord other) {
        return Layer == other.Layer && Index == other.Index;
    }

    public override bool Equals(object obj) {
        return obj is LayerCoord other && Equals(other);
    }

    public override int GetHashCode() {
        unchecked {
            return (Layer * 397) ^ Index;
        }
    }

    public override string ToString() {
        return $"(L: {Layer}, I :{Index})";
    }
}

public class NavStar : IModel {

    public ReactiveProperty<NavStarState> State { get; }
    public ReactiveProperty<bool> IsSelected { get; }

    public Star Star => _star;

    public LayerCoord StarCoord => _star.StarCoord;
    public IReadOnlyList<LayerCoord> Connections => _star.Connections;

    private Star _star;

    public NavStar(Star star) {
        _star = star;
        State = new ReactiveProperty<NavStarState>(NavStarState.Locked);
        IsSelected = new(false);
    }

    public void SetState(NavStarState newState) {
        State.Value = newState;
    }


    public LayerCoord[] GetNextConnections() {
        return _star.GetNextConnections();
    }

    public bool AreConnectedTo(LayerCoord starCoord) {
        return _star.AreConnectedTo(starCoord);
    }

    public void SetSelected(bool isEnabled) {
        IsSelected.Value = isEnabled;
    }
}

public class Star : IModel {
    public Guid Id { get; protected set; }

    public LayerCoord StarCoord { get; protected set; }

    protected List<LayerCoord> _connections = new();
    public IReadOnlyList<LayerCoord> Connections => _connections.AsReadOnly();

    
    public Star(int layer, int layerIndex) {
        Id = Guid.NewGuid();
        StarCoord = new LayerCoord(layer, layerIndex);
    }

    
    public bool AreConnectedTo(LayerCoord otherRef) {
        return _connections.Contains(otherRef);
    }

    public void AddConnection(LayerCoord otherRef) {
        if (AreConnectedTo(otherRef)) return;
        _connections.Add(otherRef);
    }

    public void AddConnection(int layer, int index) {
        AddConnection(new LayerCoord(layer, index));
    }
    public void RemoveConnection(LayerCoord otherRef) {
        if (!AreConnectedTo(otherRef)) return;
        _connections.Remove(otherRef);
    }

    public void RemoveConnection(int layer, int index) {
        RemoveConnection(new LayerCoord(layer, index));
    }

    public LayerCoord[] GetNextConnections() {
        return _connections.Where(con => con.Layer > StarCoord.Layer).ToArray();
    }

    public void ClearConnections() {
        _connections.Clear();
    }

    public override string ToString() {
        return $"Star {StarCoord}";
    }
}

