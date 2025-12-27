public interface IUiService {
    LoadingScreenPanel LoadingUI { get; }
    SettingsMenuPanel SettingsMenu { get; }
    UIPanel PauseUI { get; }

    void HidePanel(UIPanel pauseUI);
    void ShowPanel(UIPanel pauseUI);

    void TogglePanel(UIPanel panel);
}
