using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class Shaker : MonoBehaviour {

    [SerializeField, Header("Persistent Data")]
    ShakeParams defaultShake;

    [SerializeField]
    bool shaking, shake;

    [SerializeField]
    AnimationCurve shakeCurve;

    [SerializeField]
    Vector3 targetPos;

    [SerializeField]
    float speed;

    private Vector3 initialPosition, initialEulerAngles;

    private void Awake() {
        initialPosition = transform.localPosition;
        initialEulerAngles = transform.localEulerAngles;
        targetPos = transform.localPosition;
        if (defaultShake == null) defaultShake = ScriptableObject.CreateInstance<ShakeParams>();
    }

    private void OnValidate() {
        Awake();
    }

    private void Update() {
        if (shaking) transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, speed * Time.deltaTime);

        if (shake) {
            shake = false;
            StartCoroutine(Shake(defaultShake));
        }
    }

    public void ShakeObject(ShakeParams shake = null) {
        if (shake == null) shake = defaultShake;
        StartCoroutine(Shake(shake));
    }

    private IEnumerator Shake(ShakeParams shake) {
        if (shaking) yield break;
        shaking = true;
        speed = shake.frequency;
        WaitForSeconds wait = new(1 / speed);
        int numShakes = (int) (shake.duration * shake.frequency);
        Vector3 targetDir = shake.direction;
        targetPos = initialPosition;

        for (int i = 0; i < numShakes; i++) {
            targetDir = (-targetDir + Random.insideUnitSphere * shake.randomness).normalized;
            targetPos = initialPosition + shake.maxAmplitude * Mathf.Pow(shakeCurve.Evaluate((float) i / numShakes), shake.exponent) * targetDir;
            yield return wait;
        }
        targetPos = initialPosition;
        transform.localPosition = targetPos;
        shaking = false;
    }
}
