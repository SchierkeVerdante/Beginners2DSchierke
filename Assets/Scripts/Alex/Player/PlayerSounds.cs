using Cysharp.Threading.Tasks;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using Zenject;


public class PlayerSounds : MonoBehaviour {
    
    [Header("One-Shot Events")]
    [SerializeField] private EventReference swordHit;
    [SerializeField] private EventReference gunHit;
    [Header("Health")]
    [SerializeField] private EventReference takeDamage;
    [SerializeField] private EventReference explosion;
    [SerializeField] private EventReference death;

    [Header("Movement")]
    [SerializeField] private EventReference jumpSound;
    [SerializeField] private EventReference dashSound;
    [SerializeField] private EventReference landingSound;

    [Header("Weapons")]
    [SerializeField] private EventReference shooting;

    [SerializeField] private EventReference swordAttack;
  
    [SerializeField] private EventReference shotgunAttack;
    [SerializeField] private EventReference minigunAttack;

    [Header("Inventory")]
    [SerializeField] private EventReference pickUp;
    [SerializeField] private EventReference fullOil;
    [SerializeField] private EventReference foundModule;


    [Header("Looped Events")]
    [SerializeField] private LoopingSound movingSound = new LoopingSound();
    private const string speedParamName = "Speed";

    [SerializeField] private LoopingSound shieldSound = new LoopingSound();
    private const string ShieldStrengthParamName = "Strength";

    [SerializeField] private LoopingSound minigun = new LoopingSound();

    private void Start() {
        InitializeLoopSounds();
        TestMovementSound();
    }

    private void InitializeLoopSounds() {
        movingSound.Initialize(transform);
        shieldSound.Initialize(transform);
        minigun.Initialize(transform);
    }

    #region Movement Sounds

    public void SetMoving(bool isEnabled) {
        movingSound.Toggle(isEnabled);
    }

    public void SetMovementSpeed(float speed) {
        movingSound.SetParameterNormalized(speedParamName, speed);
    }

    #endregion


    #region Shield Sounds

    public void SetShield(bool isEnabled) {
        shieldSound.Toggle(isEnabled);
    }

    public void SetShieldStrength(float strength) {
        shieldSound.SetParameterNormalized(ShieldStrengthParamName, strength);
    }

    #endregion

    public void SetMiniGun(bool isEnabled) {
        minigun.Toggle(isEnabled);
    }

    #region One-Shot Sounds

    public void PlayJump() => PlayOneShot(jumpSound);
    public void PlayDash() => PlayOneShot(dashSound);
    public void PlayTakeDamage() => PlayOneShot(takeDamage);
    public void PlayDeath() => PlayOneShot(death);
    public void PlayExplosion() => PlayOneShot(explosion);

    public void PlaySwordAttack() => PlayOneShot(swordAttack);
    public void PlayShooting() => PlayOneShot(shooting);
    public void PlayShotgunAttack() => PlayOneShot(shotgunAttack);
    public void PlayMiniGunAttack() => PlayOneShot(minigunAttack);

    
    public void PlaySwordHit() => PlayOneShot(swordHit);
    public void PlayGunHit() => PlayOneShot(gunHit);

    public void PlayPickUp() => PlayOneShot(pickUp);
    public void PlayFullOil() => PlayOneShot(fullOil);
    public void PlayFoundModule() => PlayOneShot(foundModule);

    public void PlayOneShot(EventReference eventReference) {
        if (!eventReference.IsNull) {
            RuntimeManager.PlayOneShot(eventReference, transform.position);
        }
#if UNITY_EDITOR
        else {
            Debug.LogWarning($"Attempted to play null event reference in {gameObject.name}");
        }
#endif
    }

    #endregion


    #region Cleanup

    private void OnDestroy() {
        ReleaseAllInstances();
    }
    private void ReleaseAllInstances() {
        movingSound.Release();
        shieldSound.Release();
    }

    
    #endregion

#if UNITY_EDITOR
    [ContextMenu("Test Movement Sound")]
    private void TestMovementSound() {
        if (Application.isPlaying) {
            StartTestSound().Forget();
        }
    }

    private async UniTask StartTestSound() {
        SetMovementSpeed(1f);
        SetMoving(true);
        await UniTask.WaitForSeconds(2);
        SetMoving(false);
        await UniTask.WaitForSeconds(2);
        SetMoving(true);
        SetMovementSpeed(0.5f);
        await UniTask.WaitForSeconds(2);
        SetMoving(false);
        SetMovementSpeed(0f);
    }
#endif
}
