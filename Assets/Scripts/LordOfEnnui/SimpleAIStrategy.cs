using UnityEngine;

public class SimpleAIStrategy : AEnemyStrategy
{
    public override bool FireThisFrame(ABullet2D bullet) {
        float bulletRange = bullet.bulletSpeed * bullet.lifetime;
        return Vector3.Distance(GetAimDirection(), transform.position) < bulletRange;
    }
}
