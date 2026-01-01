using UnityEngine;
// ==================== STATES ====================

public class IdleSpaceShipState : State<Spaceship2D> {
    private float decelerationTime = 0f;

    public override void Enter() {
        decelerationTime = 0f;
    }

    public override void Update() {
        decelerationTime += Time.deltaTime;
        float decelerationFactor = Mathf.Exp(-3f * decelerationTime);

        Context.ApplyDeceleration(decelerationFactor);
    }

    public override void Exit() {
        Context.CacheCurrentSpeed();
    }
}
