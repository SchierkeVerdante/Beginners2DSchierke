using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator2D : MonoBehaviour
{
    [SerializeField]
    Animator animator;

    [SerializeField]
    PlayerInputStrategy strat;

    [SerializeField]
    int verticalFieldDegrees = 90;

    [Header("Particles")]
    [SerializeField]
    ParticleSystem thrustParticles, sprintParticles;
    ParticleSystemRenderer thrustRenderer, sprintRenderer;

    [SerializeField]
    float particlesPerSecond = 200f, sprintParticleCount = 1000f;

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
        strat = gameObject.GetComponentInParent<PlayerInputStrategy>();
        thrustRenderer = thrustParticles.gameObject.GetComponent<ParticleSystemRenderer>();
        sprintRenderer = sprintParticles.gameObject.GetComponent<ParticleSystemRenderer>();
    }

    void FixedUpdate()
    {
        Vector2 playerInput = strat.MoveDirection();
        float magnitude = playerInput.magnitude;
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

            int particles = Mathf.FloorToInt(strat.isSprinting? sprintParticleCount : particlesPerSecond * Time.fixedDeltaTime);
            ParticleSystem pSys = strat.isSprinting ? sprintParticles : thrustParticles;
            ParticleSystemRenderer pRend = strat.isSprinting ? thrustRenderer : sprintRenderer;
            float direction = lookLeft ? 0 : lookDown ? 90 : lookRight ? 180 : lookUp ? 270 : -30;
            pSys.transform.eulerAngles = new(0, 0, direction);
            pRend.sortingOrder = lookUp ? 100 : 0;
            pSys.Emit(particles);
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
