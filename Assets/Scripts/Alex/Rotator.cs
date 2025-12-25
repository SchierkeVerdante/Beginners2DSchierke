using UnityEngine;
using DG.Tweening;

public class Rotator : MonoBehaviour {
    [SerializeField] private float rotationDuration = 5f;
    [SerializeField] private RotateMode rotateMode = RotateMode.FastBeyond360;

    private RectTransform rectTransform;

    void Start() {
        rectTransform = GetComponent<RectTransform>();

        // Безперервне обертання
        rectTransform.DORotate(new Vector3(0, 0, 360), rotationDuration, rotateMode)
            .SetLoops(-1, LoopType.Restart) 
            .SetEase(Ease.Linear); 
    }

    void OnDestroy() {
        rectTransform.DOKill();
    }
}