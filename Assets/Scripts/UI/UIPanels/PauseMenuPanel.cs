using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PauseMenuPanel : UIPanel {
    [SerializeField] private Button contuniueButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;

    [Inject] IGameManager gameManager;
    [Inject] IUiService _uiService;

    protected override void Awake() {
        base.Awake();
        contuniueButton.onClick.AddListener(OnContinueClicked);
        exitButton.onClick.AddListener(OnExitClicked);
        settingsButton.onClick.AddListener(OnSettingsClicked);
    }

    private void OnContinueClicked() {
        gameManager.TogglePause();
    }

    private void OnExitClicked() {
        gameManager.ExitToMainMenu();
    }

    private void OnSettingsClicked() {
        _uiService.ShowPanel(_uiService.SettingsMenu);
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        contuniueButton.onClick.RemoveListener(OnContinueClicked);
        exitButton.onClick.RemoveListener(OnExitClicked);
        settingsButton.onClick.RemoveListener(OnSettingsClicked);
    }
}