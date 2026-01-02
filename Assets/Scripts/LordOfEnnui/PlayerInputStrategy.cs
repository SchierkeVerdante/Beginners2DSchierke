using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputStrategy : ACharacterStrategy {
    
    InputSystem_Actions inputActions;
    InputAction moveAction, lookAction, sprintAction, attackAction;
    
    [SerializeField, Range(0f, 0.5f)]
    float inputQueueTime = 0.1f;

    [SerializeField, Range(0f, 20f)]
    float maxAimAssist = 5f;
    [SerializeField, Range(0f, 20f)]
    float aimAssistRange = 20f;

    [SerializeField]
    Transform playerInputSpace = default;
    Vector3 right, up;

    [Header("ReadOnly")]
    [SerializeField]
    bool mouseUsed = false;
    [SerializeField]
    Vector2 moveInput, lookInput;
    [SerializeField]
    bool sprintInputQueued, attackInput;
    [SerializeField]
    Vector3 moveDirection, lookDirection;
    [SerializeField]
    float firingMoveSpeedMultiplier = 1.0f;
    [SerializeField]
    GameObject closestEnemy;

    [Header("State")]
    public bool sprintActive = true, canSprint = true, canAttack = true, canMove = true;
    public bool isSprinting = false, isAttacking = false, inputQueued = false;
    public float aimAngle;

    [SerializeField]
    float inputQueueTimer;

    private void Awake() {         
        if (playerInputSpace == null) playerInputSpace = transform;
        inputActions = new InputSystem_Actions();
        up = playerInputSpace.up; up.z = 0f; up.Normalize();
        right = playerInputSpace.right; right.z = 0f; right.Normalize();

        moveAction = InputSystem.actions.FindAction("Move");
        lookAction = InputSystem.actions.FindAction("Look");
        sprintAction = InputSystem.actions.FindAction("Sprint");
        attackAction = InputSystem.actions.FindAction("Attack");

        moveAction.performed += ctx => {
            moveInput = ctx.ReadValue<Vector2>();
            moveDirection = right * moveInput.x + up * moveInput.y;
        };
        moveAction.canceled += ctx => {
            moveInput = Vector2.zero;
            moveDirection = Vector3.zero;
        };

        lookAction.performed += ctx => {
            lookInput = ctx.ReadValue<Vector2>();
            mouseUsed = ctx.control.device is Mouse;
            if (!mouseUsed) {
                lookDirection = up * lookInput.x + up * lookInput.y;
            } else {
                lookDirection = (Camera.main.ScreenToWorldPoint(lookInput) - transform.position).normalized;
            }
            aimAngle = lookDirection.Get2DAngle();
        };

        attackAction.performed += ctx => attackInput = true;
        attackAction.canceled += ctx => attackInput = false;

        sprintAction.performed += ctx => {
            sprintInputQueued = true;
            inputQueued = true;
            inputQueueTimer = 0f;
        };
    }

    private void Update() {
        if (inputQueued) inputQueueTimer += Time.deltaTime;
    }

    public bool SprintThisFrame() {
        if (inputQueued) {
            if (inputQueueTimer > inputQueueTime) {
                sprintInputQueued = false;
                inputQueued = false;
            }
        }

        if (sprintInputQueued && sprintActive && canSprint) {
            inputQueued = false;
            sprintInputQueued = false;
            return true;
        }
        return false;
    }

    public override bool FireThisFrame(ABullet2D bullet, ShootParams shootParams) {
        firingMoveSpeedMultiplier = attackInput ? shootParams.moveSpeedMultiplier : 1.0f;
        return attackInput;
    }

    public override Vector3 AimDirection() {
        return lookDirection;
    }

    public override Vector3 MoveDirection() {
        return moveDirection;
    }

    public override float FireAngle() {
        closestEnemy = null;
        float closestDistance = float.MaxValue;
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position, aimAssistRange)) {
            if (closestEnemy == null || Vector3.Distance(transform.position, collider.transform.position) < closestDistance) {
                closestEnemy = collider.gameObject;
                closestDistance = Vector3.Distance(transform.position, closestEnemy.transform.position);
            }
        }
        float assistAngle = aimAngle;
        if (closestEnemy != null) (closestEnemy.transform.position - transform.position).Get2DAngle();

        return aimAngle.GetAngularDistance(assistAngle) < maxAimAssist ? assistAngle : aimAngle;
    }

    public float MoveSpeedMultiplier() {
        return firingMoveSpeedMultiplier;
    }
}
