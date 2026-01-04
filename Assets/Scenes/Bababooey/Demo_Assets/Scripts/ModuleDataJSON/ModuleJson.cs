using System;

public enum ModuleType {
    Custom = 0,
    Dash = 1,
    Movement = 2,
    Attack = 3
}

public enum DamageType {
    Regular = 0,
    Ice = 1,
    Fire = 2
}

public enum InvulnerabilityType {
    None = 0,
    Enemy = 1,
    EnemyAbility = 2,
    All = 3
}    

[Serializable]
public class DashWeapon {
    public int damage;
    public DamageType damageType;
    public InvulnerabilityType invulnerabilityType;
}

[Serializable]
public class ModuleJson
{
    public int id;
    public string icon;
    public string name;
    public ModuleType moduleType;
    public string description;    
    public string rarity;
    public DashWeapon dashDamage;
    public float speedMultiplier, accelerationMultiplier, dashDurationMultipler;
    public int healthModifier;
    public string[] abilities;
}