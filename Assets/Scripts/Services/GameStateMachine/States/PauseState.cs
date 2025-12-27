using UnityEngine;

public class PauseState : State {
    private IUiService _uiService;
    public PauseState(IStateMachine stateMachine, IUiService uiService) : base(stateMachine) {
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
