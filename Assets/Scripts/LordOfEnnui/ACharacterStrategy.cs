using UnityEngine;

public abstract class ACharacterStrategy : MonoBehaviour {
    public abstract Vector3 MoveDirection();   
    public abstract Vector3 AimDirection();
    public abstract float FireAngle();
    public abstract bool FireThisFrame(ABullet2D bullet, ShootParams shootParams);
    public abstract void OnFire();
}
