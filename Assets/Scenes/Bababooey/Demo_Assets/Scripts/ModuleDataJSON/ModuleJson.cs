using System;

public enum ModuleType
{
    Custom = 0,
    Dash = 1,
    Movement = 2,
    Attack = 3
}

public enum DamageType
{
    Regular = 0,
    Ice = 1,
    Fire = 2
}

public enum InvulnerabilityType
{
    None = 0,
    Enemy = 1,
    EnemyAbility = 2,
    All = 3
}

[Serializable]
public class DashWeapon
{
    public int damage;
    public DamageType damageType;
    public InvulnerabilityType invulnerabilityType;
}

[Serializable]
public class ModuleJson
{
    // --- Core ---
    public int id;
    public string icon;
    public string name;
    public ModuleType moduleType;
    public string description;
    public string rarity;
    public bool unique;

    // --- Dash ---
    public DashWeapon dashDamage;
    public float dashDurationMultipler;

    // --- Movement ---
    public float speedMultiplier;
    public float accelerationMultiplier;

    // --- Survivability ---
    public int healthModifier;
    public float damageIframesBonus;
    public bool grantsDashIframes;

    // --- Economy ---
    public float oilMultiplier;

    // --- Combat ---
    public float fireRateMultiplier;

    // --- Special / Legendary ---
    public bool oneTimeActivation;
    public bool preventsDeathOnce;

    // override object.Equals
    public override bool Equals(object obj) {

        if (obj == null || GetType() != obj.GetType()) {
            return false;
        }

        return ((ModuleJson) obj).id == id;    
    }

    public override int GetHashCode() {
        return id;
    }
}
