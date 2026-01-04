using Cysharp.Threading.Tasks;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using Zenject;


public class PlayerSounds : MonoBehaviour {
    
    [Header("One-Shot Events")]
    [SerializeField] private EventReference jumpSound;
    [SerializeField] private EventReference dashSound;
    [SerializeField] private EventReference swordAttack;
    [SerializeField] private EventReference shooting;
    [SerializeField] private EventReference takeDamage;
    [SerializeField] private EventReference explosion;
    [SerializeField] private EventReference death;
    [SerializeField] private EventReference swordHit;
    [SerializeField] private EventReference gunHit;
    [SerializeField] private EventReference landingSound;

    [Header("Looped Events")]
    [SerializeField] private EventReference movingSound;
    private const string speedParamName = "Speed";
    [SerializeField] private EventReference shieldSound;
    private const string ShiedlStrengthParamName = "Strength";
    // ≤нстанси створюЇмо один раз
    private EventInstance _movingInstance;
    private EventInstance _shieldInstance;


    private void Start() {
        InitializeLoopInstances();
        TestMovementSound();
    }

    private void InitializeLoopInstances() {
        // –ух
        _movingInstance = RuntimeManager.CreateInstance(movingSound);
        RuntimeManager.AttachInstanceToGameObject(_movingInstance, transform);

        // ўит
        _shieldInstance = RuntimeManager.CreateInstance(shieldSound);
        RuntimeManager.AttachInstanceToGameObject(_shieldInstance, transform);
    }

    #region Movement Sounds

    public void SetMoving(bool isEnabled) {
        if (!_movingInstance.isValid()) return;

        if (isEnabled) {
            _movingInstance.start();
            
        } else {
            _movingInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    public void SetMovementSpeed(float speed) {
        if (!_movingInstance.isValid()) return;

        _movingInstance.setParameterByName(speedParamName, Mathf.Clamp01(speed));
    }

    #endregion

    #region Shield Sounds

    public void SetShield(bool isEnabled) {
        if (!_movingInstance.isValid()) return;

        if (isEnabled) {
            _movingInstance.start();
        } else {
            _movingInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    public void SetShieldStrength(float strength) {
        if (!_shieldInstance.isValid()) return;

        _shieldInstance.setParameterByName(ShiedlStrengthParamName, Mathf.Clamp01(strength));
    }

    #endregion

    #region One-Shot Sounds

    public void PlayJump() => PlayOneShot(jumpSound);
    public void PlayDash() => PlayOneShot(dashSound);
    public void PlayTakeDamage() => PlayOneShot(takeDamage);
    public void PlayDeath() => PlayOneShot(death);
    public void PlayExplosion() => PlayOneShot(explosion);
    public void PlaySwordAttack() => PlayOneShot(swordAttack);
    public void PlayShooting() => PlayOneShot(shooting);
    public void PlaySwordHit() => PlayOneShot(swordHit);
    public void PlayGunHit() => PlayOneShot(gunHit);

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
        ReleaseInstance(ref _movingInstance);
        ReleaseInstance(ref _shieldInstance);
    }

    private void ReleaseInstance(ref EventInstance instance) {
        if (instance.isValid()) {
            instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            instance.release();
            instance = default;
        }
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
        SetMoving(true);
        await UniTask.WaitForSeconds(2);
        SetMoving(false);
        await UniTask.WaitForSeconds(2);
        SetMoving(true);
        await UniTask.WaitForSeconds(2);
        SetMoving(false);
    }
#endif
}