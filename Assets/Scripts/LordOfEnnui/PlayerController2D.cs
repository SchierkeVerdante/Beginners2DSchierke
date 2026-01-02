using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerInputStrategy))]
public class PlayerController2D : MonoBehaviour {
    Rigidbody2D body;
    PlayerInputStrategy moveStrat;
    PlayerCollision2D status;

    [SerializeField]
    float maxSpeed = 5, maxAcceleration = 20, 
        sprintSpeed = 20f, sprintDuration = 1f, sprintCooldown = 0.1f, sprintResetCooldown = 2f;
    [SerializeField]
    int maxConsecutiveSprints = 2;


    [Header("Out (ReadOnly)")]

    public Vector2 playerInput;
    public Vector3 targetVelocity, velocity, trueAcceleration, sprintDirection;
    public bool sprintInput;
    [SerializeField]
    int sprintPhase;
    [SerializeField]
    float timeSinceLastSprint = 0f, sprintResetTimer = 0f;

    private void Start() {
        body = GetComponent<Rigidbody2D>();
        moveStrat = GetComponent<PlayerInputStrategy>();
        TryGetComponent<PlayerCollision2D>(out status);
    }

    void Update() {
        sprintInput = moveStrat.SprintThisFrame();
        targetVelocity = moveStrat.MoveDirection() * (maxSpeed * moveStrat.MoveSpeedMultiplier());
        timeSinceLastSprint += Time.deltaTime;
        if (sprintPhase > 0) {
            sprintResetTimer += Time.deltaTime;
        }        
        if (sprintResetTimer > sprintResetCooldown) {
            sprintPhase = 0;
            sprintResetTimer = 0;
        } 
    }

    private void FixedUpdate() {
        if (!moveStrat.canMove) {
            body.linearVelocity = Vector3.zero;
            return;
        }
        velocity = body.linearVelocity;         
        Vector3 prevVelocity = velocity;
        float acceleration = maxAcceleration;

        if (sprintInput && moveStrat.sprintActive && moveStrat.canSprint) {
            sprintPhase++;
            sprintResetTimer = 0;
            timeSinceLastSprint = 0;
            moveStrat.isSprinting = true;
            sprintDirection = targetVelocity.normalized;
            if (status != null) status.HandleDashInvincibility();
        }        

        if (moveStrat.isSprinting && timeSinceLastSprint > sprintDuration) {
            moveStrat.isSprinting = false;
            acceleration = maxAcceleration * sprintSpeed;
        }        
        
        if (moveStrat.isSprinting) {
            float alignedSpeed = Vector3.Dot(velocity, sprintDirection);
            float adjustedSprintSpeed = sprintSpeed;
            if (alignedSpeed > 0f)
                adjustedSprintSpeed = Mathf.Max(adjustedSprintSpeed - alignedSpeed, 0f);
            velocity += sprintDirection * adjustedSprintSpeed;
            targetVelocity = velocity;
        } else {
            float maxSpeedChange = acceleration * Time.fixedDeltaTime;

            velocity.x =
                Mathf.MoveTowards(velocity.x, targetVelocity.x, maxSpeedChange);
            velocity.y =
                Mathf.MoveTowards(velocity.y, targetVelocity.y, maxSpeedChange);
        }

        trueAcceleration = (velocity - prevVelocity) / Time.fixedDeltaTime;
        trueAcceleration = velocity.magnitude > 0.1f && trueAcceleration.magnitude < 0.1f ? acceleration * velocity.normalized : trueAcceleration;

        body.linearVelocity = velocity;
        moveStrat.sprintActive = (sprintPhase < maxConsecutiveSprints || maxConsecutiveSprints < 0) && (timeSinceLastSprint > sprintCooldown || sprintCooldown < 0) && !moveStrat.isSprinting;
    }
}
