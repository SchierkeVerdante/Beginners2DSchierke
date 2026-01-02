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
    [Header("ReadOnly")]
    public PlayerState pState;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(Instance);
        }
        if (defaultPlayerState == null) defaultPlayerState = ScriptableObject.CreateInstance<PlayerState>();
        pState = Instantiate(defaultPlayerState);
    }
}
