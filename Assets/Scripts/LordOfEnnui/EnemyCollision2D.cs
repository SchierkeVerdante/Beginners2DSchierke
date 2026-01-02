using System.Collections;
using UnityEngine;

public class EnemyCollision2D : ACharacterCollision2D
{
    [SerializeField]
    protected int maxHealth = 5, currentHealth = 4;

    protected override bool OnCollsionIsDamaged(GameObject other) {
        return other.layer == Layers.PlayerAbility;
    }

    protected override void OnHit() {
        currentHealth = currentHealth > maxHealth ? maxHealth : currentHealth;
        currentHealth--;
        StartCoroutine(DamageFlash());
        if (currentHealth < 0) {
            Destroy(gameObject);
        }
    }

    protected IEnumerator DamageFlash() {
        spriteRenderer.material.SetFloat(hitEffectAmount, 1f);
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.material.SetFloat(hitEffectAmount, 0f);
    }
}
