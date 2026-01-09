using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

[DefaultExecutionOrder(-100)]
public class LDirectory2D : MonoBehaviour {
    public static LDirectory2D Instance;

    [Inject]
    public IGameManager gameManager;
    public ILevelProgressService levelProgress;

    public GameObject pCamera;
    public GameObject player;
    public PlayerInputStrategy playerInputStrategy;
    public PlayerController2D playerController;
    public PlayerCollision2D playerCollision;
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
        levelProgress = gameManager.GetLevelProgress();
        pState = levelProgress.GetPlayerState();
        if (pState == null) {
            pState = Instantiate(defaultPlayerState);
            gameManager.GetLevelProgress().SetPlayerState(pState);
        }        
        pState.OnNewLevel();
        lState = Instantiate(defaultLevelState);

        LoadModules(pState.modules);

        playerInputStrategy = player.GetComponent<PlayerInputStrategy>();
        playerController = player.GetComponent<PlayerController2D>();
        playerCollision = player.GetComponent<PlayerCollision2D>();
    }

    void LoadModules(List<ModuleJson> modules) {
        TextAsset jsonFile = Resources.Load<TextAsset>("Modules");

        if (jsonFile == null) {
            Debug.LogError("Modules.json not found!");
            return;
        }

        ModuleDatabaseJson database =
            JsonUtility.FromJson<ModuleDatabaseJson>(jsonFile.text);

        LoadedModules = database.modules.Where(module => !(module.unique && modules.Contains(module))).ToList();

        Debug.Log("Loaded modules: " + LoadedModules.Count);
    }
}
