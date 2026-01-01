using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator2D : MonoBehaviour
{
    [SerializeField]
    Animator animator;

    [SerializeField]
    PlayerController2D controller;

    [SerializeField]
    int verticalFieldDegrees = 90;

    [Header("Particles")]
    [SerializeField]
    ParticleSystem[] thrustParticles;

    [SerializeField]
    float particlesPerSecond = 200f, sprintParticles = 1000f;

    [SerializeField, Header("ReadOnly")]
    bool facingRight = true;

    [SerializeField]
    bool lookLeft, lookUp, lookDown, lookRight, moving;

    [SerializeField]
    int state;

    private int
        idleState = Animator.StringToHash("Idle"),
        movingHorState = Animator.StringToHash("MoveRight"),
        movingUpState = Animator.StringToHash("MoveUp"),
        movingDownState = Animator.StringToHash("MoveDown");

    void Start()
    {
        animator = GetComponent<Animator>();    
        controller = gameObject.GetComponentInParent<PlayerController2D>();
    }

    void FixedUpdate()
    {
        Vector2 playerInput = controller.playerInput;
        float magnitude = controller.playerInput.magnitude;
        float angle = Vector2.SignedAngle(Vector2.right, playerInput);
        angle = angle < 0 ? 360 + angle : angle;

        animator.SetFloat("speed", magnitude);

        lookLeft = angle < 270 - verticalFieldDegrees / 2 && angle > 90 + verticalFieldDegrees / 2;
        lookRight = angle < 90 - verticalFieldDegrees / 2 || angle > 270 + verticalFieldDegrees / 2;
        lookUp = angle < 90 + verticalFieldDegrees / 2 && angle > 90 - verticalFieldDegrees / 2;
        lookDown = angle < 270 + verticalFieldDegrees / 2 && angle > 270 - verticalFieldDegrees / 2;
        moving = magnitude > float.Epsilon;

        if (moving) {
            if (facingRight && lookLeft) {
                facingRight = false;
                Flip();
            }

            if (!facingRight && (lookRight || lookUp || lookDown)) {
                facingRight = true;
                Flip();
            }        
            
            state = lookRight ? 1 : lookUp ? 2 : lookLeft ? 3 : lookDown ? 4 : -1;

            int particles = Mathf.FloorToInt(controller.isSprinting? sprintParticles : particlesPerSecond * Time.fixedDeltaTime);
            thrustParticles[0].Emit(lookLeft? particles : 0);
            thrustParticles[1].Emit(lookDown ? particles : 0);
            thrustParticles[2].Emit(lookRight ? particles : 0);
            thrustParticles[3].Emit(lookUp ? particles : 0);

        } else {
            state = 0;
        }

        animator.SetInteger("state", state);

        animator.SetBool("isMoving", moving && (lookRight || lookLeft));
        animator.SetBool("isUpMoving", moving && lookUp);
        animator.SetBool("isDownMoving", moving && lookDown);
    }

    void Flip() {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
