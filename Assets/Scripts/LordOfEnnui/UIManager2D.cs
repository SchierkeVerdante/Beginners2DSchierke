using UnityEngine;
using UnityEngine.UI;

public class UIManager2D : MonoBehaviour
{
    [SerializeField]
    UIBar oilBar;

    [SerializeField]
    UIAnimatedHealthBar healthBar;

    [SerializeField]
    UIModuleSelecter moduleSelecter;

    [SerializeField]
    Image gameOverPanel, levelCompletePanel, gameCompletePanel;

    [SerializeField]
    PlayerState pState;

    [SerializeField]
    LevelState lState;

    private void Awake() {
        pState = LDirectory2D.Instance.pState;
        lState = LDirectory2D.Instance.lState;
        pState.onDamage.AddListener(() => UpdateHealthBar(true));
        pState.onOilPickup.AddListener(UpdateOilBar);
        pState.onModulePickup.AddListener(OnModulePickup);
        moduleSelecter.onSelectModule.AddListener(OnModuleSelect);
        pState.onDeath.AddListener(ShowDeathPanel);
        lState.onLevelComplete.AddListener(ShowLevelCompletePanel);
    }

    private void ShowDeathPanel() {
        Time.timeScale = 0.0f;
        lState.SetUIActive(true);
        gameOverPanel.gameObject.SetActive(true);
    }

    private void ShowLevelCompletePanel() {
        Time.timeScale = 0.0f;
        lState.SetUIActive(true);
        levelCompletePanel.gameObject.SetActive(true);
    }

    private void ShowGameSuccessPanel() {
        Time.timeScale = 0.0f;
        lState.SetUIActive(true);
        gameCompletePanel.gameObject.SetActive(true);
    }

    private void Start() {
        UpdateHealthBar(false);
        UpdateOilBar();
    }

    private void UpdateOilBar(OilPickup oil = null) {
        oilBar.UpdateBar(pState.currentOil, pState.maxOil);
    }

    private void UpdateHealthBar(bool shake) {
        healthBar.UpdateBar(pState.currentHealth, pState.maxHealth, shake);
    }

    private void OnModulePickup(ModulePickup module) {
        pState.moveSpeedForAudio = 0f;
        moduleSelecter.Show(module);
        lState.SetUIActive(true);
    }

    private void OnModuleSelect(ModuleJson module) {
        lState.SetUIActive(false);
        pState.AddModule(module);
        UpdateHealthBar(false);
    }
}
