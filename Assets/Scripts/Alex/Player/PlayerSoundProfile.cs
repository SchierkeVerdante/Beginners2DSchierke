using FMODUnity;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Sound Profile", menuName = "Audio/Player Sound Profile")]
public class PlayerSoundProfile : EntitySoundProfile {
    [Header("=== Player Combat Sounds ===")]
    public EventReference swordHit;
    public EventReference gunHit;

    [Header("=== Player Weapon Sounds ===")]
    public EventReference shooting;
    public EventReference swordAttack;
    public EventReference shotgunAttack;
    public EventReference minigunAttack;

    [Header("=== Player Inventory Sounds ===")]
    public EventReference pickUp;
    public EventReference fullOil;
    public EventReference foundModule;

    [Header("=== Player Special Sounds ===")]
    public EventReference shieldSound;
    public string shieldStrengthParameterName = "Strength";
}