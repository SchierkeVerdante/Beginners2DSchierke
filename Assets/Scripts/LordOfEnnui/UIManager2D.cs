using UnityEngine;

public class UIManager2D : MonoBehaviour
{
    [SerializeField]
    UIBar oilBar;

    [SerializeField]
    PlayerState pState;

    private void Awake() {
        pState = LDirectory2D.Instance.pState;
    }

    private void Update() {
        oilBar.UpdateComponent(pState.currentOil, pState.maxOil);
    }
}
