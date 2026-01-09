using System;
using UnityEngine;
using Random = UnityEngine.Random;

public enum EnemyAIState {
    Idle,
    Patrol,
    Combat
}

public abstract class AEnemyStrategy : ACharacterStrategy {

    [SerializeField]
    protected GameObject target;

    [SerializeField]
    protected GameObject facingArrow;

    [SerializeField]
    protected float enemyRange = 20f;

    [SerializeField]
    public float facingAngle, speed;

    [SerializeField]
    protected float idealDistanceFromTarget;

    [SerializeField]
    public bool turnCharacter = true, isAttacking = false;

    [SerializeField, Range(0, 1)]
    public float followVSRepel = 0.5f;

    [SerializeField]
    public float oilDropProb = 0.5f, moduleDropProb = 0.1f, maxOilAmount = 10f;

    [SerializeField]
    LevelState lState;

    public float timeToFire;
    public bool dead;
    public float targetDistance;

    protected virtual void Awake() {
        if (target == null) target = LDirectory2D.Instance.player;
        if (facingArrow != null) facingAngle = facingArrow.transform.rotation.eulerAngles.z;
        lState = LDirectory2D.Instance.lState;
        dead = false;
    }

    public override Vector3 AimDirection() {
        return (target.transform.position - transform.position).normalized;
    }

    public override bool FireThisFrame(ABullet2D bullet, ShootParams shootParams) {
        return !dead && targetDistance < enemyRange;
    }

    public virtual Vector3 IdealPosition() {
        Vector3 targetPosition = target.transform.position;
        targetDistance = Vector3.Distance(targetPosition, transform.position);
        return targetDistance > idealDistanceFromTarget && !dead ? targetPosition : transform.position;
    }

    public override Vector3 MoveDirection() {
        Vector3 repelDir = Vector3.zero;
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position, 20f, 1 << Layers.Enemy)) {
            if (collider.gameObject == gameObject) continue;
            Vector3 direction = collider.transform.position - transform.position;
            repelDir -= direction / direction.sqrMagnitude;
        }

        Vector3 towardsPlayer = (IdealPosition() - transform.position);
        towardsPlayer = towardsPlayer.magnitude > 0.01f ? towardsPlayer.normalized : Vector3.zero;
        repelDir = repelDir.magnitude > 0.01f ? repelDir.normalized : Vector3.zero;

        return targetDistance < enemyRange ? Vector3.Lerp(towardsPlayer, repelDir, followVSRepel) : Vector3.zero;
    }

    public override float FireAngle() {
        return facingAngle;
    }

    public void SetFacingAndSpeed(float facingAngle, float speed) {
        this.facingAngle = facingAngle;
        this.speed = speed;
        if (facingArrow != null) facingArrow.transform.eulerAngles = new Vector3(0, 0, facingAngle);
    }

    public override void SetTimeToFire(float timeToFire, bool thisFrame) {
        this.timeToFire = timeToFire;
        if (thisFrame) {
            lState.onEnemyFire.Invoke(transform);
        }
    }

    public override Vector3 TargetLocation() {
        return target.transform.position;
    }

    public override float GetFireRateMult() {
        return 1.0f;
    }

    public void OnDeath() {
        if (Random.value < oilDropProb) {
            OilPickup oil = Instantiate(lState.oilPrefab);
            oil.amount = (int) Mathf.Max(5f, Random.value * maxOilAmount);
            oil.transform.position = (Vector2) transform.position + 3 * Random.insideUnitCircle;
        }

        if (Random.value < moduleDropProb) {
            ModulePickup module = Instantiate(lState.modulePrefab);
            module.transform.position = (Vector2) transform.position + 3 * Random.insideUnitCircle;
        }

        dead = true;
    }

    public bool TrySprint() {
        return !dead && targetDistance < enemyRange;
    }
}
