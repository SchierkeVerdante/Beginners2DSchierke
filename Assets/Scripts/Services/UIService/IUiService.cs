public interface IUiService {
    LoadingScreenUI LoadingUI { get; }
    PauseMenuUI PauseUI { get; }

    void HidePanel(UIPanel pauseUI);
    void ShowPanel(UIPanel pauseUI);

    void TogglePanel(UIPanel panel);
}
