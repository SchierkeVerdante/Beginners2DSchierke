using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using Zenject;

public class Spaceship2D : MonoBehaviour {
    [Header("Movement Settings")]
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private float arrivalDistance = 0.5f;

    private Vector3 targetPosition;
    private Rigidbody2D rb;

    public Vector3 TargetPosition => targetPosition;
    public Rigidbody2D Rigidbody => rb;
    public float MaxSpeed => maxSpeed;
    public float Acceleration => acceleration;
    public float RotationSpeed => rotationSpeed;
    public float ArrivalDistance => arrivalDistance;

    private ContextStateMachine<Spaceship2D> stateMachine;

    [Inject]
    public void Construct(IStateFactory<Spaceship2D> stateFactory) {
        stateMachine = new ContextStateMachine<Spaceship2D>(this, stateFactory);
    }

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        rb.gravityScale = 0f;
        rb.linearDamping = 1f;
        rb.angularDamping = 3f;

        stateMachine.ChangeState<IdleSpaceShipState>();
    }

    private void Update() {
        stateMachine?.UpdateState();
    }

    public void SetTarget(Vector3 target) {
        targetPosition = target;
        stateMachine.ChangeState<MovingToTargetState>(); 
    }

    public void Stop() {
        stateMachine.ChangeState<IdleSpaceShipState>(); 
    }

    public bool HasReachedTarget() {
        return Vector2.Distance(transform.position, targetPosition) < arrivalDistance;
    }
}

public class IdleSpaceShipState : State<Spaceship2D> {

    public override void Enter() {
        Context.Rigidbody.linearVelocity = Vector2.zero;
    }

    public override void Exit() { }
}

public class MovingToTargetState : State<Spaceship2D> {
    private float currentSpeed = 0f;

    public override void Enter() {
        currentSpeed = 0f;
    }

    public override void Update() {
        Vector2 currentPosition = Context.transform.position;
        Vector2 directionToTarget = (Vector2)Context.TargetPosition - currentPosition;
        float distanceToTarget = directionToTarget.magnitude;

        if (distanceToTarget < Context.ArrivalDistance) {
            Context.Stop();
            return;
        }

        // Обчислення кута
        float targetAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg - 90f;
        float currentAngle = Context.transform.eulerAngles.z;
        float angleDifference = Mathf.DeltaAngle(currentAngle, targetAngle);

        // Поворот
        float rotationStep = Context.RotationSpeed * Time.deltaTime;
        float newAngle = currentAngle + Mathf.Clamp(angleDifference, -rotationStep, rotationStep);
        Context.transform.rotation = Quaternion.Euler(0f, 0f, newAngle);

        // Рух
        float alignmentThreshold = 30f;
        if (Mathf.Abs(angleDifference) < alignmentThreshold) {
            currentSpeed = Mathf.Min(currentSpeed + Context.Acceleration * Time.deltaTime, Context.MaxSpeed);

            if (distanceToTarget < Context  .ArrivalDistance * 3f) {
                float slowdownFactor = distanceToTarget / (Context.ArrivalDistance * 3f);
                currentSpeed *= slowdownFactor;
            }
        } else {
            currentSpeed = Mathf.Max(currentSpeed - Context.Acceleration * Time.deltaTime * 2f, 0f);
        }

        Vector2 forward = Context.transform.up;
        Context.Rigidbody.linearVelocity = forward * currentSpeed;
    }

    public override void Exit() {
        Context.Rigidbody.linearVelocity = Vector2.zero;
    }
}