using System.Collections;
using UnityEngine;

public class PlayerStatus2D : ACharacterStatus2D
{
    float damageIframes = 60, sprintIframes = 30, flashesPerSecond = 2;

    protected override bool OnCollsionIsDamaged(GameObject other) {
        return other.layer == Layers.Enemy || other.layer == Layers.EnemyAbility;
    }

    protected override void OnDamageTaken() {
        StartCoroutine(Invincibility((int) damageIframes, new[] { Layers.Enemy, Layers.EnemyAbility }));
    }

    public void HandleDashInvincibility() {
        StartCoroutine(Invincibility((int) sprintIframes, new[] { Layers.EnemyAbility }));
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
