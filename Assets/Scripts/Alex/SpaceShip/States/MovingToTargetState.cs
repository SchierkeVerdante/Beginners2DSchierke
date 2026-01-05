using UnityEngine;

public class MovingToTargetState : State<Spaceship2D> {
    public override void Enter() {
        if (Context.CurrentSpeed < 0.1f) {
            Context.CacheCurrentSpeed();
        }
    }

    public override void Update() {
        if (Context.HasReachedTarget()) {
            Context.Stop();
            Context.doOnTargetReached?.Invoke();
            return;
        }

        Vector2 direction = Context.GetDirectionTo(Context.TargetPosition);
        float distance = Context.GetDistanceTo(Context.TargetPosition);
        float angleDifference = Context.GetAngleDifference(Context.TargetPosition);

        Context.RotateTowards(direction, 0.1f);

        float targetSpeed = CalculateTargetSpeed(distance, angleDifference);
        Context.UpdateSpeed(targetSpeed, Time.deltaTime);
        Context.MoveForward();
    }

    private float CalculateTargetSpeed(float distance, float angleDifference) {
        if (Mathf.Abs(angleDifference) > 30f) {
            return Context.MaxSpeed * 0.2f;
        }

        float baseSpeed = CalculateDistanceBasedSpeed(distance);

        float alignmentFactor = 1f - Mathf.Abs(angleDifference) / 30f;
        return baseSpeed * Mathf.Lerp(0.3f, 1f, alignmentFactor);
    }

    private float CalculateDistanceBasedSpeed(float distance) {
        float slowdownDistance = Context.ArrivalDistance * 4f;

        if (distance < slowdownDistance) {
            float slowdownFactor = Mathf.Clamp01(distance / slowdownDistance);
            return Context.MaxSpeed * slowdownFactor * 0.5f;
        }

        return Context.MaxSpeed;
    }

    public override void Exit() {
        Context.CacheCurrentSpeed();
    }
}
