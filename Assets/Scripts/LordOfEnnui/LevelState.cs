using UnityEngine;
using UnityEngine.Events;

public class LevelState : ScriptableObject {
    public bool uiActive;

    [Header("Events")]
    public UnityEvent<bool> onUI;
    public UnityEvent<Transform> onEnemyHit;
    public UnityEvent onBulletHit;
    public UnityEvent<Transform> onEnemyFire;

    public void SetUIActive(bool active) {
        uiActive = active;
        onUI.Invoke(uiActive);
    }

    public void EnemyHit(Transform enemy) {
        onEnemyHit.Invoke(enemy);
    }
}
