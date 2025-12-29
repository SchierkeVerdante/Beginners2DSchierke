using System;
using System.Collections;
using UnityEngine;

public class AMove2D : MonoBehaviour {

    [SerializeField]
    protected float
        duration = 5, startupTime = 1, endTime = 2;

    [SerializeField]
    protected MoveOverride[] moveOverrides;

    [Header("ReadOnly")]
    public float turnSpeed, moveSpeed;

    protected virtual void Start() {
        foreach (MoveOverride mOverride in moveOverrides) {
            StartCoroutine(OverrideValue(mOverride));
        }
    }

    protected virtual IEnumerator OverrideValue(MoveOverride mOverride) {
        float prevValue;
        Action<float> set;
        Action reset;
        switch (mOverride.type) {
            case OverrideType.TurnSpeed:
                prevValue = turnSpeed;
                set = speed => turnSpeed = speed;
                reset = () => turnSpeed = prevValue;
                break;
            case OverrideType.MovementSpeed:
                prevValue = moveSpeed;
                set = speed => moveSpeed = speed;
                reset = () => moveSpeed = prevValue;
                break;
            default:
                prevValue = 0;
                set = speed => prevValue = 0;
                reset = () => prevValue = 0;
                break;
        }

        switch (mOverride.duration) {
            case OverrideDuration.Startup:
                set(mOverride.value);
                yield return new WaitForSeconds(startupTime);
                reset();
                break;

            case OverrideDuration.Move:
                yield return new WaitForSeconds(startupTime);
                set(mOverride.value);
                yield return new WaitForSeconds(duration);
                reset();
                break;

            case OverrideDuration.End:
                yield return new WaitForSeconds(startupTime);
                yield return new WaitForSeconds(duration);
                set(mOverride.value);
                yield return new WaitForSeconds(endTime);
                reset();
                break;
        }
    }
}
