using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController2D : MonoBehaviour {
    Rigidbody2D body;

    [SerializeField]
    float maxSpeed = 5, maxAcceleration = 20;
    [SerializeField]
    public bool canMove = true;

    [Header("Out (ReadOnly)")]

    [SerializeField]
    Vector3 targetVelocity, velocity;
    [SerializeField]
    ACharacterStrategy moveStrat;

    private void Start() {
        body = GetComponent<Rigidbody2D>();
        moveStrat = GetComponent<ACharacterStrategy>();
    }

    void Update() {
        Vector3 targetDirection = moveStrat.IdealPosition() - transform.position;
        targetVelocity = targetDirection * maxSpeed;
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
