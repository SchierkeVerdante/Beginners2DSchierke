using UnityEngine;

[CreateAssetMenu(fileName = "ScreenShakeParams", menuName = "Scriptable Object/Screen Shake Params")]
public class ShakeParams : ScriptableObject {
    [SerializeField]
    public float maxAmplitude = 1, frequency = 10, duration = 1, randomness = 0.2f;

    [SerializeField]
    public Vector3 direction = Vector3.right;

    [SerializeField]
    public int exponent = 3;
}
