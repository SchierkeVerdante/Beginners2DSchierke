using UnityEngine;

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
    public float facingAngle, speed;

    [SerializeField]
    protected float idealDistanceFromTarget;

    [SerializeField]
    public bool turnCharacter = true, isAttacking = false;

    [SerializeField, Range(0, 1)]
    public float followVSRepel = 0.5f;

    [SerializeField]
    LevelState lState;

    public float timeToFire;

    protected virtual void Awake() {
        if (target == null) target = LDirectory2D.Instance.player;
        if (facingArrow != null) facingAngle = facingArrow.transform.rotation.eulerAngles.z;
        lState = LDirectory2D.Instance.lState;
    }

    public override Vector3 AimDirection() {
        return (target.transform.position - transform.position).normalized;
    }

    public override bool FireThisFrame(ABullet2D bullet, ShootParams shootParams) {
        return true;
    }

    public virtual Vector3 IdealPosition() {
        Vector3 targetPosition = target.transform.position;
        float targetDistance = Vector3.Distance(targetPosition, transform.position);
        return targetDistance > idealDistanceFromTarget ? targetPosition : transform.position;
    }

    public override Vector3 MoveDirection() {
        Vector3 repelDir = Vector3.zero;
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position, 20f, 1 << Layers.Enemy)) {
            if (collider.gameObject == gameObject) continue;
            Vector3 direction = collider.transform.position - transform.position;
            repelDir -= direction / direction.sqrMagnitude;
        }
        return Vector3.Lerp((IdealPosition() - transform.position).normalized, repelDir.normalized, followVSRepel);
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
}
