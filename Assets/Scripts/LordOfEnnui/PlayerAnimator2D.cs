using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator2D : ASpriteAnimator2D {

    [SerializeField]
    PlayerState pState;

    [Header("Particles")]
    [SerializeField]
    ParticleSystem thrustParticles, sprintParticles, sprintDamageParticles;
    ParticleSystemRenderer thrustRenderer, sprintRenderer, sprintDamageRenderer;

    [SerializeField]
    float particlesPerSecond = 200f, sprintParticleCount = 1000f, damageSprintParticleCount = 1000f;


    protected override void Start()
    {
        base.Start();
        pState = LDirectory2D.Instance.pState;
        thrustRenderer = thrustParticles.gameObject.GetComponent<ParticleSystemRenderer>();
        sprintRenderer = sprintParticles.gameObject.GetComponent<ParticleSystemRenderer>();
    }

    void FixedUpdate() {
        Vector2 playerInput = strat.MoveDirection();
        float speed = playerInput.magnitude;
        float angle = Vector2.SignedAngle(Vector2.right, playerInput);
        angle = angle < 0 ? 360 + angle : angle;

        ComputeAnimatorValues(angle, speed);
        SetAnimatorValues();

        if (moving) {
            int particles = Mathf.FloorToInt(strat.isSprinting ? sprintParticleCount : particlesPerSecond * Time.fixedDeltaTime);
            ParticleSystem pSys = strat.isSprinting ? sprintParticles : thrustParticles;
            ParticleSystemRenderer pRend = strat.isSprinting ? sprintRenderer : thrustRenderer;

            float direction = lookLeft ? 0 : lookDown ? 90 : lookRight ? 180 : lookUp ? 270 : -30;
            pSys.transform.eulerAngles = new(0, 0, direction);
            sprintDamageParticles.transform.eulerAngles = new(0, 0, direction);

            pRend.sortingOrder = lookUp ? 100 : 0;
            if (pState.netMod.dashDamage != null && strat.isSprinting) {
                sprintDamageParticles.Emit(Mathf.FloorToInt(damageSprintParticleCount * Time.fixedDeltaTime));
            } else {
                pSys.Emit(particles);
            }
        }
    }
}
