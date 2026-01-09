using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "LevelState", menuName = "Scriptable Object/Level State")]
public class LevelState : ScriptableObject {
    public bool uiActive;

    [Header("Events")]
    public UnityEvent<bool> onUI;
    public UnityEvent<Transform> onEnemyHit;
    public UnityEvent onBulletHit;
    public UnityEvent<Transform> onEnemyFire;
    public UnityEvent onLevelComplete, onDeath;

    [Header("Drops")]
    public OilPickup oilPrefab;
    public ModulePickup modulePrefab;

    public void SetUIActive(bool active) {
        uiActive = active;
        onUI.Invoke(uiActive);
    }

    public void EnemyHit(Transform enemy) {
        onEnemyHit.Invoke(enemy);
    }
}
