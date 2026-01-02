using System;
using System.Collections;
using UnityEngine;

public class EffectsManager : MonoBehaviour
{
    [SerializeField]
    PlayerState pState;

    [SerializeField]
    Shaker screenShaker;

    [SerializeField]
    ShakeParams damageShake;

    private void Awake() {
        pState = LDirectory2D.Instance.pState;
        if (screenShaker == null) screenShaker = LDirectory2D.Instance.screenShaker;
        pState.onDamage.AddListener(HandleDamageEffects);
    }

    public void HandleDamageEffects() {
        StartCoroutine(TimeStop(pState.damageHitStopTimeScale, pState.damageHitStopDuration, () => screenShaker.ShakeObject(damageShake)));
    }

    private IEnumerator TimeStop(float timeScale, float duration, Action Continuation = null) {
        Time.timeScale = timeScale;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1.0f;

        Continuation?.Invoke();
    }
}
