using UnityEngine;
using Zenject;

public class GameManager2D : MonoBehaviour {
    [SerializeField]
    Trigger2D levelExitTrigger;

    [SerializeField]
    PlayerState pState;

    LevelState lState;

    [Inject] private IGameManager gameManager;
    private void Awake() {
        Application.targetFrameRate = -1;
    }

    void Start() {
        if (levelExitTrigger != null) levelExitTrigger.triggerEvent.AddListener(OnLevelCompleteZoneEnter);
        gameManager = LDirectory2D.Instance.gameManager;
        pState = LDirectory2D.Instance.pState;
        lState = LDirectory2D.Instance.lState;
        pState.onSufficientOil.AddListener(SetLevelTriggerState);
    }

    private void SetLevelTriggerState() {
        if (levelExitTrigger != null) levelExitTrigger.SetActive(true);
    }

    public void OnLevelCompleteZoneEnter(GameObject player, bool entered) {
        if (entered) {
            lState.onLevelComplete.Invoke();
        }
    }

    public void OnLevelComplete() {
        gameManager.ContinueGame();
    }

    public void OnGameOver() {
        gameManager.FinishGame();
    }
}