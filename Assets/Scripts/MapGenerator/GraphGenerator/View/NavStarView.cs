using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Zenject;

public enum StarState {
    Locked,
    Available,
    Visited,
    Current,
    Selected
}

public class StarView : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private TextMeshPro _label;
    [SerializeField] private Transform _visualRoot;
    [SerializeField] private StarVisualTheme _theme;

    private Star _star;
    private Action<Star> _onClickCallback;
    private bool _isSelected;

    private Tween _breathingTween;

    public void Initialize(Star star, Action<Star> onClick) {
        _star = star;
        _onClickCallback = onClick;

        _label.text = star.Coord.ToString();
        star.State.OnChanged += HandleStateChanged;
        HandleStateChanged(star.State.Value);
    }

    private void HandleStateChanged(StarState state) {
        Color color = _theme != null ? _theme.GetColor(state) : GetDefaultColor(state);
        _renderer.color = _isSelected ? color * 0.7f : color;

        float targetScale = state == StarState.Current ? 1.3f : 1f;
        _visualRoot.DOScale(targetScale, 0.3f).SetEase(Ease.OutBack);

        ToggleAvailableAnimation(state == StarState.Available);
    }

    public void SetSelected(bool selected) {
        _isSelected = selected;
        HandleStateChanged(_star.State.Value);
    }

    private void ToggleAvailableAnimation(bool isEnabled) {
        _breathingTween?.Kill();

        if (!isEnabled) {
            _breathingTween = null;
            return;
        }

        float baseScale = _star.State.Value == StarState.Current ? 1.3f : 1f;
        float breathScale = baseScale * 1.1f;
        float breathDuration = 0.8f;

        _breathingTween = _visualRoot.DOScale(breathScale, breathDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private Color GetDefaultColor(StarState state) => state switch {
        StarState.Locked => new Color(0.3f, 0.3f, 0.3f),
        StarState.Available => new Color(1f, 0.9f, 0.3f),
        StarState.Visited => new Color(0.4f, 0.6f, 1f),
        StarState.Current => new Color(0.3f, 1f, 0.3f),
        _ => Color.white
    };

    public void OnPointerClick(PointerEventData eventData) =>
        _onClickCallback?.Invoke(_star);

    public void OnPointerEnter(PointerEventData eventData) {
        _breathingTween?.Pause();

        Vector3 currentScale = _visualRoot.localScale;
        _visualRoot.DOScale(currentScale * 1.15f, 0.2f);
    }

    public void OnPointerExit(PointerEventData eventData) {
        float targetScale = _star.State.Value == StarState.Current ? 1.3f : 1f;
        _visualRoot.DOScale(targetScale, 0.2f)
            .OnComplete(() => {
                _breathingTween?.Play();
            });
    }

    private void OnDestroy() {
        if (_star != null) {
            _star.State.OnChanged -= HandleStateChanged;
        }
        _breathingTween?.Kill();
        _visualRoot?.DOKill();
    }
}

public readonly struct LayerCoord : IEquatable<LayerCoord> {
    public int Layer { get; }
    public int Index { get; }

    public LayerCoord(int layer, int index) {
        Layer = layer;
        Index = index;
    }

    public bool Equals(LayerCoord other) =>
        Layer == other.Layer && Index == other.Index;

    public override bool Equals(object obj) =>
        obj is LayerCoord other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Layer, Index);

    public override string ToString() => $"L{Layer}:I{Index}";
}

public class Star {
    public Guid Id { get; }
    public LayerCoord Coord { get; }

    private readonly HashSet<LayerCoord> _connections = new();
    public string Name { get; set; }

    public IReadOnlyCollection<LayerCoord> Connections => _connections;

    public ReactiveProperty<StarState> State { get; }

    public Star(int layer, int index, string name = "Unknown") {
        Id = Guid.NewGuid();
        Coord = new LayerCoord(layer, index);
        State = new ReactiveProperty<StarState>(StarState.Locked);
        Name = name;
    }

    public void ConnectTo(LayerCoord coord) => _connections.Add(coord);
    public bool IsConnectedTo(LayerCoord coord) => _connections.Contains(coord);

    public IEnumerable<LayerCoord> GetForwardConnections() =>
        _connections.Where(c => c.Layer > Coord.Layer);

    public override string ToString() => $"{Name} {Coord}";
}

