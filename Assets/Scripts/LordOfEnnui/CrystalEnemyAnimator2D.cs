using UnityEngine;

public class CrystalEnemyAnimator2D : ASpriteAnimator2D {

    protected static int hdrIntensity = Shader.PropertyToID("_HDRIntensity");

    [SerializeField]
    Animator eyeAnimator;
    SpriteRenderer eyeRenderer;
    
    [SerializeField]
    AEnemyStrategy strat;

    [SerializeField]
    float maxEyeBrightness = 6f, eyeBrightness;

    [SerializeField]
    AnimationCurve eyeBrightnessCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    [SerializeField, Range(0, 1f)]
    float attackForewarn = 0.5f;

    [SerializeField]
    protected int
        isFacingForward = Animator.StringToHash("isFacingForward"),
        isFacingSide = Animator.StringToHash("isFacingSide"),
        isAttacking = Animator.StringToHash("isAttacking");

    protected override void Start() {
        base.Start();
        strat = GetComponentInParent<AEnemyStrategy>();
        eyeRenderer = eyeAnimator.GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate() {
        ComputeAnimatorValues(strat.facingAngle, strat.speed);
        animator.SetBool(isFacingForward, lookDown);
        animator.SetBool(isFacingSide, lookLeft || lookRight);
        animator.SetBool(isAttacking, strat.timeToFire < attackForewarn);            
        eyeAnimator.SetBool(isFacingForward, lookDown);
        eyeAnimator.SetBool(isFacingSide, lookLeft || lookRight);
        eyeAnimator.SetBool(isAttacking, strat.timeToFire < attackForewarn);
        eyeBrightness = Mathf.Min(1f + maxEyeBrightness * eyeBrightnessCurve.Evaluate(1f - strat.timeToFire), maxEyeBrightness + 1f);
        eyeRenderer.material.SetFloat(hdrIntensity, eyeBrightness);

        eyeAnimator.gameObject.SetActive(!lookUp);
    }
}
