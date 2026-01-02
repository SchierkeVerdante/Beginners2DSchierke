using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "PlayerState", menuName = "Scriptable Object/Player State")]
public class PlayerState : ScriptableObject {
   
    [Header("Status")]
    public float requiredOil = 80;
    public float maxHealth = 5, currentHealth = 4;
    public float maxOil = 100, currentOil = 0;

    [Header("Invincibility")]
    public float flashesPerSecond = 2;
    public float damageIframes = 60, sprintIframes = 30;

    [Header("HitStop")]
    public float damageHitStopDuration = 0.1f;
    public float damageHitStopTimeScale = 0.0f;

    [Header("Events")]
    public UnityEvent onDamage;
    public UnityEvent onDeath;

    public void TakeDamage(float amount) {
        currentHealth -= amount;
        onDamage.Invoke();
        if (currentHealth < 0) {
            onDeath.Invoke();
        }
    }
}
