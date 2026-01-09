using System;
using System.Collections;
using UnityEngine;

public class EffectsManager : MonoBehaviour
{
    [SerializeField]
    PlayerState pState;

    [SerializeField]
    LevelState lState;

    [SerializeField]
    Shaker screenShaker;

    [SerializeField]
    ShakeParams damageShake, fireShake;

    private void Awake() {
        pState = LDirectory2D.Instance.pState;
        lState = LDirectory2D.Instance.lState;
        if (screenShaker == null) screenShaker = LDirectory2D.Instance.screenShaker;
        pState.onDamage.AddListener(HandleDamageEffects);
        pState.onFire.AddListener(HandleFireEffects);
    }

    public void HandleDamageEffects() {
        StartCoroutine(TimeStop(pState.damageHitStopTimeScale, pState.damageHitStopDuration, () => screenShaker.ShakeObject(damageShake)));
    }

    public void HandleFireEffects() {
        screenShaker.ShakeObject(fireShake);
    }

    private IEnumerator TimeStop(float timeScale, float duration, Action Continuation = null) {
        Time.timeScale = timeScale;

        yield return new WaitForSecondsRealtime(duration);

        if (!lState.uiActive) Time.timeScale = 1.0f;

        Continuation?.Invoke();
    }
}
