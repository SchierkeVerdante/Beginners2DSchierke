using System;
using UnityEngine;

[Serializable]
public abstract class ABullet2D : MonoBehaviour
{
    [SerializeField]
    public float lifetime = 1f;

    [SerializeField]
    public bool playerBullet = false;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        Destroy(gameObject);
        LDirectory2D.Instance.lState.onBulletHit.Invoke();
    }
}
