using UnityEngine;

public static class Layers {
    public static int Enemy = LayerMask.NameToLayer("Enemy");
    public static int EnemyAbility = LayerMask.NameToLayer("EnemyAbility");
    public static int Player = LayerMask.NameToLayer("Player");
    public static int PlayerAbility = LayerMask.NameToLayer("PlayerAbility");
    public static int Pickup = LayerMask.NameToLayer("Pickup");

    public static LayerMask EnemyMask = GetMask(Enemy);
    public static LayerMask EnemyAbilityMask = GetMask(EnemyAbility);

    public static LayerMask GetInvMask(InvulnerabilityType type) {
        switch (type) {
            case InvulnerabilityType.None:
                return 0;
            case InvulnerabilityType.Enemy:
                return EnemyMask;
            case InvulnerabilityType.EnemyAbility:
                return EnemyAbilityMask;
            case InvulnerabilityType.All:
                return EnemyAbilityMask | EnemyMask;
            default:
                return 0;
        }
    }

    public static LayerMask GetMask(int index) {
        return 1 << index;
    }
}
