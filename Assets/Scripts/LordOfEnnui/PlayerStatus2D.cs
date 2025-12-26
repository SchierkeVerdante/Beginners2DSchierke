using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerStatus2D : MonoBehaviour
{
    [SerializeField]
    int maxHealth = 5, currentHealth = 4;
    [SerializeField]
    float contactKnockbackForce = 10;

    [SerializeField]
    Rigidbody2D rb;
    [SerializeField]
    SpriteRenderer spriteRenderer;
    [SerializeField]
    float damageIframes = 60, sprintIframes = 30, flashesPerSecond = 2;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void HandleDashInvincibility() {
        StartCoroutine(Invincibility((int) sprintIframes, new[] { Layers.EnemyAbility }));
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.layer == Layers.Enemy || collision.gameObject.layer == Layers.EnemyAbility) {
            rb.AddForce(collision.GetContact(0).normal * contactKnockbackForce, ForceMode2D.Impulse);
            currentHealth = currentHealth > maxHealth ? maxHealth : currentHealth;
            currentHealth--;
            if (currentHealth <= 0) {
                Debug.Log("ded");
                gameObject.SetActive(false);
                return;
            }
            StartCoroutine(Invincibility((int) damageIframes, new[] { Layers.Enemy, Layers.EnemyAbility }));
        }
    }

    private IEnumerator Invincibility(int frames, int[] ignoreLayers, bool flash = true) {
        foreach (int layer in ignoreLayers) {
            Physics2D.IgnoreLayerCollision(Layers.Player, layer, true);
        }

        float iTime = frames / 60f;
        if (flash) {
            int flashes = (int) (iTime * flashesPerSecond);
            WaitForSeconds timePerFlash = new WaitForSeconds(iTime / (flashes * 2));

            for (int i = 0; i < flashes; i++) {
                spriteRenderer.color = new Color(1, 1, 1, 0.5f);
                yield return timePerFlash;
                spriteRenderer.color = Color.white;
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
