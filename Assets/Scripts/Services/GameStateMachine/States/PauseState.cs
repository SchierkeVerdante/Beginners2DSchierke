using UnityEngine;

public class PauseState : State<GameManager> {
    private IUiService _uiService;
    public PauseState(IUiService uiService) {
        _uiService = uiService;
    }

    public override void Enter() {
        Time.timeScale = 0f;

        _uiService.ShowPanel(_uiService.PauseUI);
    }

    public override void Exit() {
        _uiService.HidePanel(_uiService.PauseUI);
        _uiService.HidePanel(_uiService.SettingsMenu);

        Time.timeScale = 1f;
    }
}
