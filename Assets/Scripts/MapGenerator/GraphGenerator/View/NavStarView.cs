using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

[Flags]
public enum StarVisualState {
    None = 0,
    Locked = 1 << 0,      // 1
    Unlocked = 1 << 1,    // 2
    Visited = 1 << 2,     // 4
    Current = 1 << 3,     // 8
    Selected = 1 << 4,    // 16
    Hovered = 1 << 5      // 32
}

public class NavStarView : MonoBehaviour, IView, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler {
    public Action<NavStarView> OnClicked { get; internal set; }

    [SerializeField] TextMeshPro text;
    [Header("Line settings")]
    [SerializeField] Material lineMaterial;
    [SerializeField] float lineWidth = 0.05f;

    [Header("State Colors")]
    [SerializeField] Color baseColor = Color.white;
    [SerializeField] Color lockedTint = new Color(0.5f, 0.5f, 0.5f, 1f);
    [SerializeField] Color unlockedTint = Color.white;
    [SerializeField] Color visitedTint = new Color(0.7f, 0.9f, 1f, 1f);
    [SerializeField] Color currentTint = new Color(1f, 1f, 0f, 1f);
    [SerializeField] Color selectedTint = new Color(0f, 1f, 0.5f, 1f);
    [SerializeField] Color hoveredTint = new Color(1.2f, 1.2f, 1.2f, 1f);

    [Header("State Weights")]
    [SerializeField] float lockedWeight = 1f;
    [SerializeField] float unlockedWeight = 0.5f;
    [SerializeField] float visitedWeight = 0.3f;
    [SerializeField] float currentWeight = 0.6f;
    [SerializeField] float selectedWeight = 0.4f;
    [SerializeField] float hoveredWeight = 0.3f;

    [SerializeField] SpriteRenderer spriteRenderer;

    private StarVisualState _currentState = StarVisualState.Locked;

    private void Awake() {
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
   
    public void SetState(StarVisualState state, bool active) {
        if (active)
            _currentState |= state;
        else
            _currentState &= ~state; 

        UpdateVisuals();
    }

    public void SetAccessible(bool accessible) {
        if (accessible) {
            SetState(StarVisualState.Locked, false);
            SetState(StarVisualState.Unlocked, true);
        } else {
            SetState(StarVisualState.Unlocked, false);
            SetState(StarVisualState.Locked, true);
        }
    }

    public void SetCurrent(bool isCurrent) {
        SetState(StarVisualState.Current, isCurrent);
    }

    public void SetVisited(bool isVisited) {
        SetState(StarVisualState.Visited, isVisited);
    }

    public void SetSelected(bool isSelected) {
        SetState(StarVisualState.Selected, isSelected);
    }

    private void UpdateVisuals() {
        Color finalColor = baseColor;
        float totalWeight = 0f;

        if (HasState(StarVisualState.Locked)) {
            finalColor = BlendAdditive(finalColor, lockedTint, lockedWeight);
            totalWeight += lockedWeight;
        }

        if (HasState(StarVisualState.Unlocked)) {
            finalColor = BlendAdditive(finalColor, unlockedTint, unlockedWeight);
            totalWeight += unlockedWeight;
        }

        if (HasState(StarVisualState.Visited)) {
            finalColor = BlendAdditive(finalColor, visitedTint, visitedWeight);
            totalWeight += visitedWeight;
        }

        if (HasState(StarVisualState.Current)) {
            finalColor = BlendAdditive(finalColor, currentTint, currentWeight);
            totalWeight += currentWeight;
        }

        if (HasState(StarVisualState.Selected)) {
            finalColor = BlendAdditive(finalColor, selectedTint, selectedWeight);
            totalWeight += selectedWeight;
        }

        if (HasState(StarVisualState.Hovered)) {
            finalColor = BlendAdditive(finalColor, hoveredTint, hoveredWeight);
            totalWeight += hoveredWeight;
        }


        SetBodyColor(finalColor);
    }

    private Color BlendAdditive(Color baseColor, Color tint, float weight) {
        return baseColor + (tint - Color.white) * weight;
    }

    private bool HasState(StarVisualState state) {
        return (_currentState & state) == state;
    }

    // UI Events
    public void OnPointerDown(PointerEventData eventData) {
        Debug.Log("Clicked!");
        OnClicked?.Invoke(this);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        SetState(StarVisualState.Hovered, true);
        transform.DOScale(1.2f, 0.5f).SetEase(Ease.Flash);
    }

    public void OnPointerExit(PointerEventData eventData) {
        SetState(StarVisualState.Hovered, false);
        transform.DOScale(1.0f, 0.5f).SetEase(Ease.Flash);
    }

    // Старі методи
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
        if (spriteRenderer != null)
            spriteRenderer.color = newColor;
    }

