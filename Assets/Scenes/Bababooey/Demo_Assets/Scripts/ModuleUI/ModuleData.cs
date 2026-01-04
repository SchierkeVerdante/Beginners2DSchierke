using UnityEngine;

[CreateAssetMenu(menuName = "Modules/Module")]
public class ModuleData : ScriptableObject
{
    public string moduleName;
    public string description;

    public Sprite icon;

    public float speedMultiplier = 1f;
    public float healthModifier = 0f;
}
