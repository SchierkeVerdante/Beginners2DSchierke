using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class LDirectory2D : MonoBehaviour {
    public static LDirectory2D Instance;

    public GameObject pCamera;
    public GameObject player;
    public PlayerController2D playerController;
    public PlayerCollision2D playerStatus;
    public Shaker screenShaker;

    public PlayerState defaultPlayerState;
    public LevelState defaultLevelState;

    [Header("ReadOnly")]
    public PlayerState pState;
    public LevelState lState;
    public List<ModuleJson> LoadedModules;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(Instance);
        }
        if (defaultPlayerState == null) defaultPlayerState = ScriptableObject.CreateInstance<PlayerState>();
        if (defaultLevelState == null) defaultLevelState = ScriptableObject.CreateInstance<LevelState>();
        pState = Instantiate(defaultPlayerState);
        lState = Instantiate(defaultLevelState);
        LoadModules();
    }

    void LoadModules() {
        TextAsset jsonFile = Resources.Load<TextAsset>("Modules");

        if (jsonFile == null) {
            Debug.LogError("Modules.json not found!");
            return;
        }

        ModuleDatabaseJson database =
            JsonUtility.FromJson<ModuleDatabaseJson>(jsonFile.text);

        LoadedModules = new List<ModuleJson>(database.modules);

        Debug.Log("Loaded modules: " + LoadedModules.Count);
    }
}
