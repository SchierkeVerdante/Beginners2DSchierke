using UnityEngine;
using UnityEngine.UI;

public class UIAnimatedHealthBar : MonoBehaviour {

    [SerializeField]
    Image[] images;

    [SerializeField]
    Sprite[] sprites;

    [SerializeField]
    Shaker shaker;

    [SerializeField]
    bool shake;

    private void Awake() {
        images = GetComponentsInChildren<Image>();       
        shaker = GetComponent<Shaker>();
        shake = true;
    }

    public void UpdateBar(float currentHealth = float.MaxValue, float maxHealth = float.MaxValue, bool shakeBar = true) {
        if (shaker != null && shake && shakeBar) shaker.ShakeObject();
        for (int i = 0; i < images.Length; i++) {
            images[i].sprite = sprites[(int) Mathf.Min(Mathf.Max(currentHealth - 6 * i, 0f), sprites.Length - 1)];
            images[i].gameObject.SetActive(i < Mathf.CeilToInt(maxHealth / 6));
        }
    }
}
