using UnityEngine;

public abstract class ASpriteAnimator2D : MonoBehaviour {
    [SerializeField]
    protected Animator animator;

    [SerializeField]
    protected SpriteRenderer spriteRenderer;

    [SerializeField]
    protected int verticalFieldDegrees = 90;

    [SerializeField, Header("ReadOnly")]
    protected bool facingRight = true;

    [SerializeField]
    protected bool lookLeft, lookUp, lookDown, lookRight, moving;

    [SerializeField]
    protected int state;

    protected int
        idleState = Animator.StringToHash("Idle"),
        movingHorState = Animator.StringToHash("MoveRight"),
        movingUpState = Animator.StringToHash("MoveUp"),
        movingDownState = Animator.StringToHash("MoveDown"),
        speedParam = Animator.StringToHash("speed"),
        stateParam = Animator.StringToHash("state"),
        movingParam = Animator.StringToHash("isMoving"),
        movingUpParam = Animator.StringToHash("isMovingUp"),
        movingDownParam = Animator.StringToHash("isMovingDown");

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected virtual void SetAnimatorValues(float angle, float speed) {
        animator.SetFloat(speedParam, speed);

        lookLeft = angle < 270 - verticalFieldDegrees / 2 && angle > 90 + verticalFieldDegrees / 2;
        lookRight = angle < 90 - verticalFieldDegrees / 2 || angle > 270 + verticalFieldDegrees / 2;
        lookUp = angle <= 90 + verticalFieldDegrees / 2 && angle >= 90 - verticalFieldDegrees / 2;
        lookDown = angle <= 270 + verticalFieldDegrees / 2 && angle >= 270 - verticalFieldDegrees / 2;
        moving = speed > float.Epsilon;

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
        } else {
            state = 0;
        }

        animator.SetInteger(stateParam, state);

        animator.SetBool(movingParam, moving && (lookRight || lookLeft));
        animator.SetBool(movingUpParam, moving && lookUp);
        animator.SetBool(movingDownParam, moving && lookDown);
    }

    void Flip() {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
