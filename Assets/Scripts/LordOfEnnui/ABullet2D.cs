using UnityEngine;

public abstract class ABullet2D : MonoBehaviour
{
    [SerializeField]
    public float fireRate = 3f, fireSpread = 10f, lifetime = 1f, bulletSpeed = 10f, recoilForce = 5f;

    [SerializeField]
    public bool playerBullet = false;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        Destroy(gameObject);
    }
}
