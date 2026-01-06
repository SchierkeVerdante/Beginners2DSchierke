using DG.Tweening;
using UnityEngine;

public class SpriteBobber : MonoBehaviour
{
    [SerializeField]
    float amplitude = 0.7f, duration = 0.333f;

    [SerializeField]
    Tween tween;

    [SerializeField]
    AnimationCurve curve;

    private void Start() {
        tween = transform.DOLocalMoveY(transform.localPosition.y + amplitude / 2, duration / 2)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(curve)
            .SetLink(gameObject);
    }

    public void ResetTween() {
        tween.Restart();
    }
}
