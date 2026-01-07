using Cysharp.Threading.Tasks;
using FMODUnity;
using UnityEngine;

public class EntitySounds : MonoBehaviour {
    [SerializeField] protected EntitySoundProfile soundProfile;

    [Header("Looped Events")]
    [SerializeField] protected LoopingSound movementLoopSound = new LoopingSound();
    protected const string SpeedParamName = "Speed";

    [SerializeField] protected bool enableStartTest = false;

    protected virtual void Start() {
        InitializeLoopSounds();

        if (enableStartTest)
            TestMovementSound();
    }

    protected virtual void InitializeLoopSounds() {
        if (soundProfile != null && !soundProfile.movementLoop.IsNull) {
            movementLoopSound.Initialize(soundProfile.movementLoop, transform);
        }
    }

    #region Movement Sounds

    public virtual void SetMovement(bool isEnabled) {
        movementLoopSound.Toggle(isEnabled);
    }

    public virtual void SetMovementSpeed(float speed) {
        movementLoopSound.SetParameterNormalized(SpeedParamName, speed);
    }

    #endregion

    #region Common Sounds

    public virtual void PlayTakeDamage() {
        if (soundProfile != null)
            PlayOneShot(soundProfile.takeDamage);
    }

    public virtual void PlayDeath() {
        if (soundProfile != null)
            PlayOneShot(soundProfile.death);
    }

    public virtual void PlayExplosion() {
        if (soundProfile != null)
            PlayOneShot(soundProfile.explosion);
    }

    public virtual void PlayJump() {
        if (soundProfile != null)
            PlayOneShot(soundProfile.jump);
    }

    public virtual void PlayLanding() {
        if (soundProfile != null)
            PlayOneShot(soundProfile.landing);
    }

    public virtual void PlayDash() {
        if (soundProfile != null)
            PlayOneShot(soundProfile.dash);
    }

    public virtual void PlayAttack() {
        if (soundProfile != null)
            PlayOneShot(soundProfile.attackSounds);
    }

    public virtual void PlayHit() {
        if (soundProfile != null)
            PlayOneShot(soundProfile.hitSounds);
    }

    public virtual void PlaySpecial() {
        if (soundProfile != null)
            PlayOneShot(soundProfile.specialSounds);
    }

    #endregion

    #region Utility Methods

    public virtual void PlayOneShot(EventReference eventReference) {
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

    protected virtual void OnDestroy() {
        ReleaseAllInstances();
    }

    protected virtual void ReleaseAllInstances() {
        movementLoopSound.Release();
    }

    #endregion

#if UNITY_EDITOR
    [ContextMenu("Test Movement Sound")]
    protected virtual void TestMovementSound() {
        if (Application.isPlaying) {
            StartTestSound().Forget();
        }
    }

    protected virtual async UniTask StartTestSound() {
        SetMovementSpeed(1f);
        SetMovement(true);
        await UniTask.WaitForSeconds(2);
        SetMovement(false);
        await UniTask.WaitForSeconds(2);
        SetMovement(true);
        SetMovementSpeed(0.5f);
        await UniTask.WaitForSeconds(2);
        SetMovement(false);
        SetMovementSpeed(0f);
    }
#endif
}