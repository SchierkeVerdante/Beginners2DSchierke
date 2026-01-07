using DG.Tweening;
using System;
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
    public Action<Star> OnStarClicked;
    private bool _isSelected;

    private Tween _breathingTween;
    private Vector3 originalScale = Vector3.zero;
    [SerializeField] float _breathingMultiplier = 1.3f;

    private void Awake() {
        originalScale = _visualRoot.transform.localScale;
    }

    public void Initialize(Star star) {
        _star = star;

        _label.text = star.Coord.ToString();
        star.State.OnChanged += HandleStateChanged;
        HandleStateChanged(star.State.Value);
    }

    private void HandleStateChanged(StarState state) {
        Color color = _theme != null ? _theme.GetColor(state) : GetDefaultColor(state);
        _renderer.color = _isSelected ? color * 0.7f : color;

        float targetScale = state == StarState.Current ? originalScale.x * _breathingMultiplier : originalScale.x;
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

        float baseScale = _star.State.Value == StarState.Current ? originalScale.x * _breathingMultiplier : originalScale.x;
        float breathScale = baseScale * originalScale.x * 1.1f;
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
        OnStarClicked?.Invoke(_star);

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

