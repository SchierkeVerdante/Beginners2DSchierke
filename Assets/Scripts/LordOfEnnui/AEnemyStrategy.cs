using UnityEngine;

public enum EnemyAIState {
    Idle,
    Patrol,
    Combat
}

public abstract class AEnemyStrategy : MonoBehaviour {

    [SerializeField]
    EnemyAIState state = EnemyAIState.Idle;

    [SerializeField]
    protected GameObject target;

    [SerializeField]
    protected GameObject facingArrow;

    [SerializeField]
    public float facingAngle;

    [SerializeField]
    protected float idealDistanceFromTarget;

    [SerializeField]
    public bool turnCharacter = true;

    protected virtual void Awake() {
        if (target == null) target = LDirectory2D.Instance.player;
        if (facingArrow != null) facingAngle = facingArrow.transform.rotation.eulerAngles.z;
    }



    public virtual Vector3 GetAimDirection() {
        return (target.transform.position - transform.position).normalized;
    }

    public virtual bool FireThisFrame(ABullet2D bullet) {
        return true;
    }

    public virtual Vector3 IdealPosition() {
        Vector3 targetPosition = target.transform.position;
        float targetDistance = Vector3.Distance(targetPosition, transform.position);
        return targetDistance > idealDistanceFromTarget ? targetPosition : transform.position;
    }

    public void SetFacing(float facingAngle) {
        this.facingAngle = facingAngle;
        if (facingArrow != null) facingArrow.transform.eulerAngles = new Vector3(0, 0, facingAngle);
    }
}
