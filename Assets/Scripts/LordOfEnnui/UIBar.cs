using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-20)]
public class UIBar : MonoBehaviour
{
    [SerializeField]
    Image fill;

    [Header("ReadOnly")]
    [SerializeField]
    float maxValue = 5, currentValue = 5;

    private void Awake() {
        fill = GetComponent<Image>();
    }

    public void UpdateBar(float current, float max)
    {
        if (enabled == false) return;
        maxValue = max;
        currentValue = Mathf.Min(max, current);
        fill.fillAmount = currentValue / maxValue;
    }
}
