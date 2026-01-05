using DG.Tweening;
using UnityEngine;

public class SphereCameraRotator : MonoBehaviour {
    [Header("ﾎ魵昕 浯・瑙・")]
    [SerializeField] private float baseRotationDuration = 30f;
    [SerializeField] private bool randomizeAxes = true;
    [SerializeField] private bool useWorldSpace = false;

    [Header("ﾄ鮏瑣・箋 ・・・")]
    [SerializeField] private float minAxisWeight = 0.3f;
    [SerializeField] private float maxAxisWeight = 1f;
    [SerializeField] private bool differentSpeedsPerAxis = false;
    [SerializeField] private Vector3 axisSpeeds = new Vector3(1f, 1.5f, 0.7f);

    void Start() {
        InitializeRotation();
    }
    void InitializeRotation() {
        Vector3 rotationVector;

        if (randomizeAxes) {
            if (differentSpeedsPerAxis) {
                rotationVector = new Vector3(
                    Random.Range(minAxisWeight, maxAxisWeight) * axisSpeeds.x,
                    Random.Range(minAxisWeight, maxAxisWeight) * axisSpeeds.y,
                    Random.Range(minAxisWeight, maxAxisWeight) * axisSpeeds.z
                ) * 360f;
            } else {
                rotationVector = Random.onUnitSphere * 360f;

                rotationVector.x = Mathf.Sign(rotationVector.x) * Mathf.Max(Mathf.Abs(rotationVector.x), minAxisWeight * 360f);
                rotationVector.y = Mathf.Sign(rotationVector.y) * Mathf.Max(Mathf.Abs(rotationVector.y), minAxisWeight * 360f);
                rotationVector.z = Mathf.Sign(rotationVector.z) * Mathf.Max(Mathf.Abs(rotationVector.z), minAxisWeight * 360f);
            }
        } else {
            rotationVector = new Vector3(1f, 1.5f, 0.7f).normalized * 360f;
        }

        var tween = transform.DORotate(rotationVector, baseRotationDuration,
            useWorldSpace ? RotateMode.WorldAxisAdd : RotateMode.LocalAxisAdd)
            .SetLoops(-1, LoopType.Incremental)
            .SetEase(Ease.Linear)
            .SetUpdate(UpdateType.Late); 

        tween.id = "CameraRotation";
    }

    void OnDestroy() {
        DOTween.Kill(transform);
    }
    public void ChangeRotationSpeed(float speedMultiplier) {
        transform.DOKill(false);
        baseRotationDuration /= speedMultiplier;
        InitializeRotation();
    }

    public void RestartWithNewRotation() {
        transform.DOKill(false);
        InitializeRotation();
    }
}
