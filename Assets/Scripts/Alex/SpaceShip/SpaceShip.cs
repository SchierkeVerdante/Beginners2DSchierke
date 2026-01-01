using System;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using Zenject;

public class Spaceship2D : MonoBehaviour {
    [Header("Movement Settings")]
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float deceleration = 3f;
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private float arrivalDistance = 0.5f;

    [Header("Physics Settings")]
    [SerializeField] private float dragCoefficient = 0.5f;
    [SerializeField] private float rotationDamping = 5f;

    [Header("Orbit Settings")]
    [SerializeField] public OrbitParamsConfig orbitParamsConfig;

    private Vector3 targetPosition;
    private Rigidbody2D rb;
    private float currentSpeed = 0f;
    private float smoothRotationVelocity = 0f;

    public Vector3 TargetPosition => targetPosition;
    public float MaxSpeed => maxSpeed;
    public float CurrentSpeed => currentSpeed;
    public float ArrivalDistance => arrivalDistance;
    public OrbitParamsConfig OrbitConfig => orbitParamsConfig;

    private ContextStateMachine<Spaceship2D> stateMachine;
    public string currentState;

    [Inject]
    public void Construct(IStateFactory<Spaceship2D> stateFactory) {
        stateMachine = new ContextStateMachine<Spaceship2D>(this, stateFactory);
    }

    private void Awake() {
        InitializeRigidbody();
        stateMachine.OnStateChanged += UpdateField;
        stateMachine.ChangeState<IdleSpaceShipState>();
    }

    private void InitializeRigidbody() {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        rb.gravityScale = 0f;
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    private void UpdateField(IState state) {
        currentState = state.ToString();
    }

    private void FixedUpdate() {
        stateMachine?.Tick();
        ApplyDrag();
    }

    private void ApplyDrag() {
        Vector2 velocity = rb.linearVelocity;
        float speed = velocity.magnitude;

        if (speed > 0.01f) {
            Vector2 dragForce = -velocity.normalized * (speed * speed * dragCoefficient);
            rb.AddForce(dragForce);
        }

        float angularVel = rb.angularVelocity;
        if (Mathf.Abs(angularVel) > 0.1f) {
            rb.AddTorque(-angularVel * rotationDamping);
        }
    }

    // ==================== PUBLIC API ====================

    public void SetTarget(Vector3 target) {
        targetPosition = target;
        stateMachine.ChangeState<MovingToTargetState>();
    }

    public void Stop() {
        stateMachine.ChangeState<IdleSpaceShipState>();
    }

    public void StartOrbit() {
        stateMachine.ChangeState<OrbitState>();
    }

    // ==================== QUERY METHODS ====================

    public bool HasReachedTarget() {
        return GetDistanceTo(targetPosition) < arrivalDistance;
    }

    public float GetDistanceTo(Vector3 target) {
        return Vector2.Distance(transform.position, target);
    }

    public Vector2 GetDirectionTo(Vector3 target) {
        return ((Vector2)target - (Vector2)transform.position).normalized;
    }

    public float GetAngleTo(Vector3 target) {
        Vector2 direction = GetDirectionTo(target);
        return DirectionToAngle(direction);
    }

    public float GetAngleDifference(Vector3 target) {
        float targetAngle = GetAngleTo(target);
        float currentAngle = transform.eulerAngles.z;
        return Mathf.DeltaAngle(currentAngle, targetAngle);
    }

    public float GetCurrentVelocityMagnitude() {
        return rb.linearVelocity.magnitude;
    }

    // ==================== MOVEMENT METHODS ====================

    public void ApplyDeceleration(float factor) {
        rb.linearVelocity *= factor;
        rb.angularVelocity *= factor;
        currentSpeed *= factor;

        if (rb.linearVelocity.magnitude < 0.01f) {
            rb.linearVelocity = Vector2.zero;
            currentSpeed = 0f;
        }

        if (Mathf.Abs(rb.angularVelocity) < 0.1f) {
            rb.angularVelocity = 0f;
        }
    }

    public void UpdateSpeed(float targetSpeed, float deltaTime) {
        if (currentSpeed < targetSpeed) {
            currentSpeed = Mathf.Min(
                currentSpeed + acceleration * deltaTime,
                targetSpeed
            );
        } else {
            currentSpeed = Mathf.Max(
                currentSpeed - deceleration * deltaTime,
                targetSpeed
            );
        }
    }

    public void MoveForward() {
        Vector2 forward = transform.up;
        Vector2 desiredVelocity = forward * currentSpeed;
        Vector2 velocityChange = desiredVelocity - rb.linearVelocity;

        float maxChange = acceleration * 2f;
        velocityChange = Vector2.ClampMagnitude(velocityChange, maxChange);

        rb.AddForce(velocityChange * rb.mass, ForceMode2D.Force);
    }


    public void ApplyForce(Vector2 force) {
        rb.AddForce(force);
    }

    public void ClampVelocity(float maxVelocity) {
        float speed = rb.linearVelocity.magnitude;
        if (speed > maxVelocity) {
            float reduction = Mathf.Lerp(1f, 0.95f, (speed - maxVelocity) / maxVelocity);
            rb.linearVelocity *= reduction;
        }
    }

    // ==================== ROTATION METHODS ====================

    public void RotateTowards(Vector2 direction, float smoothTime) {
        float targetAngle = DirectionToAngle(direction);
        float currentAngle = transform.eulerAngles.z;

        float newAngle = Mathf.SmoothDampAngle(
            currentAngle,
            targetAngle,
            ref smoothRotationVelocity,
            smoothTime,
            rotationSpeed
        );

        transform.rotation = Quaternion.Euler(0f, 0f, newAngle);
    }

    public void RotateToAngle(float targetAngle, float smoothTime) {
        float currentAngle = transform.eulerAngles.z;

        float newAngle = Mathf.SmoothDampAngle(
            currentAngle,
            targetAngle,
            ref smoothRotationVelocity,
            smoothTime,
            rotationSpeed
        );

        transform.rotation = Quaternion.Euler(0f, 0f, newAngle);
    }

    // ==================== ORBIT METHODS ====================

    public Vector2 CalculateOrbitPosition(Vector3 center, float angle, float radius) {
        float radians = angle * Mathf.Deg2Rad;
        return (Vector2)center + new Vector2(
            Mathf.Cos(radians) * radius,
            Mathf.Sin(radians) * radius
        );
    }

    public float CalculateOrbitTangentAngle(float orbitAngle) {
        float radians = orbitAngle * Mathf.Deg2Rad;
        Vector2 tangent = new Vector2(-Mathf.Sin(radians), Mathf.Cos(radians));
        return Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg - 90f;
    }

    // ==================== HELPER METHODS ====================

    private float DirectionToAngle(Vector2 direction) {
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
    }

    public void CacheCurrentSpeed() {
        currentSpeed = rb.linearVelocity.magnitude;
    }

    public void ResetRotationVelocity() {
        smoothRotationVelocity = 0f;
    }

    public float GetSmoothRotationVelocity() {
        return smoothRotationVelocity;
    }

    private void OnDestroy() {
        if (stateMachine != null) {
            stateMachine.OnStateChanged -= UpdateField;
        }
    }
}

[Serializable]
public class OrbitParamsConfig {
    public float pGain = 3.0f;
    public float iGain = 0.1f;
    public float dGain = 1.5f;
    public float orbitRadius = 1f;
    public float orbitSpeed = 90f;
}
