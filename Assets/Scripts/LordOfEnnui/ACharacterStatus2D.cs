using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class ACharacterStatus2D : MonoBehaviour {
    [SerializeField]
    protected int maxHealth = 5, currentHealth = 4;

    [SerializeField]
    protected float contactKnockbackForce = 10;

    [SerializeField]
    protected Rigidbody2D rb;

    [SerializeField]
    protected SpriteRenderer spriteRenderer;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (OnCollsionIsDamaged(collision.gameObject)) {
            rb.AddForce(collision.GetContact(0).normal * contactKnockbackForce, ForceMode2D.Impulse);
            currentHealth = currentHealth > maxHealth ? maxHealth : currentHealth;
            currentHealth--;
            if (currentHealth <= 0) {
                OnDeath();
                return;
            }
            OnDamageTaken();
        }
    }

    protected abstract bool OnCollsionIsDamaged(GameObject other);

    protected abstract void OnDamageTaken();

    protected virtual void OnDeath() {
        Debug.Log("ded");
        gameObject.SetActive(false);
    }
}
