using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerStatus2D))]
public class PlayerController2D : MonoBehaviour {
    InputAction moveAction, sprintAction;
    Rigidbody2D body;

    [SerializeField]
    Transform playerInputSpace = default;

    [SerializeField]
    float maxSpeed = 5, maxAcceleration = 20, 
        sprintSpeed = 20f, sprintDuration = 1f, sprintCooldown = 0.1f, sprintResetCooldown = 2f;
    [SerializeField]
    int maxConsecutiveSprints = 2;
    [SerializeField]
    public bool canMove = true;


    [Header("Out (ReadOnly)")]

    public Vector2 playerInput;
    public Vector3 targetVelocity, velocity, trueAcceleration, sprintDirection;
    public bool sprintInput, isSprinting;
    [SerializeField]
    int sprintPhase;
    [SerializeField]
    float timeSinceLastSprint = 0f, sprintResetTimer = 0f;

    private void Start() {
        // Find the references to the "Move" and "Jump" actions
        moveAction = InputSystem.actions.FindAction("Move");
        sprintAction = InputSystem.actions.FindAction("Sprint");
        body = GetComponent<Rigidbody2D>();
        playerInputSpace = transform;
    }

    void Update() {
        playerInput = moveAction.ReadValue<Vector2>();
        sprintInput |= sprintAction.WasPressedThisFrame();
        Vector3 up = playerInputSpace.up;
        up.z = 0f;
        up.Normalize();
        Vector3 right = playerInputSpace.right;
        right.z = 0f;
        right.Normalize();
        targetVelocity =
            (up * playerInput.y + right * playerInput.x) * maxSpeed;
        timeSinceLastSprint += Time.deltaTime;
        if (sprintPhase > 0) {
            sprintResetTimer += Time.deltaTime;
        }
    }

    private void FixedUpdate() {
        if (!canMove) {
            body.linearVelocity = Vector3.zero;
            return;
        }
        velocity = body.linearVelocity;            
        Vector3 prevVelocity = velocity;
        float acceleration = maxAcceleration;

        if (sprintResetTimer > sprintResetCooldown) {
            sprintPhase = 0;
            sprintResetTimer = 0;
        }

        if (sprintInput) {
            sprintInput = false;
            if ((sprintPhase < maxConsecutiveSprints || maxConsecutiveSprints < 0) && timeSinceLastSprint > sprintCooldown && !isSprinting) {
                sprintPhase++;
                sprintResetTimer = 0;
                timeSinceLastSprint = 0;
                isSprinting = true;
                sprintDirection = targetVelocity.normalized;
                GetComponent<PlayerStatus2D>().HandleDashInvincibility();
            }
        }

        if (isSprinting && timeSinceLastSprint > sprintDuration) {
            isSprinting = false;
            acceleration = maxAcceleration * sprintSpeed;
        }

        if (isSprinting) {
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
    }
}
