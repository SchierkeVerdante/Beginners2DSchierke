using System.Collections;
using UnityEngine;

public class PlayerCollision2D : ACharacterCollision2D
{
    [SerializeField]
    Collider2D collider2d;

    [SerializeField]
    Collider2D dashHitBox;

    [SerializeField]
    float dashKnockback = 10;

    [SerializeField]
    PlayerState pState;

    [SerializeField]
    ParticleSystem damageParticles;

    [SerializeField]
    int invCount;

    protected override void Start() {
        base.Start();
        collider2d = GetComponent<Collider2D>();
        pState = LDirectory2D.Instance.pState;
        pState.onDamage.AddListener(HandleDamageInvincibility);
        pState.onDamage.AddListener(HandleDamageParticles);
        pState.onDash.AddListener(HandleDashInvincibility);
    }

    protected override bool OnCollsionIsDamaged(GameObject other) {
        return other.layer == Layers.Enemy || other.layer == Layers.EnemyAbility;
    }

    public override void OnHit(GameObject gameObject, int damage = 1) {
        if (dashHitBox.isActiveAndEnabled) {
            if (TryGetComponent(out EnemyCollision2D enemy)) {
                enemy.OnHit(gameObject, pState.netMod.dashDamage.damage);
                enemy.Knockback(enemy.transform.position - transform.position, dashKnockback);
            }
        } else {
            pState.TakeDamage(damage);
            Knockback(transform.position - gameObject.transform.position, contactKnockbackForce);
        }    
    }

    protected override void OnCollisionEnter2D(Collision2D collision) {
        base.OnCollisionEnter2D(collision);

        if (collision.gameObject.layer == Layers.Pickup) {
            if (collision.gameObject.TryGetComponent(out OilPickup oil)) {
                pState.ObtainOil(oil);
            } else if (collision.gameObject.TryGetComponent(out ModulePickup module)) {
                pState.ModuleChoice(module);
            }
            Destroy(collision.gameObject);
        }
    }      
    
    protected void HandleDamageParticles() {
        damageParticles.Play();
    }
    
    protected void HandleDamageInvincibility() {
        LayerMask invMask = Layers.GetInvMask(InvulnerabilityType.All);
        StartCoroutine(Invincibility(pState.damageIframesDuration, invMask));
    }
    
    public void HandleDashInvincibility() {
        float duration = pState.sprintIframesDuration * pState.netMod.dashDurationMultipler;
        LayerMask invMask = Layers.EnemyAbilityMask;
        if (pState.netMod.dashDamage != null) {
            invMask = Layers.GetInvMask(pState.netMod.dashDamage.invulnerabilityType);
            StartCoroutine(DashHitBox(duration));
        }
        StartCoroutine(Invincibility(duration, invMask, false)); 
    }

    private IEnumerator DashHitBox(float duration) {
        dashHitBox.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        dashHitBox.gameObject.SetActive(false);
    }

    private IEnumerator Invincibility(float duration, LayerMask ignoreMask, bool flash = true) {
        invCount++;
        collider2d.excludeLayers |= ignoreMask;

        float iTime = duration;
        if (flash) {
            int flashes = (int) (iTime * pState.flashesPerSecond);
            WaitForSeconds timePerFlash = new WaitForSeconds(iTime / (flashes * 2));

            for (int i = 0; i < flashes; i++) {
                spriteRenderer.material.SetFloat(hitEffectAmount, 1f);
                yield return timePerFlash;
                spriteRenderer.material.SetFloat(hitEffectAmount, 0f);
                yield return timePerFlash;
            }
        } else {
            yield return new WaitForSeconds(iTime);
        }
        invCount--;

        if (invCount <= 0) collider2d.excludeLayers = 0;
    }    

}
