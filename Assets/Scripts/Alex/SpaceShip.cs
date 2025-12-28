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

    private Vector3 targetPosition;
    private Rigidbody2D rb;

    // Для збереження швидкості між станами
    private float currentSpeed = 0f;
    private float targetRotationVelocity = 0f;
    [SerializeField] public OrbitParamsConfig OrbitParamsConfig;

    public Vector3 TargetPosition => targetPosition;
    public Rigidbody2D Rigidbody => rb;
    public float MaxSpeed => maxSpeed;
    public float Acceleration => acceleration;
    public float Deceleration => deceleration;
    public float RotationSpeed => rotationSpeed;
    public float ArrivalDistance => arrivalDistance;
    public float DragCoefficient => dragCoefficient;
    public float RotationDamping => rotationDamping;

    public float CurrentSpeed {
        get => currentSpeed;
        set => currentSpeed = value;
    }

    public float TargetRotationVelocity {
        get => targetRotationVelocity;
        set => targetRotationVelocity = value;
    }

    private ContextStateMachine<Spaceship2D> stateMachine;

    public string currentState;

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
        rb.linearDamping = 0f;  // Вимикаємо вбудоване демпфування
        rb.angularDamping = 0f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate; // Плавніша анімація

        stateMachine.OnStateChanged += UpdateField;
        stateMachine.ChangeState<IdleSpaceShipState>();
    }

    private void UpdateField(IState state) {
        currentState = state.ToString();
    }

    private void Update() {
        
    }

    private void FixedUpdate() {
        stateMachine?.UpdateState();
        // Застосування власного drag для реалістичності
        ApplyDrag();
    }

    private void ApplyDrag() {
        // Квадратичний drag (більш реалістично для космосу)
        Vector2 velocity = rb.linearVelocity;
        float speed = velocity.magnitude;

        if (speed > 0.01f) {
            Vector2 dragForce = -velocity.normalized * (speed * speed * dragCoefficient);
            rb.AddForce(dragForce);
        }

        // Демпфування обертання
        float angularVel = rb.angularVelocity;
        if (Mathf.Abs(angularVel) > 0.1f) {
            rb.AddTorque(-angularVel * rotationDamping);
        }
    }

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

    public bool HasReachedTarget() {
        return Vector2.Distance(transform.position, targetPosition) < arrivalDistance;
    }

    private void OnDestroy() {
        if (stateMachine != null) {
            stateMachine.OnStateChanged -= UpdateField;
        }
    }
}

public class IdleSpaceShipState : State<Spaceship2D> {
    private float decelerationTime = 0f;

    public override void Enter() {
        decelerationTime = 0f;
    }

    public override void Update() {
        // Плавна зупинка замість миттєвої
        decelerationTime += Time.deltaTime;
        float decelerationFactor = Mathf.Exp(-Context.Deceleration * decelerationTime);

        Context.Rigidbody.linearVelocity *= decelerationFactor;
        Context.CurrentSpeed *= decelerationFactor;

        // Зупинити обертання
        Context.Rigidbody.angularVelocity *= decelerationFactor;

        // Повна зупинка при малих швидкостях
        if (Context.Rigidbody.linearVelocity.magnitude < 0.01f) {
            Context.Rigidbody.linearVelocity = Vector2.zero;
            Context.CurrentSpeed = 0f;
        }

        if (Mathf.Abs(Context.Rigidbody.angularVelocity) < 0.1f) {
            Context.Rigidbody.angularVelocity = 0f;
        }
    }

    public override void Exit() {
        // Зберігаємо поточну швидкість для плавного переходу
        Context.CurrentSpeed = Context.Rigidbody.linearVelocity.magnitude;
    }
}

public class MovingToTargetState : State<Spaceship2D> {
    private float smoothRotationVelocity;

    public override void Enter() {
        InitializeSpeed();
        CacheRotationVelocity();
    }

    private void InitializeSpeed() {
        if (Context.CurrentSpeed < 0.1f) {
            Context.CurrentSpeed = 0f;
        }
    }

    private void CacheRotationVelocity() {
        smoothRotationVelocity = Context.Rigidbody.angularVelocity;
    }


