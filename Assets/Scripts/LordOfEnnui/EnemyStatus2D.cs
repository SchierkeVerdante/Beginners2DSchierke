using UnityEngine;

public class EnemyStatus2D : ACharacterStatus2D
{
    protected override bool OnCollsionIsDamaged(GameObject other) {
        return other.layer == Layers.PlayerAbility;
    }

    protected override void OnDamageTaken() {
        
    }
}
