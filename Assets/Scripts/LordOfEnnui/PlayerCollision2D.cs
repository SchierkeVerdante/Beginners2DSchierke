using System.Collections;
using UnityEngine;

public class PlayerCollision2D : ACharacterCollision2D
{
    [SerializeField]
    PlayerState pState;

    [SerializeField]
    ParticleSystem damageParticles;

    protected override void Start() {
        base.Start();
        pState = LDirectory2D.Instance.pState;
        pState.onDamage.AddListener(HandleDamageInvincibility);
        pState.onDamage.AddListener(HandleDamageParticles);
    }

    protected override bool OnCollsionIsDamaged(GameObject other) {
        return other.layer == Layers.Enemy || other.layer == Layers.EnemyAbility;
    }

    protected override void OnHit() {
        pState.TakeDamage(1.0f);
    }

    protected override void OnCollisionEnter2D(Collision2D collision) {
        base.OnCollisionEnter2D(collision);

        if (collision.gameObject.layer == Layers.Pickup) {
            OilPickup oil;
            if (collision.gameObject.TryGetComponent<OilPickup>(out oil)) {
                pState.currentOil += oil.oilAmount;
            }
            Destroy(collision.gameObject);
        }
    }      
    
    protected void HandleDamageParticles() {
        damageParticles.Play();
    }
    
    protected void HandleDamageInvincibility() {
        StartCoroutine(Invincibility((int) pState.damageIframes, new[] { Layers.Enemy, Layers.EnemyAbility }));
    }
    
    public void HandleDashInvincibility() {
        StartCoroutine(Invincibility((int) pState.sprintIframes, new[] { Layers.EnemyAbility }, false));
    }

    private IEnumerator Invincibility(int frames, int[] ignoreLayers, bool flash = true) {
        foreach (int layer in ignoreLayers) {
            Physics2D.IgnoreLayerCollision(Layers.Player, layer, true);
        }

        float iTime = frames / 60f;
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

        foreach (int layer in ignoreLayers) {
            Physics2D.IgnoreLayerCollision(Layers.Player, layer, false);
        }
    }    

}
