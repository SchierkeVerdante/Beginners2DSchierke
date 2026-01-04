using UnityEngine;

public abstract class APlayerAbility : MonoBehaviour {
    public abstract float GetDamageDealt();
}

public class DashDamageBox : APlayerAbility {
    [SerializeField]
    PlayerState pState;

    private void Awake() {
        pState = LDirectory2D.Instance.pState;
    }

    public override float GetDamageDealt() {
        return 1;
    }
}
