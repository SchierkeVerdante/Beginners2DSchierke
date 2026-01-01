using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ProjectileShooter2D : MonoBehaviour
{
    [SerializeField]
    AEnemyStrategy shootStrat;
    
    [SerializeField]
    GameObject bulletObject;

    [SerializeField]
    ABullet2D bullet;

    [SerializeField]
    AMove2D currentMove;

    [SerializeField]
    Rigidbody2D rb;

    [SerializeField]
    float placeDistance = 0.5f, fireRate = 3f, fireSpread = 10f;

    [SerializeField, Range(-180, 180)]
    float[] fireStreams = new[] { 0f };

    [Header("ReadOnly")]
    [SerializeField]
    float timeToBullet, bulletTimer = 0;



    void Start()
    {
        if (shootStrat == null) shootStrat = GetComponent<AEnemyStrategy>();
        if (bullet == null) bullet = bulletObject.GetComponent<ABullet2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        timeToBullet = 1 / fireRate;
        bulletTimer += Time.deltaTime;
    }

    private void FixedUpdate() { 
        if (shootStrat.FireThisFrame(bullet) && bulletTimer > timeToBullet) {
            bulletTimer = 0;

            foreach (float fireStreamOffset in fireStreams) {
                Vector3 spreadDirection = Quaternion.AngleAxis(Random.Range(-fireSpread, fireSpread) + shootStrat.facingAngle + fireStreamOffset, transform.forward) * transform.right;
                Vector3 placePosition = transform.position + spreadDirection * placeDistance;

                GameObject bo = Instantiate(bulletObject, placePosition, Quaternion.identity);
                bo.SetActive(true);
                bo.GetComponent<Rigidbody2D>().linearVelocity = spreadDirection * bullet.bulletSpeed;
                rb.AddForce(-spreadDirection * bullet.recoilForce * rb.mass);
            }
        }
    }

    //private IEnumerator FireBullets() {
    //    WaitForSeconds wait = new WaitForSeconds(1 / bullet.fireRate);

    //    while (true) {
    //        if (fire) {
    //            Vector3 targetDirection = (target.transform.position - transform.position).normalized;

    //            Vector3 placePosition = transform.position + targetDirection * placeDistance;
    //            GameObject bo = Instantiate(bulletObject, placePosition, Quaternion.identity);
    //            bo.GetComponent<Rigidbody2D>().linearVelocity = targetDirection * bullet.bulletSpeed;
    //            rb.AddForce(-targetDirection * bullet.recoilForce);
    //        }

    //        yield return wait;
    //    }
    //}
}
