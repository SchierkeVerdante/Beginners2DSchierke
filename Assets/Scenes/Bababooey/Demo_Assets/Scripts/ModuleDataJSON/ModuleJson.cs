using System;

[Serializable]
public class ModuleJson
{
    public string id;
    public string name;
    public string icon;
    public string description;
    public float speedMultiplier;
    public int healthModifier;
    public string[] abilities;
    public string rarity;
}