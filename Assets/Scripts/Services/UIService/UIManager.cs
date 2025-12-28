using System;
using UnityEngine;
using Zenject;

public class UIManager : MonoBehaviour, IUiService {

    [SerializeField] UIPanel pauseUI;

    [SerializeField] SettingsMenuPanel settingsPanel;
    [SerializeField] LoadingScreenPanel loading;

    public LoadingScreenPanel LoadingUI => loading;
    public SettingsMenuPanel SettingsMenu => settingsPanel;
    public UIPanel PauseUI => pauseUI;

    public void HidePanel(UIPanel panel) {
        panel.Hide();
    }

    public void ShowPanel(UIPanel panel) {
        panel.Show();
    }

    public void TogglePanel(UIPanel panel) {
        if (panel.IsOpen) {
            panel.Hide();
        } else {
            panel.Show();
        }
    }
}
