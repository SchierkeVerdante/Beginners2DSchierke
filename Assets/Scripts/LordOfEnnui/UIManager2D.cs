using UnityEngine;

public class UIManager2D : MonoBehaviour
{
    [SerializeField]
    UIBar oilBar;

    [SerializeField]
    UIAnimatedHealthBar healthBar;

    [SerializeField]
    UIModuleSelecter moduleSelecter;

    [SerializeField]
    PlayerState pState;

    [SerializeField]
    LevelState lState;

    private void Awake() {
        pState = LDirectory2D.Instance.pState;
        lState = LDirectory2D.Instance.lState;
        pState.onDamage.AddListener(UpdateHealthBar);
        pState.onOilPickup.AddListener(UpdateOilBar);
        pState.onModulePickup.AddListener(OnModulePickup);
        moduleSelecter.onSelectModule.AddListener(OnModuleSelect);
    }

    private void Start() {
        UpdateHealthBar();
        UpdateOilBar();
    }

    private void UpdateOilBar(OilPickup oil = null) {
        oilBar.UpdateBar(pState.currentOil, pState.maxOil);
    }

    private void UpdateHealthBar() {
        healthBar.UpdateBar(pState.currentHealth, pState.maxHealth);
    }

    private void OnModulePickup(ModulePickup module) {
        pState.moveSpeedForAudio = 0f;
        moduleSelecter.Show(module);
        lState.SetUIActive(true);
    }

    private void OnModuleSelect(ModuleJson module) {
        lState.SetUIActive(false);
        pState.AddModule(module);
    }
}