    public override void Update() {
        if (HasArrived()) {
            StopMovement();
            return;
        }

        Vector2 direction = GetDirectionToTarget();
        float angleDifference = GetAngleDifference(direction);

        RotateTowards(direction);
        float targetSpeed = CalculateTargetSpeed(angleDifference);

        UpdateCurrentSpeed(targetSpeed);
        ApplyMovement();
    }

    private bool HasArrived() {
        return DistanceToTarget() < Context.ArrivalDistance;
    }

    private void StopMovement() {
        Context.Stop();
    }

    private float DistanceToTarget() {
        return Vector2.Distance(Context.transform.position, Context.TargetPosition);
    }

    private Vector2 GetDirectionToTarget() {
        return ((Vector2)Context.TargetPosition - (Vector2)Context.transform.position).normalized;
    }

    private float GetAngleDifference(Vector2 direction) {
        float targetAngle = DirectionToAngle(direction);
        float currentAngle = Context.transform.eulerAngles.z;
        return Mathf.DeltaAngle(currentAngle, targetAngle);
    }

    private float DirectionToAngle(Vector2 direction) {
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
    }

    private void RotateTowards(Vector2 direction) {
        float targetAngle = DirectionToAngle(direction);
        float currentAngle = Context.transform.eulerAngles.z;

        float newAngle = Mathf.SmoothDampAngle(
            currentAngle,
            targetAngle,
            ref smoothRotationVelocity,
            0.1f,
            Context.RotationSpeed
        );

        Context.transform.rotation = Quaternion.Euler(0f, 0f, newAngle);
        Context.TargetRotationVelocity = smoothRotationVelocity;
    }

    private float CalculateTargetSpeed(float angleDifference) {
        float targetSpeed;

        if (IsAligned(angleDifference)) {
            targetSpeed = CalculateAlignedSpeed(angleDifference);
        } else {
            targetSpeed = Context.MaxSpeed * 0.2f;
        }

        return targetSpeed;
    }

    private bool IsAligned(float angleDifference) {
        return Mathf.Abs(angleDifference) < 30f;
    }

    private float CalculateAlignedSpeed(float angleDifference) {
        float speed = CalculateDistanceBasedSpeed();
        float alignmentFactor = 1f - Mathf.Abs(angleDifference) / 30f;
        return speed * Mathf.Lerp(0.3f, 1f, alignmentFactor);
    }

    private float CalculateDistanceBasedSpeed() {
        float distance = DistanceToTarget();
        float slowdownDistance = Context.ArrivalDistance * 4f;

        if (distance < slowdownDistance) {
            float slowdownFactor = Mathf.Clamp01(distance / slowdownDistance);
            return Context.MaxSpeed * slowdownFactor * 0.5f;
        }

        return Context.MaxSpeed;
    }

    private void UpdateCurrentSpeed(float targetSpeed) {
        float delta = Time.deltaTime;

        if (Context.CurrentSpeed < targetSpeed) {
            Context.CurrentSpeed = Mathf.Min(
                Context.CurrentSpeed + Context.Acceleration * delta,
                targetSpeed
            );
        } else {
            Context.CurrentSpeed = Mathf.Max(
                Context.CurrentSpeed - Context.Deceleration * delta,
                targetSpeed
            );
        }
    }

    private void ApplyMovement() {
        Vector2 forward = Context.transform.up;
        Vector2 desiredVelocity = forward * Context.CurrentSpeed;

        Vector2 velocityChange = desiredVelocity - Context.Rigidbody.linearVelocity;
        float maxChange = Context.Acceleration * Time.deltaTime * 2f;

        velocityChange = Vector2.ClampMagnitude(velocityChange, maxChange);

        Context.Rigidbody.AddForce(
            velocityChange * Context.Rigidbody.mass / Time.deltaTime
        );
    }


    public override void Exit() {
        CacheExitSpeed();
    }

