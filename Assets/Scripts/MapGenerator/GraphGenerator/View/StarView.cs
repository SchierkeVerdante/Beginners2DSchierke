using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class StarView : MonoBehaviour, IView, IPointerDownHandler {
    public Action<StarView> OnClicked { get; internal set; }
    [SerializeField] TextMeshPro text;

    [Header("Line settings")]
    [SerializeField] Material lineMaterial;
    [SerializeField] float lineWidth = 0.05f;

    [SerializeField] Color lockedColorTint;
    [SerializeField] Color unlockedColorTint;
    [SerializeField] Color completedColorTint;

    [SerializeField] SpriteRenderer spriteRenderer;

    private bool isInteractable = false;

    private void Awake() {
        if (spriteRenderer == null)
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetInteractable(bool isEnabled) {
        isInteractable = isEnabled;
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (!isInteractable) return;

        Debug.Log("Clicked!");

        OnClicked?.Invoke(this);
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

    public void SetState(StarState newState) {
        switch (newState) {
            case StarState.Available:
                SetBodyColor(unlockedColorTint);
                break;
            case StarState.Locked:
                SetBodyColor(lockedColorTint);
                break;
            case StarState.Completed:
                SetBodyColor(completedColorTint);
                break;
            default:
                break;
        }
    }

    private void SetBodyColor(Color newColor) {
        if (spriteRenderer != null)
        spriteRenderer.color = newColor;
    }

    public void SetText(string text) {
        if (this.text != null)
            this.text.text = text;
    }
}


public class StarPresenter {
    public Star Model { get; private set; }
    public StarView View { get; private set; }
    private IStarMapService starMapService;

    public StarPresenter(Star model, StarView view) {
        Model = model;
        View = view;

        View.OnClicked += HandleViewClicked;

        Model.OnStateChanged += HandleStateChanged;

        View.SetState(model.CurrentState);
        View.SetText($"{model.StarCoord}");
    }

    private void HandleStateChanged(StarState newState) {
        View.SetState(newState);
    }

    private void HandleViewClicked(StarView view) {
        // Тут логіка перевірки: чи можна активувати цю зірку?
        Debug.Log($"Star clicked: {Model}");
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

public class Star : IModel {
    private StarState _starState = StarState.Locked;
    public Guid Id { get; }

    public LayerCoord StarCoord { get; }

    private readonly List<LayerCoord> _connections = new();
    public IReadOnlyList<LayerCoord> Connections => _connections.AsReadOnly();

    public Action<StarState> OnStateChanged;
    public StarState CurrentState => _starState;

    public Star(int layer, int layerIndex) {
        Id = Guid.NewGuid();
        StarCoord = new LayerCoord(layer, layerIndex);
    }

    public void SetState(StarState newState) {
        if (newState == _starState) return;

        _starState = newState;
        OnStateChanged?.Invoke(newState);
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
        return _connections.ToArray();
    }

    public void ClearConnections() {
        _connections.Clear();
    }

    public override string ToString() {
        return $"Star {StarCoord}";
    }
}


public enum StarState {
    Locked,
    Available,
    Current,
    Completed
}