    public void SetText(string text) {
        if (this.text != null)
            this.text.text = text;
    }

    private void OnDestroy() {
        transform.DOKill();
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

        View.OnClicked += HandleViewClicked;
        Model.OnAvailabilityChanged += HandleAccessChanged;
        Model.OnCurrentChanged += HandleCurrentChanged;
        Model.OnVisitedChanged += HandleVisitedChanged;
        Model.OnSelectedCHanged += HandleSelectedChanged;

        View.SetAccessible(model.IsAvailable);
        View.SetCurrent(model.IsCurrent);
        View.SetVisited(model.IsVisited);
        View.SetSelected(model.IsSelected);

        View.SetText($"{model.StarCoord}");
    }

    private void HandleSelectedChanged(bool isEnabled) {
        View.SetSelected(isEnabled);
    }

    private void HandleVisitedChanged(bool isEnabled) {
        View.SetVisited(isEnabled);
    }

    private void HandleCurrentChanged(bool isEnabled) {
        View.SetCurrent(isEnabled);
    }

    private void HandleAccessChanged(bool isEnabled) {
        View.SetAccessible(isEnabled);
    }

    private void HandleViewClicked(NavStarView view) {
        Debug.Log($"Star clicked: {Model}");

        _starNavigation.SelectStar(Model);
    }

    public void Dispose() {
        View.OnClicked -= HandleViewClicked;

        Model.OnAvailabilityChanged -= HandleAccessChanged;
        Model.OnCurrentChanged -= HandleCurrentChanged;
        Model.OnVisitedChanged -= HandleVisitedChanged;
        Model.OnSelectedCHanged -= HandleSelectedChanged;
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

    public bool IsAvailable => _isAvailable;
    public bool IsCurrent => _isCurrent;
    public bool IsVisited => _isVisited;
    public bool IsSelected => _isSelected;

    private bool _isAvailable = false;
    private bool _isCurrent = false;
    private bool _isVisited = false;
    private bool _isSelected = false;

    public Action<bool> OnAvailabilityChanged;
    public Action<bool> OnCurrentChanged;
    public Action<bool> OnVisitedChanged;
    public Action<bool> OnSelectedCHanged;
    
    public Star Star => _star;

    public LayerCoord StarCoord => _star.StarCoord;
    public IReadOnlyList<LayerCoord> Connections => _star.Connections;

    private Star _star;

    public NavStar(Star star) {
        _star = star;
    }

    public void SetAvailability(bool isEnabled) {
        if (_isAvailable == isEnabled) return;

        _isAvailable = isEnabled;
        OnAvailabilityChanged?.Invoke(isEnabled);
    }


    public LayerCoord[] GetNextConnections() {
        return _star.GetNextConnections();
    }

    public bool AreConnectedTo(LayerCoord starCoord) {
        return _star.AreConnectedTo(starCoord);
    }

    public void SetVisited(bool isEnabled) {
        if (_isVisited == isEnabled) return;

        _isVisited = isEnabled;
        OnVisitedChanged?.Invoke(isEnabled);
    }

    public void SetCurrent(bool isEnabled) {
        if (_isCurrent == isEnabled) return;

        _isCurrent = isEnabled;
        OnCurrentChanged?.Invoke(isEnabled);
    }

    public void SetSelected(bool isEnabled) {
        if (_isSelected == isEnabled) return;

        _isSelected = isEnabled;
        OnSelectedCHanged?.Invoke(isEnabled);
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
