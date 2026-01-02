using UnityEngine;

public class GameManager2D : MonoBehaviour
{
    [SerializeField]
    Trigger2D levelExitTrigger;

    [SerializeField]
    PlayerState pState;

    private void Awake() {
        Application.targetFrameRate = -1;
    }

    void Start()
    {
        if (levelExitTrigger != null) levelExitTrigger.triggerEvent.AddListener(OnLevelComplete);
        pState = LDirectory2D.Instance.pState;
        pState.onDeath.AddListener(OnDeath);
    }

    void Update()
    {
        if (levelExitTrigger != null) levelExitTrigger.SetActive(pState.currentOil >= pState.requiredOil);
    }

    public void OnLevelComplete(GameObject player, bool entered) {
        if (entered) Debug.Log("Pog");
    }

    public void OnDeath() {
        Debug.Log("Ded");
    }
}
