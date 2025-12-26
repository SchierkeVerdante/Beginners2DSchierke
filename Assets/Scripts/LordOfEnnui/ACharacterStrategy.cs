using UnityEngine;

public abstract class ACharacterStrategy : MonoBehaviour {

    [SerializeField]
    protected GameObject target;

    [SerializeField]
    protected float idealDistanceFromTarget;

    protected virtual void Start() {
        if (target == null) target = LDirectory2D.Instance.player;
    }
    public virtual Vector3 GetFireDirection() {
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
}
