using Cysharp.Threading.Tasks;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using Zenject;


public class PlayerSounds : EntitySounds {
    PlayerSoundProfile playerSoundProfile;

    [Header("Player Looped Sounds")]
     private LoopingSound shieldSound = new LoopingSound();
    private const string ShieldStrengthParamName = "Strength";

    private LoopingSound minigunSound = new LoopingSound();

    protected override void Start() {
        if (soundProfile is PlayerSoundProfile playerProfile) {
            playerSoundProfile = playerProfile;
        }

        base.Start();
    }

    protected override void InitializeLoopSounds() {
        base.InitializeLoopSounds();

        if (playerSoundProfile != null && !playerSoundProfile.shieldSound.IsNull) {
            shieldSound.Initialize(playerSoundProfile.shieldSound, transform);
        }

        minigunSound.Initialize(playerSoundProfile.minigunAttack, transform);
    }

    #region Shield Sounds

    public void SetShield(bool isEnabled) {
        shieldSound.Toggle(isEnabled);
    }

    public void SetShieldStrength(float strength) {
        shieldSound.SetParameterNormalized(ShieldStrengthParamName, strength);
    }

    #endregion

    #region Weapon Sounds

    public void SetMiniGun(bool isEnabled) {
        minigunSound.Toggle(isEnabled);
    }

    public void PlaySwordAttack() {
        if (playerSoundProfile != null)
            PlayOneShot(playerSoundProfile.swordAttack);
    }

    public void PlayShooting() {
        if (playerSoundProfile != null)
            PlayOneShot(playerSoundProfile.shooting);
    }

    public void PlayShotgunAttack() {
        if (playerSoundProfile != null)
            PlayOneShot(playerSoundProfile.shotgunAttack);
    }

    public void PlayMiniGunAttack() {
        if (playerSoundProfile != null)
            PlayOneShot(playerSoundProfile.minigunAttack);
    }

    #endregion

    #region Combat Hit Sounds

    public void PlaySwordHit() {
        if (playerSoundProfile != null)
            PlayOneShot(playerSoundProfile.swordHit);
    }

    public void PlayGunHit() {
        if (playerSoundProfile != null)
            PlayOneShot(playerSoundProfile.gunHit);
    }

    #endregion

    #region Inventory Sounds

    public void PlayPickUp() {
        if (playerSoundProfile != null)
            PlayOneShot(playerSoundProfile.pickUp);
    }

    public void PlayFullOil() {
        if (playerSoundProfile != null)
            PlayOneShot(playerSoundProfile.fullOil);
    }

    public void PlayFoundModule() {
        if (playerSoundProfile != null)
            PlayOneShot(playerSoundProfile.foundModule);
    }

    #endregion

    #region Cleanup

    protected override void ReleaseAllInstances() {
        base.ReleaseAllInstances();
        shieldSound.Release();
        minigunSound.Release();
    }

    #endregion
}
