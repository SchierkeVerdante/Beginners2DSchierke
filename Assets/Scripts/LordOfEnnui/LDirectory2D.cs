using UnityEngine;

public class LDirectory2D : MonoBehaviour {
    public static LDirectory2D Instance;

    public GameObject player;
    public PlayerController2D playerController;
    public GameObject pCamera;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(Instance);
        }
    }

}
