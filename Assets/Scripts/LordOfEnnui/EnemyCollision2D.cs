using DG.Tweening;
using System.Collections;
using UnityEngine;

public class EnemyCollision2D : ACharacterCollision2D
{
    [SerializeField]
    AEnemyStrategy strat;

    [SerializeField]
    protected int maxHealth = 5, currentHealth = 4;

    [SerializeField]
    LevelState lState;

    public Collider2D collider2d;

    private void Awake() {
        lState = LDirectory2D.Instance.lState;
        if (collider2d == null) collider2d = GetComponent<Collider2D>();
        if (strat  == null) strat = GetComponent<AEnemyStrategy>();
    }

    protected override bool OnCollsionIsDamaged(GameObject other) {
        return other.layer == Layers.PlayerAbility;
    }

    public override void OnHit(GameObject other, int damage = 1) {
        base.OnHit(other, damage);
        currentHealth = currentHealth > maxHealth ? maxHealth : currentHealth;
        currentHealth -= damage;
        damageParticles.transform.localEulerAngles = new Vector3(0, 0, (transform.position - other.transform.position).Get2DAngle());
        lState.EnemyHit(transform);
        StartCoroutine(DamageFlash());
        HandleDamageParticles();
        if (currentHealth < 0) {
            strat.OnDeath();
            collider2d.enabled = false;
            spriteRenderer.DOFade(0f, 0.3f).SetLink(gameObject).OnComplete(() => Destroy(gameObject));
        }
    }

    protected IEnumerator DamageFlash() {
        spriteRenderer.material.SetFloat(hitEffectAmount, 1f);
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.material.SetFloat(hitEffectAmount, 0f);
    }
}
