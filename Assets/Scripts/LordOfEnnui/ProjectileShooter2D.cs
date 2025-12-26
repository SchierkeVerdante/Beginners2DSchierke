using UnityEngine;

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

    [SerializeField]
    float placeDistance = 0.5f;

    [Header("ReadOnly")]
    [SerializeField]
    float timeToBullet, bulletTimer = 0;

    [SerializeField]
    Vector3 fireDirection, spreadDirection, placePosition;

    
    void Start()
    {
        if (shootStrat == null) shootStrat = GetComponent<ACharacterStrategy>();
        if (bullet == null) bullet = bulletObject.GetComponent<ABullet2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        timeToBullet = 1 / bullet.fireRate;
        fireDirection = shootStrat.GetFireDirection();
        bulletTimer += Time.deltaTime;
    }

    private void FixedUpdate() { 
        if (shootStrat.FireThisFrame(bullet) && bulletTimer > timeToBullet) {
            bulletTimer = 0;

            Quaternion randomOffset = Quaternion.AngleAxis(Random.Range(-bullet.fireSpread, bullet.fireSpread), transform.forward);
            spreadDirection = randomOffset * fireDirection;
            placePosition = transform.position + spreadDirection * placeDistance;

            GameObject bo = Instantiate(bulletObject, placePosition, Quaternion.identity);
            bo.SetActive(true);
            bo.GetComponent<Rigidbody2D>().linearVelocity = spreadDirection * bullet.bulletSpeed;
            rb.AddForce(-spreadDirection * bullet.recoilForce);
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
