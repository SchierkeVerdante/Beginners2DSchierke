using UnityEngine;

public abstract class ACharacterStrategy : MonoBehaviour {

    [SerializeField]
    protected GameObject target;

    [SerializeField]
    protected GameObject facingArrow;

    [SerializeField]
    public float facingAngle;

    [SerializeField]
    protected float idealDistanceFromTarget;

    protected virtual void Awake() {
        if (target == null) target = LDirectory2D.Instance.player;
        facingAngle = facingArrow.transform.rotation.eulerAngles.z;
    }
    public virtual Vector3 GetAimDirection() {
        return (target.transform.position - transform.position).normalized;
    }

    public virtual Vector3[] GetFireDirections() {
        return new[] { Quaternion.AngleAxis(facingAngle, transform.forward) * transform.right };
    }

    public virtual bool FireThisFrame(ABullet2D bullet) {
        return true;
    }

    public virtual Vector3 IdealPosition() {
        Vector3 targetPosition = target.transform.position;
        float targetDistance = Vector3.Distance(targetPosition, transform.position);
        return targetDistance > idealDistanceFromTarget ? targetPosition : transform.position;
    }
    public bool UpdateFacing() {
        return true;
    }

    public void SetFacing(float facingAngle) {
        this.facingAngle = facingAngle;
        facingArrow.transform.eulerAngles = new Vector3(0, 0, facingAngle);
    }
}
