using UnityEngine;
using UnityEngine.UI;

public class UIAnimatedHealthBar : MonoBehaviour {

    [SerializeField]
    Image[] images;
    
    [SerializeField]
    PlayerState pState;

    [SerializeField]
    Sprite[] sprites;

    [SerializeField]
    Shaker shaker;

    private void Awake() {
        images = GetComponentsInChildren<Image>();
        shaker = GetComponent<Shaker>();
        pState = LDirectory2D.Instance.pState;
        pState.onDamage.AddListener(UpdateBar);
        UpdateBar();
    }

    private void UpdateBar() {
        shaker.ShakeObject();
        for (int i = 0; i < images.Length; i++) {
            images[i].sprite = sprites[(int) Mathf.Min(Mathf.Max(pState.currentHealth - 6 * i, 0f), sprites.Length - 1)];
        }
    }
}
