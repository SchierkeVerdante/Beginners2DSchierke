using UnityEngine;

public enum FireStreamType {
    SingleStream,
    EvenSpacing,
    Custom
}

[DefaultExecutionOrder(10)]
[RequireComponent(typeof(Rigidbody2D))]
public class ProjectileShooter2D : MonoBehaviour
{
    [SerializeField]
    ACharacterStrategy shootStrat;
    
    [SerializeField]
    GameObject bulletObject;

    [SerializeField]
    ABullet2D bullet;

    [SerializeField]
    Rigidbody2D rb;

    [SerializeField, Header("Persistent Data")]
    ShootParams shootParams;

    [SerializeField, Range(0f, 1f)]
    float inheritVelocityMultiplier = 1.0f;

    [Header("ReadOnly")]
    [SerializeField]
    float timeToBullet;
    float bulletTimer = 0;

    void Start()
    {
        if (shootStrat == null) shootStrat = GetComponent<ACharacterStrategy>();
        if (bullet == null) bullet = bulletObject.GetComponent<ABullet2D>();
        if (shootParams == null) shootParams = ScriptableObject.CreateInstance<ShootParams>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        timeToBullet = 1 / shootParams.fireRate;
        bulletTimer += Time.deltaTime;
    }

    private void FixedUpdate() {
        bool fireInput = shootStrat.FireThisFrame(bullet, shootParams);
        if (!fireInput) bulletTimer = 0;
        if (fireInput && bulletTimer > timeToBullet) {
            shootStrat.OnFire();
            bulletTimer = 0;

            float[] fireStreams = shootParams.customFireStreams;

            switch(shootParams.fireStreamType) {
                case FireStreamType.SingleStream:
                    fireStreams = new[] { 0f };
                    break;
                case FireStreamType.EvenSpacing:
                    fireStreams = new float[shootParams.numberOfStreams];
                    for (int i = 0; i < shootParams.numberOfStreams; i++) {
                        fireStreams[i] = (-shootParams.streamSpacing * shootParams.numberOfStreams / 2) + i * shootParams.streamSpacing;
                    }
                    break;
                case FireStreamType.Custom:
                    fireStreams = shootParams.customFireStreams; 
                    break;

            }

            Vector3 recoilDirection = Vector3.zero;
            foreach (float fireStreamOffset in fireStreams) {
                Vector3 spreadDirection = Quaternion.AngleAxis(Random.Range(-shootParams.fireSpread, shootParams.fireSpread) + shootStrat.FireAngle() + fireStreamOffset, transform.forward) * transform.right;
                Vector3 placePosition = transform.position + spreadDirection * shootParams.placeDistance;

                GameObject bo = Instantiate(bulletObject, placePosition, Quaternion.identity);
                bo.SetActive(true);
                bo.GetComponent<Rigidbody2D>().linearVelocity = inheritVelocityMultiplier * Mathf.Max(Vector3.Dot(rb.linearVelocity.normalized, spreadDirection), 0) * rb.linearVelocity + (Vector2) spreadDirection * shootParams.bulletSpeed; 
                recoilDirection += spreadDirection;
            }

            rb.AddForce(shootParams.recoilForce * rb.mass * -recoilDirection / Mathf.Min(fireStreams.Length, 1));
        }
    }
}
