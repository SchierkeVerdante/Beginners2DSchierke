using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModuleButton : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public Image iconImage;

    private ModuleJson moduleData;
    private UIModuleSelecter selectionUI;

    public void Setup(ModuleJson data, Sprite icon, UIModuleSelecter ui)
    {
        moduleData = data;
        selectionUI = ui;

        nameText.text = data.name;
        descriptionText.text = data.description;
        if (icon != null && iconImage != null) iconImage.sprite = icon;
    }

    public void OnClick()
    {
        selectionUI.SelectModule(moduleData);
    }
}
