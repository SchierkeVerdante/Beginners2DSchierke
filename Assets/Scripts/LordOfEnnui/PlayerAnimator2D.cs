using DG.Tweening;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator2D : ASpriteAnimator2D {

    [SerializeField]
    PlayerInputStrategy strat;

    [Header("AfterImage")]
    [SerializeField]
    SpriteRenderer afterImage;

    [Header("Particles")]
    [SerializeField]
    ParticleSystem thrustParticles, sprintParticles;
    ParticleSystemRenderer thrustRenderer, sprintRenderer;

    [SerializeField]
    float particlesPerSecond = 200f, sprintParticleCount = 1000f, afterImageCreate = 0.01f, afterImageDuration = 0.4f;

    protected override void Start()
    {
        base.Start();
        strat = gameObject.GetComponentInParent<PlayerInputStrategy>();
        thrustRenderer = thrustParticles.gameObject.GetComponent<ParticleSystemRenderer>();
        sprintRenderer = sprintParticles.gameObject.GetComponent<ParticleSystemRenderer>();
        StartCoroutine(AfterImage());
    }

    void FixedUpdate() {
        Vector2 playerInput = strat.MoveDirection();
        float speed = playerInput.magnitude;
        float angle = Vector2.SignedAngle(Vector2.right, playerInput);
        angle = angle < 0 ? 360 + angle : angle;

        SetAnimatorValues(angle, speed);

        if (moving) {
            int particles = Mathf.FloorToInt(strat.isSprinting ? sprintParticleCount : particlesPerSecond * Time.fixedDeltaTime);
            ParticleSystem pSys = strat.isSprinting ? sprintParticles : thrustParticles;
            ParticleSystemRenderer pRend = strat.isSprinting ? thrustRenderer : sprintRenderer;
            float direction = lookLeft ? 0 : lookDown ? 90 : lookRight ? 180 : lookUp ? 270 : -30;
            pSys.transform.eulerAngles = new(0, 0, direction);
            pRend.sortingOrder = lookUp ? 100 : 0;
            pSys.Emit(particles);
        }
    }

    IEnumerator AfterImage() {
        WaitForSeconds wait = new(afterImageCreate);
        while (true) {
            if (strat.isSprinting) {
                SpriteRenderer aImg = Instantiate(afterImage, transform.position, Quaternion.identity, transform);
                aImg.transform.SetParent(null, true);
                aImg.gameObject.SetActive(true);
            
                aImg.sprite = spriteRenderer.sprite;

                aImg.DOFade(0f, afterImageDuration).SetLink(aImg.gameObject).OnComplete(() => Destroy(aImg));
            }
            yield return wait;
        }
    }
}
