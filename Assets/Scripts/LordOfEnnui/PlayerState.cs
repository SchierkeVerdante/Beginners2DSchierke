using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "PlayerState", menuName = "Scriptable Object/Player State")]
public class PlayerState : ScriptableObject {
   
    [Header("Status")]
    public float requiredOil = 80;
    public float maxHealth = 15, currentHealth = 15;
    public float maxOil = 100, currentOil = 0;

    [Header("Invincibility")]
    public float flashesPerSecond = 2;
    public float damageIframesDuration = 1, sprintIframesDuration = 0.5f;

    [Header("HitStop")]
    public float damageHitStopDuration = 0.1f;
    public float damageHitStopTimeScale = 0.0f;

    [Header("Events")]
    public UnityEvent onDamage;
    public UnityEvent onDeath;
    public UnityEvent onFire;
    public UnityEvent onDash;
    public UnityEvent<ModulePickup> onModulePickup;
    public UnityEvent<OilPickup> onOilPickup;
    public UnityEvent onSufficientOil;

    [Header("Audio")]
    public float moveSpeedForAudio;

    [Header("Base Stats")]
    public float baseMaxHealth = 15;
    public float baseDamageIframesDuration = 1f;

    [Header("Modules")]
    public List<ModuleJson> modules = new();
    public ModuleJson netMod;

    private void Awake() {
        CalculateNetParams();
        DebugPlayerState();
    }

    public void AddModule(ModuleJson module) {
        modules.Add(module);
        CalculateNetParams();
        ApplyNetStats();
        DebugPlayerState();
    }

    public void CalculateNetParams() {
        netMod = new ModuleJson {
            dashDamage = null,
            speedMultiplier = 1.0f,
            accelerationMultiplier = 1.0f,
            dashDurationMultipler = 1.0f,
            healthModifier = 0,
            oilMultiplier = 1f,
            fireRateMultiplier = 1f,
            damageIframesBonus = 0f
        };
        foreach (ModuleJson module in modules) {
            if (module.moduleType == ModuleType.Dash) netMod.dashDamage = module.dashDamage;
            netMod.speedMultiplier *= HandleDefaultMult(module.speedMultiplier);
            netMod.accelerationMultiplier *= HandleDefaultMult(module.accelerationMultiplier);
            netMod.dashDurationMultipler *= HandleDefaultMult(module.dashDurationMultipler);
            netMod.healthModifier += module.healthModifier;
            netMod.oilMultiplier *= HandleDefaultMult(module.oilMultiplier);
            netMod.fireRateMultiplier *= HandleDefaultMult(module.fireRateMultiplier);
            netMod.damageIframesBonus += module.damageIframesBonus;
        }
    }

    public static float HandleDefaultMult(float mult) {
        return mult < 0.0001f ? 1.0f : mult;
    }

    public void ApplyNetStats()
    {
        float newMaxHealth = baseMaxHealth + netMod.healthModifier;
        newMaxHealth = Mathf.Max(1, newMaxHealth); // never allow 0 or less

        //adjusting health proportionally
        float healthPercent = currentHealth / Mathf.Max(1, maxHealth);
        maxHealth = newMaxHealth;
        currentHealth = Mathf.Clamp(maxHealth * healthPercent, 1, maxHealth);
        Debug.Log("max-health: " + maxHealth);
        Debug.Log("current-health: " + currentHealth);

        damageIframesDuration = baseDamageIframesDuration + netMod.damageIframesBonus;
        damageIframesDuration = Mathf.Max(0.05f, damageIframesDuration);

    }

    public void TakeDamage(float amount) {
        currentHealth -= amount;
        onDamage.Invoke();
        if (currentHealth < 0) {
            onDeath.Invoke();
        }
    }    

    public void ObtainOil(OilPickup oil) {
        currentOil +=  (oil.amount * netMod.oilMultiplier);
        onOilPickup.Invoke(oil);
        if (currentOil >= requiredOil) {
            onSufficientOil.Invoke();
        }
    }

    public void OnFire() {
        onFire.Invoke();
    }

    public void OnDash() {
        onDash.Invoke();
    }

    public void ModuleChoice(ModulePickup module) {
        onModulePickup.Invoke(module);
    }

    [ContextMenu("Debug Player State")]
    public void DebugPlayerState()
    {
        Debug.Log("===== PLAYER STATE DEBUG =====");

        Debug.Log($"Health: {currentHealth}/{maxHealth}");
        Debug.Log($"Oil: {currentOil}/{maxOil}");

        Debug.Log("--- NET MULTIPLIERS (After Modules) ---");
        Debug.Log($"Speed Multiplier: {netMod.speedMultiplier}");
        Debug.Log($"Acceleration Multiplier: {netMod.accelerationMultiplier}");
        Debug.Log($"Dash Duration Multiplier: {netMod.dashDurationMultipler}");
        Debug.Log($"Oil Multiplier: {netMod.oilMultiplier}");
        Debug.Log($"BonusIframes: {netMod.damageIframesBonus}");
        Debug.Log($"Health Modifier: {netMod.healthModifier}");

        if (netMod.dashDamage != null)
        {
            Debug.Log($"Dash Damage: {netMod.dashDamage.damage}");
        }
        else
        {
            Debug.Log("Dash Damage: None");
        }

        Debug.Log("--- ACTIVE MODULES ---");
        if (modules.Count == 0)
        {
            Debug.Log("No modules equipped.");
        }

        foreach (ModuleJson module in modules)
        {
            Debug.Log(
                $"[{module.name}] " +
                $"Type: {module.moduleType}, " +
                $"Speed x{HandleDefaultMult(module.speedMultiplier)}, " +
                $"Accel x{HandleDefaultMult(module.accelerationMultiplier)}, " +
                $"Dash x{HandleDefaultMult(module.dashDurationMultipler)}, " +
                $"Health {module.healthModifier}"
            );
        }

        Debug.Log("===== END DEBUG =====");
    }
}
