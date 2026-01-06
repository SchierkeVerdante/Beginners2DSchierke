using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController2D : MonoBehaviour {

    Rigidbody2D body;

    [SerializeField]
    float maxSpeed = 7, maxAcceleration = 20, turnSpeed = 10;
    [SerializeField]
    FacingDirectionType facingType = FacingDirectionType.Free;
    [SerializeField]
    public bool canMove = true;

    [Header("Out (ReadOnly)")]

    [SerializeField]
    Vector3 targetVelocity, velocity;
    [SerializeField]
    float trueFacingAngle;
    [SerializeField]
    AEnemyStrategy moveStrat;

    private void Start() {
        body = GetComponent<Rigidbody2D>();
        moveStrat = GetComponent<AEnemyStrategy>();
        trueFacingAngle = moveStrat.facingAngle;
    }

    void Update() {
        targetVelocity = moveStrat.MoveDirection() * maxSpeed;
        if (moveStrat.turnCharacter) {
            Vector3 aimDirection = moveStrat.AimDirection();
            float targetAngle = (360 + Mathf.Sign(aimDirection.y) * Mathf.Acos(Vector3.Dot(transform.right, aimDirection)) * Mathf.Rad2Deg) % 360;
            trueFacingAngle = Mathf.MoveTowardsAngle(trueFacingAngle, targetAngle, turnSpeed * Time.deltaTime * 60f);
            moveStrat.SetFacingAndSpeed(Mathf.Round(trueFacingAngle / (int) facingType) * (int) facingType, targetVelocity.magnitude);
        }
    }

    private void FixedUpdate() {
        if (!canMove) {
            body.linearVelocity = Vector3.zero;
            return;
        }
        velocity = body.linearVelocity;

        float acceleration = maxAcceleration;
        float maxSpeedChange = acceleration * Time.fixedDeltaTime;
        velocity.x =
            Mathf.MoveTowards(velocity.x, targetVelocity.x, maxSpeedChange);
        velocity.y =
            Mathf.MoveTowards(velocity.y, targetVelocity.y, maxSpeedChange);

        body.linearVelocity = velocity;
    }
}
