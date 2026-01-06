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

    [SerializeField]
    protected ParticleSystem damageParticles;

    private void FixedUpdate() {
        hitThisFrame = false;
    }

    protected virtual void Start() {
        rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision) {
        HandleCollision(collision.collider);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision) {
        HandleCollision(collision);
    }

    protected virtual void HandleCollision(Collider2D collision) {
        if (OnCollsionIsDamaged(collision.gameObject)) {
            if (!hitThisFrame) {
                OnHit(collision.gameObject);
            }
        }
    }

    protected abstract bool OnCollsionIsDamaged(GameObject other);

    public virtual void OnHit(GameObject gameObject, int damage = 1) {
        hitThisFrame |= true;
    }

    public virtual void Knockback(Vector3 direction, float force) {
        rb.AddForce(force * rb.mass * direction.normalized, ForceMode2D.Impulse);
    }

    protected virtual void HandleDamageParticles() {
        if (damageParticles != null) damageParticles.Play();
    }
}