    private void CacheExitSpeed() {
        Context.CurrentSpeed = Context.Rigidbody.linearVelocity.magnitude;
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

public class OrbitState : State<Spaceship2D> {
    private Vector3 orbitCenter;
    private float currentAngle;
    private bool isInitialized = false;

    private Vector2 integralError = Vector2.zero;
    private Vector2 previousError = Vector2.zero;
    private float smoothRotationVelocity = 0f;
    private float orbitSpeedMultiplier = 1f;

    public override void Enter() {
        orbitCenter = Context.TargetPosition;

        // Обчислити початковий кут на основі поточної позиції
        Vector2 toCenter = (Vector2)Context.transform.position - (Vector2)orbitCenter;
        currentAngle = Mathf.Atan2(toCenter.y, toCenter.x) * Mathf.Rad2Deg;

        isInitialized = true;

        // Скидання PID
        integralError = Vector2.zero;
        previousError = Vector2.zero;

        smoothRotationVelocity = Context.TargetRotationVelocity;

        // Адаптувати швидкість орбіти до поточної швидкості корабля
        float currentSpeed = Context.Rigidbody.linearVelocity.magnitude;
        if (currentSpeed > 0.1f) {
            orbitSpeedMultiplier = Mathf.Clamp(currentSpeed / Context.MaxSpeed, 0.5f, 1.5f);
        }
    }

    public override void Update() {
        if (!isInitialized) return;

        Vector2 currentPosition = Context.transform.position;
        Vector2 toCenter = (Vector2)orbitCenter - currentPosition;
        float currentDistance = toCenter.magnitude;

        float targetOrbitSpeed = Context.OrbitParamsConfig.orbitSpeed * orbitSpeedMultiplier;
        orbitSpeedMultiplier = Mathf.Lerp(orbitSpeedMultiplier, 1f, Time.deltaTime * 0.5f);

        currentAngle += targetOrbitSpeed * Time.deltaTime;
        if (currentAngle >= 360f) currentAngle -= 360f;
        if (currentAngle < 0f) currentAngle += 360f;

        float radians = currentAngle * Mathf.Deg2Rad;
        Vector2 desiredPosition = (Vector2)orbitCenter + new Vector2(
            Mathf.Cos(radians) * Context.OrbitParamsConfig.orbitRadius,
            Mathf.Sin(radians) * Context.OrbitParamsConfig.orbitRadius
        );

        Vector2 error = desiredPosition - currentPosition;

        float maxIntegral = 5f;
        integralError += error * Time.deltaTime;
        integralError = Vector2.ClampMagnitude(integralError, maxIntegral);

        Vector2 derivative = (error - previousError) / Time.deltaTime;
        Vector2 pidOutput = error * Context.OrbitParamsConfig.pGain + integralError * Context.OrbitParamsConfig.iGain + derivative * Context.OrbitParamsConfig.dGain;
        previousError = error;

        // Динамічне обмеження сили
        float distanceFromIdeal = error.magnitude;
        float maxForce = Context.Acceleration * Mathf.Lerp(1f, 2f, Mathf.Clamp01(distanceFromIdeal / Context.OrbitParamsConfig.orbitRadius));
        pidOutput = Vector2.ClampMagnitude(pidOutput, maxForce);

        // Застосування сили
        Context.Rigidbody.AddForce(pidOutput);

        // М'яке обмеження швидкості
        float currentSpeed = Context.Rigidbody.linearVelocity.magnitude;
        if (currentSpeed > Context.MaxSpeed) {
            float speedReduction = Mathf.Lerp(1f, 0.95f, (currentSpeed - Context.MaxSpeed) / Context.MaxSpeed);
            Context.Rigidbody.linearVelocity *= speedReduction;
        }

        // Плавний поворот по дотичній
        Vector2 tangentDirection = new Vector2(-Mathf.Sin(radians), Mathf.Cos(radians));
        float targetRotation = Mathf.Atan2(tangentDirection.y, tangentDirection.x) * Mathf.Rad2Deg - 90f;

        float currentRotation = Context.transform.eulerAngles.z;
        float newAngle = Mathf.SmoothDampAngle(
            currentRotation,
            targetRotation,
            ref smoothRotationVelocity,
            0.15f,
            Context.RotationSpeed
        );

        Context.transform.rotation = Quaternion.Euler(0f, 0f, newAngle);
        Context.TargetRotationVelocity = smoothRotationVelocity;
    }

    public override void Exit() {
        Context.CurrentSpeed = Context.Rigidbody.linearVelocity.magnitude;
        isInitialized = false;
    }
}