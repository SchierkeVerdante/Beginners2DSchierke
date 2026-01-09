using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class StarView : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
    [Header("References")]
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private TextMeshPro _label;
    [SerializeField] private Transform _visualRoot;
    [SerializeField] private StarVisualTheme _theme;

    [Header("Animation Settings")]
    [SerializeField] private float _breathingScaleMultiplier = 1.3f;
    [SerializeField] private float _hoverScaleMultiplier = 1.15f;
    [SerializeField] private float _breathingDuration = 0.8f;
    [SerializeField] private float _hoverTransitionDuration = 0.2f;

    private Star _star;
    public Action<Star> OnStarClicked;

    private Vector3 _baseScale = Vector3.one;
    private Tween _breathingTween;
    private Tween _hoverTween;

    private Dictionary<LayerCoord, LineRenderer> _connectionLines = new();

    private void Awake() {
        if (_visualRoot != null) {
            _baseScale = _visualRoot.localScale;
        }
    }

    public void Initialize(Star star) {
        if (star == null) {
            Debug.LogError("Cannot initialize StarView with null star");
            return;
        }

        _star = star;
        _label.text = star.ToString();

        ApplyPlanetConfig(star.PlanetConfig);
        SubscribeToStarEvents();
        UpdateVisualState();
    }

    private void SubscribeToStarEvents() {
        _star.IsAvailable.OnChanged += OnAvailabilityChanged;
        _star.IsSelected.OnChanged += OnSelectionChanged;
        _star.IsCurrent.OnChanged += OnCurrentChanged;
    }

    private void UnsubscribeFromStarEvents() {
        if (_star == null) return;

        _star.IsAvailable.OnChanged -= OnAvailabilityChanged;
        _star.IsSelected.OnChanged -= OnSelectionChanged;
        _star.IsCurrent.OnChanged -= OnCurrentChanged;
    }

    private void ApplyPlanetConfig(PlanetConfig config) {
        if (config == null) return;

        if (config.planetSprite != null) {
            UpdateSprite(config.planetSprite);
        }

        Vector3 targetScale = new Vector3(
            config.normalizedVolume,
            config.normalizedVolume,
            config.normalizedVolume
        );
        UpdatePlanetSize(targetScale);
    }

    public void UpdateSprite(Sprite newSprite) {
        if (_renderer == null || newSprite == null) return;
        _renderer.sprite = newSprite;
    }

    public void UpdatePlanetSize(Vector3 newScale) {
        if (_visualRoot == null) return;
        _baseScale = newScale;
        _visualRoot.localScale = _baseScale;
    }

    public void AddConnectionLine(LayerCoord to, LineRenderer line) {
        _connectionLines[to] = line;
        line.material.color = GetLineColor();
    }

    public void UpdateConnectionLinesColor(Color color) {
        if (_connectionLines == null) return;

        foreach (var line in _connectionLines.Values) {
            if (line != null && line.material != null) {
                line.material.color = color;
            }
        }
    }

    private void OnAvailabilityChanged(bool isAvailable) {
        UpdateVisualState();
    }

    private void OnCurrentChanged(bool isCurrent) {
        UpdateVisualState();
    }

    private void OnSelectionChanged(bool isSelected) {
        UpdateVisualState();
    }

    private void UpdateVisualState() {
        if (_star == null) return;

        Color planetColor = GetPlanetColor();
        ApplyPlanetColor(planetColor);

        bool shouldBreathe = _star.IsAvailable.Value && !_star.IsCurrent.Value;
        ToggleBreathingAnimation(shouldBreathe);

        Color lineColor = GetLineColor();
        UpdateConnectionLinesColor(lineColor);
    }

    private Color GetPlanetColor() {
        // Priority: Current > Selected > Available > Locked
        if (_star.IsCurrent.Value) {
            return _theme.CurrentColor;
        }

        if (_star.IsSelected.Value) {
            return _theme.SelectedColor;
        }

        if (_star.IsAvailable.Value) {
            return _theme.AvailableColor;
        }

        return _theme.LockedColor;
    }

    private Color GetLineColor() {
        if (_star.IsVisited.Value && _star.IsCurrent.Value) {
            return _theme.AvailableLineColor;
        }
        return _theme.LockedLineColor;
    }

    private void ApplyPlanetColor(Color color) {
        if (_renderer != null) {
            _renderer.color = color;
        }
    }

    private void ToggleBreathingAnimation(bool shouldBreathe) {
        bool isBreathing = _breathingTween != null && _breathingTween.IsPlaying();

        if (_visualRoot == null) return;

        if (!shouldBreathe && isBreathing) {
            _breathingTween?.Kill();
            _breathingTween = null;
            _visualRoot.localScale = _baseScale;
            return;
        }

        if (shouldBreathe && !isBreathing) {
            _visualRoot.localScale = _baseScale;
            Vector3 targetScale = _baseScale * _breathingScaleMultiplier;

            _breathingTween = _visualRoot.DOScale(targetScale, _breathingDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .OnKill(() => _breathingTween = null);
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (_star != null) {
            OnStarClicked?.Invoke(_star);
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (_visualRoot == null || _star == null) return;

        _breathingTween?.Pause();
        _hoverTween?.Kill();

        Vector3 targetScale = _baseScale * _hoverScaleMultiplier;
        _hoverTween = _visualRoot.DOScale(targetScale, _hoverTransitionDuration)
            .SetEase(Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData) {
        _hoverTween?.Kill();
        _hoverTween = _visualRoot.DOScale(_baseScale, _hoverTransitionDuration)
            .SetEase(Ease.OutBack)
            .OnComplete(() => {
                if (_star.IsAvailable.Value && !_star.IsCurrent.Value) {
                    ToggleBreathingAnimation(true);
                }
            });
    }

    private void OnDestroy() {
        UnsubscribeFromStarEvents();
        _breathingTween?.Kill();
        _visualRoot?.DOKill();
        _connectionLines.Clear();
    }
}