using System.Collections;
using UnityEngine;

public abstract class ABullet2D : MonoBehaviour
{
    [SerializeField]
    public float lifetime = 1f, disableColliderAtStart = 0f;

    [SerializeField]
    public float health = 1f, damage = 1f;

    [SerializeField]
    public bool playerBullet = false;

    [SerializeField]
    ParticleSystem deathSplash;

    [SerializeField]
    ParticleSystem projectileSystem;

    [SerializeField]
    Collider2D collider2d;

    [SerializeField]
    Rigidbody2D rb;

    [Header("Tracking")]
    [SerializeField]
    bool targeting = false;
    [SerializeField]
    GameObject target;
    [SerializeField, Range(0, 1f)]
    float targetingAmount = 0.1f;
    PlayerInputStrategy pStrat;

    [Header("Explosions")]
    [SerializeField]
    Collider2D explosionCollider;
    SpriteRenderer explosionSprite;

    [SerializeField]
    ShakeParams explosionShake;

    [Header("Child Bullets")]
    [SerializeField]
    ABullet2D childBullet;
    float facingDirection;

    [SerializeField]
    float startSpeed = 10f;

    [SerializeField]
    float[] angleOffsets = new[] { 45f, 135f, 225f, 315f };

    void Awake()
    {
        if (projectileSystem == null) projectileSystem = GetComponent<ParticleSystem>();
        if (collider2d == null) collider2d = GetComponent<Collider2D>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (!playerBullet && targeting) target = LDirectory2D.Instance.player;
        if (playerBullet  && targeting) pStrat = LDirectory2D.Instance.playerInputStrategy;
        StartCoroutine(DestroyAfter(lifetime));
        collider2d.enabled = false;
        StartCoroutine(SetCollider(collider2d, true, disableColliderAtStart));
    }

    private void FixedUpdate() {
        if (targeting) {
            float initialSpeed = rb.linearVelocity.magnitude;
            if (playerBullet) target = pStrat.closestEnemy.gameObject;
            if (target != null) rb.linearVelocity = Vector2.Lerp(rb.linearVelocity.normalized, (Vector2) (target.transform.position - transform.position).normalized, targetingAmount) * initialSpeed;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.TryGetComponent(out ABullet2D bullet)) {
            health -= bullet.damage;
            if (health > 0f) return;
        }
        OnDeath();
    }

    protected virtual void OnDeath() {
        facingDirection = rb.linearVelocity.Get2DAngle();
        rb.linearVelocity = Vector3.zero;
        if (deathSplash != null) {
            deathSplash.transform.SetParent(null, true);
            deathSplash.transform.position = transform.position;
            deathSplash.Play();
            Destroy(deathSplash.gameObject, 0.5f);
        }
        if (explosionShake != null) LDirectory2D.Instance.screenShaker.ShakeObject(explosionShake);
        collider2d.enabled = false;
        projectileSystem.Stop();
        Destroy(gameObject, 0.1f);
        LDirectory2D.Instance.lState.onBulletHit.Invoke();

        if (childBullet != null) SpawnChildBullets();

        if (explosionCollider != null) {
            explosionCollider.enabled = true;
        }
        StartCoroutine(SetCollider(explosionCollider, false, 0.1f));
    }

    private void SpawnChildBullets() {
        foreach (float angleOffset in angleOffsets) {
            Vector3 shootDirection = Quaternion.AngleAxis(facingDirection + angleOffset, transform.forward) * transform.right;
            Vector3 placePosition = transform.position + shootDirection * 0.5f;

            ABullet2D bo = Instantiate(childBullet, placePosition, Quaternion.identity, transform);
            bo.transform.SetParent(null, true);
            bo.gameObject.SetActive(true);
            bo.GetComponent<Rigidbody2D>().linearVelocity = (Vector2) shootDirection * startSpeed;
        }
    }

    private IEnumerator DestroyAfter(float lifetime) {
        float lifeTimeTimer = lifetime;
        float blinkRate = 0.25f;
        if (explosionCollider != null) explosionSprite = explosionCollider.GetComponent<SpriteRenderer>();
        if (explosionSprite != null) {
            while (lifeTimeTimer > 0) {
                explosionSprite.enabled = !explosionSprite.enabled;
                float wait = Mathf.Max(lifeTimeTimer * blinkRate, 0.25f);
                yield return new WaitForSeconds(wait);
                lifeTimeTimer -= wait;
            }
            OnDeath();
        } else {
            yield return new WaitForSeconds(lifetime);
            OnDeath();
        }
    }

    private IEnumerator SetCollider(Collider2D collider, bool active, float duration) {
        yield return new WaitForSeconds(duration);
        if (collider != null) {
            collider.enabled = active;
        }
    }


}
