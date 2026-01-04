using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ModuleButton : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public Image iconImage;

    private ModuleJson moduleData;
    private ModuleSelectionUI selectionUI;

    public void Setup(ModuleJson data, Sprite icon, ModuleSelectionUI ui)
    {
        moduleData = data;
        selectionUI = ui;

        nameText.text = data.name;
        descriptionText.text = data.description;
        iconImage.sprite = icon;
    }

    public void OnClick()
    {
        selectionUI.SelectModule(moduleData);
    }
}
