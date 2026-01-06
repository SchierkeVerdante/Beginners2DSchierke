using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "PlayerState", menuName = "Scriptable Object/Player State")]
public class PlayerState : ScriptableObject {
   
    [Header("Status")]
    public float requiredOil = 80;
    public float maxHealth = 5, currentHealth = 4;
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

    [Header("Modules")]
    public List<ModuleJson> modules = new();
    public ModuleJson netMod;

    private void Awake() {
        CalculateNetParams();
    }

    public void AddModule(ModuleJson module) {
        modules.Add(module);
        CalculateNetParams();
    }

    public void CalculateNetParams() {
        netMod = new ModuleJson {
            dashDamage = null,
            speedMultiplier = 1.0f,
            accelerationMultiplier = 1.0f,
            dashDurationMultipler = 1.0f,
            healthModifier = 0,
        };
        foreach (ModuleJson module in modules) {
            if (module.moduleType == ModuleType.Dash) netMod.dashDamage = module.dashDamage;
            netMod.speedMultiplier *= HandleDefaultMult(module.speedMultiplier);
            netMod.accelerationMultiplier *= HandleDefaultMult(module.accelerationMultiplier);
            netMod.dashDurationMultipler *= HandleDefaultMult(module.dashDurationMultipler);
            netMod.healthModifier += module.healthModifier;
        }
    }

    public static float HandleDefaultMult(float mult) {
        return mult < 0.0001f ? 1.0f : mult;
    }

    public void TakeDamage(float amount) {
        currentHealth -= amount;
        onDamage.Invoke();
        if (currentHealth < 0) {
            onDeath.Invoke();
        }
    }    

    public void ObtainOil(OilPickup oil) {
        currentOil +=  oil.amount;
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
}
