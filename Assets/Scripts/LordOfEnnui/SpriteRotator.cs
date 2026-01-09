using DG.Tweening;
using UnityEngine;

public class SpriteRotator : MonoBehaviour
{
    [SerializeField]
    float rotationDuration = 1.0f;

    [SerializeField]
    AnimationCurve rotationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    [SerializeField]
    Vector3 rotateTo = new Vector3(0, 360, 0);

    private void Start() {
        transform.DORotate(rotateTo, rotationDuration, RotateMode.FastBeyond360).SetRelative(true).SetEase(rotationCurve).SetLoops(-1).SetLink(gameObject);
    }
}
