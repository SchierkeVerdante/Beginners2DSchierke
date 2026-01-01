using UnityEngine;

public class OrbitState : State<Spaceship2D> {
    private Vector3 orbitCenter;
    private float currentAngle;
    private bool isInitialized = false;

    // PID контролер
    private Vector2 integralError = Vector2.zero;
    private Vector2 previousError = Vector2.zero;

    private float orbitSpeedMultiplier = 1f;

    public override void Enter() {
        orbitCenter = Context.TargetPosition;
        InitializeAngle();
        ResetPidController();
        CacheInitialVelocity();
        isInitialized = true;
    }

    private void InitializeAngle() {
        Vector2 toCenter = (Vector2)Context.transform.position - (Vector2)orbitCenter;
        currentAngle = Mathf.Atan2(toCenter.y, toCenter.x) * Mathf.Rad2Deg;
    }

    private void ResetPidController() {
        integralError = Vector2.zero;
        previousError = Vector2.zero;
    }

    private void CacheInitialVelocity() {
        float speed = Context.GetCurrentVelocityMagnitude();
        orbitSpeedMultiplier = speed > 0.1f
            ? Mathf.Clamp(speed / Context.MaxSpeed, 0.5f, 1.5f)
            : 1f;
    }

    public override void Update() {
        if (!isInitialized) return;

        UpdateOrbitAngle();

        Vector2 desiredPosition = Context.CalculateOrbitPosition(
            orbitCenter,
            currentAngle,
            Context.OrbitConfig.orbitRadius
        );

        Vector2 error = desiredPosition - (Vector2)Context.transform.position;
        Vector2 pidForce = CalculatePidForce(error);

        Context.ApplyForce(pidForce);
        Context.ClampVelocity(Context.MaxSpeed);

        float targetRotation = Context.CalculateOrbitTangentAngle(currentAngle);
        Context.RotateToAngle(targetRotation, 0.15f);
    }

    private void UpdateOrbitAngle() {
        float targetOrbitSpeed = Context.OrbitConfig.orbitSpeed * orbitSpeedMultiplier;
        orbitSpeedMultiplier = Mathf.Lerp(orbitSpeedMultiplier, 1f, Time.deltaTime * 0.5f);

        currentAngle += targetOrbitSpeed * Time.deltaTime;

        if (currentAngle >= 360f) currentAngle -= 360f;
        if (currentAngle < 0f) currentAngle += 360f;
    }

    private Vector2 CalculatePidForce(Vector2 error) {
        // Integral
        integralError += error * Time.deltaTime;
        integralError = Vector2.ClampMagnitude(integralError, 5f);

        // Derivative
        Vector2 derivative = (error - previousError) / Time.deltaTime;
        previousError = error;

        // PID Output
        Vector2 output =
            error * Context.OrbitConfig.pGain +
            integralError * Context.OrbitConfig.iGain +
            derivative * Context.OrbitConfig.dGain;

        return ClampForce(output, error.magnitude);
    }

    private Vector2 ClampForce(Vector2 force, float distanceError) {
        float maxForce = Context.MaxSpeed *
            Mathf.Lerp(1f, 2f, Mathf.Clamp01(distanceError / Context.OrbitConfig.orbitRadius));

        return Vector2.ClampMagnitude(force, maxForce);
    }

    public override void Exit() {
        Context.CacheCurrentSpeed();
        isInitialized = false;
    }
}