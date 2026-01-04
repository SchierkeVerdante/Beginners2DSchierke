using UnityEngine;
using UnityEngine.Events;

public class LevelState : ScriptableObject {
    public bool uiActive;

    [Header("Events")]
    public UnityEvent<bool> onUI;

    public void SetUIActive(bool active) {
        uiActive = active;
        onUI.Invoke(uiActive);
    }
}
