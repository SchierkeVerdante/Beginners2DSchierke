using UnityEngine;

public class SimpleAIStrategy : ACharacterStrategy
{
    public override bool FireThisFrame(ABullet2D bullet) {
        float bulletRange = bullet.bulletSpeed * bullet.lifetime;
        return Vector3.Distance(GetFireDirection(), transform.position) < bulletRange;
    }
}
