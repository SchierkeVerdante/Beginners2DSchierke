using UnityEngine;

public class EnemyAnimator2D : ASpriteAnimator2D
{
    [SerializeField]
    AEnemyStrategy strat;

    protected override void Start() {
        base.Start();
        strat = GetComponentInParent<AEnemyStrategy>();
    }

    private void FixedUpdate() {
        SetAnimatorValues(strat.facingAngle, strat.speed);
    }
}
