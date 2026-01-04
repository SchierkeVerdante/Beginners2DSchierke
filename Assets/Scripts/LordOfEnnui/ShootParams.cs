using UnityEngine;

[CreateAssetMenu(fileName = "ShootParams", menuName = "Scriptable Object/ShootParams")]
public class ShootParams : ScriptableObject {

    public float fireRate = 3f, bulletSpeed = 10f, fireSpread = 10f, recoilForce = 5f, placeDistance = 0.5f;

    public FireStreamType fireStreamType;

    [Range(-180, 180), Header("Custom")]
    public float[] customFireStreams = new[] { 0f };

    [Range(0, 90), Header("EvenSpacing")]
    public float streamSpacing = 30f;
    [Range(1, 30)]
    public int numberOfStreams = 3;

    [Header("MovementEffects")]
    public float startUpDuration = 0f;

    [Range(0f, 1f)]
    public float moveSpeedMultiplier = 1f;
}
