using Cysharp.Threading.Tasks;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using Zenject;

public class PlayerSounds : MonoBehaviour {
    [Header("One-Shot Events")]
    [SerializeField] private EventReference jumpSound;
    [SerializeField] private EventReference dashSound;

    [SerializeField] private EventReference sword;
    [SerializeField] private EventReference shooting;

    [SerializeField] private EventReference takeDamage;
    [SerializeField] private EventReference explosion;
    [SerializeField] private EventReference death;

    [SerializeField] private EventReference swordHit;
    [SerializeField] private EventReference gunHit;

    [Header("Looped Events")]
    [SerializeField] private EventReference movingSound;
    [SerializeField] private EventReference shieldSound;

    private FmodAudioService audioService;

    [Inject]
    public void Construct(FmodAudioService audioService) {
        this.audioService = audioService;
    }

    private void Start() {
        StartTestSound().Forget();
    }

    private async UniTask StartTestSound() {
        SetMoving(true);
        await UniTask.WaitForSeconds(4);
        SetMoving(false);
    }

    #region Movement Sounds

    public void SetMoving(bool isMoving) {
        if (isMoving) {
            if (!audioService.IsLoopedPlaying(movingSound.Guid)) {
                audioService.PlayLooped(movingSound.Guid, movingSound, transform);
            }
        } else {
            audioService.StopLooped(movingSound.Guid);
        }
    }

    public void SetMovementSpeed(float speed) {
        audioService.SetLoopedParameter(movingSound.Guid, "Speed", speed);
    }

    #endregion

    #region Shield Sounds

    public void SetShield(bool isActive) {
        if (isActive) {
            if (!audioService.IsLoopedPlaying(shieldSound.Guid)) {
                audioService.PlayLooped(shieldSound.Guid, shieldSound, transform);
            }
        } else {
            audioService.StopLooped(shieldSound.Guid);
        }
    }

    public void SetShieldStrength(float strength) {
        audioService.SetLoopedParameter(shieldSound.Guid, "Strength", strength);
    }

    #endregion

    #region One-Shot Sounds

    public void PlayJump() => PlaySound(jumpSound);
    public void PlayDash() => PlaySound(dashSound);
    public void PlayTakeDamage() => PlaySound(takeDamage);
    public void PlayDeath() => PlaySound(death);
    public void PlayExplosion() => PlaySound(explosion);
    public void PlaySwordAttack() => PlaySound(sword);
    public void PlayShooting() => PlaySound(shooting);
    public void PlaySwordHit() => PlaySound(swordHit);
    public void PlayGunHit() => PlaySound(gunHit);

    public void PlayLanding(float impactForce) {
        audioService.PlayOneShotWithParameter(
            jumpSound,  // „и це правильний звук дл€ приземленн€?
            transform.position,
            "Impact",
            impactForce
        );
    }

    public void PlaySound(EventReference eventReference) {
        audioService.PlayOneShot(eventReference, transform.position);
    }

    #endregion

    private void OnDestroy() {
        // «упин€Їмо вс≥ звуки цього гравц€
        audioService?.StopLooped(movingSound.Guid);
        audioService?.StopLooped(shieldSound.Guid);
    }

    private void OnDisable() {
        audioService?.StopLooped(movingSound.Guid);
        audioService?.StopLooped(shieldSound.Guid);
    }
}
