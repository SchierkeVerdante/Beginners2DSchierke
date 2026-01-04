using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class ACharacterCollision2D : MonoBehaviour {

    protected static int hitEffectAmount = Shader.PropertyToID("_HitEffectAmount");

    [SerializeField]
    protected float contactKnockbackForce = 10;

    [SerializeField]
    protected bool hitThisFrame;

    [SerializeField]
    protected Rigidbody2D rb;

    [SerializeField]
    protected SpriteRenderer spriteRenderer;

    private void FixedUpdate() {
        hitThisFrame = false;
    }

    protected virtual void Start() {
        rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision) {
        if (OnCollsionIsDamaged(collision.gameObject)) {
            if (!hitThisFrame) {
                OnHit(collision.gameObject);
                hitThisFrame |= true;
            }
        }
    }

    protected abstract bool OnCollsionIsDamaged(GameObject other);

    public abstract void OnHit(GameObject gameObject, int damage = 1);

    public virtual void Knockback(Vector3 direction, float force) {
        rb.AddForce(direction.normalized * force * rb.mass, ForceMode2D.Impulse);
    }
}
