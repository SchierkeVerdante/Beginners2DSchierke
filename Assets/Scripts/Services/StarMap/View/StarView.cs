using DG.Tweening;
using System;
using System.Net.Sockets;
using TMPro;
using Unity.VisualScripting;
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
    [Header("References")]
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private TextMeshPro _label;
    [SerializeField] private Transform _visualRoot;
    [SerializeField] private StarVisualTheme _theme;

    [Header("Animation Settings")]
    [SerializeField] private float _breathingScaleMultiplier = 1.3f;
    [SerializeField] private float _hoverScaleMultiplier = 1.15f;
    [SerializeField] private float _stateTransitionDuration = 0.3f;
    [SerializeField] private float _hoverTransitionDuration = 0.2f;
    [SerializeField] private float _breathingDuration = 0.8f;

    private Star _star;
    public Action<Star> OnStarClicked;
    private bool _isSelected;

    private Vector3 _baseScale = Vector3.one;

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

        if (_star != null) {
            _star.State.OnChanged -= HandleStateChanged;
        }

        star.State.OnChanged += HandleStateChanged;
        HandleStateChanged(star.State.Value);

        PlanetConfig planetConfig = star.PlanetConfig;
        if (planetConfig != null) {
            if (planetConfig.planetSprite != null) {
                UpdateSprite(planetConfig.planetSprite);
            }
            Vector3 targetScale = new Vector3(planetConfig.normalizedVolume, planetConfig.normalizedVolume, planetConfig.normalizedVolume);
            UpdatePlanetSize(targetScale);
        }
    }

    public void UpdateSprite(Sprite newSprite) {
        if (_renderer == null || newSprite == null) return;
        _renderer.sprite = newSprite;
    }

    public void UpdatePlanetSize(Vector3 newScale) {
        if (_visualRoot == null) return;

        bool wasBreathing = _breathingTween != null && _breathingTween.IsPlaying();

        if (wasBreathing) {
            ToggleAvailableAnimation(false);
        }

        _baseScale = newScale;
        _visualRoot.localScale = _baseScale;

        if (wasBreathing) {
            ToggleAvailableAnimation(true);
        }
    }

    private void HandleStateChanged(StarState state) {
        UpdateColorForState(state);
        ToggleAvailableAnimation(state == StarState.Available);
    }

    private void UpdateColorForState(StarState state) {
        if (_renderer == null) return;

        _renderer.color = _theme.GetColor(state);
    }

    public void SetSelected(bool selected) {
        _isSelected = selected;
        if (_star == null) return; 

        StarState currentState = _star.State.Value;
        StarState state = _isSelected ? StarState.Selected : currentState;

        UpdateColorForState(state);
    }

    private Tween _breathingTween;

    private void ToggleAvailableAnimation(bool isEnabled) {
        bool wasBreathing = _breathingTween != null && _breathingTween.IsPlaying();

        if (_visualRoot == null) return;

        if (!isEnabled && wasBreathing) {
            _breathingTween?.Kill();
            _breathingTween = null;
            _visualRoot.localScale = _baseScale;

            return;
        }

        _visualRoot.localScale = _baseScale;
        Vector3 targetScale = _baseScale * _breathingScaleMultiplier;

        _breathingTween = _visualRoot.DOScale(targetScale, _breathingDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .OnKill(() => _breathingTween = null);
    }


    public void OnPointerClick(PointerEventData eventData) {
        if (_star != null) {
            OnStarClicked?.Invoke(_star);
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (_visualRoot == null) return;

        _breathingTween?.Pause();

        Vector3 targetScale = _baseScale * _hoverScaleMultiplier;
        _visualRoot.DOScale(targetScale, _hoverTransitionDuration)
            .SetEase(Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (_visualRoot == null) return;

        _visualRoot.DOScale(_baseScale, _hoverTransitionDuration)
            .SetEase(Ease.OutBack)
            .OnComplete(() => {
                _breathingTween?.Play();
            });
    }

    private void OnDestroy() {
        if (_star != null) {
            _star.State.OnChanged -= HandleStateChanged;
        }

        _breathingTween?.Kill();

        if (_visualRoot != null) {
            _visualRoot.DOKill();
        }
    }
}