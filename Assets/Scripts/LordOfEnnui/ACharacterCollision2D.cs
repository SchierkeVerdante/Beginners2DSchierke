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
                rb.AddForce(collision.GetContact(0).normal * contactKnockbackForce, ForceMode2D.Impulse);
                OnHit();
                hitThisFrame |= true;
            }
        }
    }

    protected abstract bool OnCollsionIsDamaged(GameObject other);

    protected abstract void OnHit();
}
