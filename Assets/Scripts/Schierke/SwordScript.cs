using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class SwordScript : MonoBehaviour
{
    [SerializeField] Transform TargetTransform;
    [SerializeField] float offsety;
    [SerializeField] float offsetx;
    [SerializeField] float smoothTime = 0.3f;

    [Header("Enemy Detection / Attack")]
    [SerializeField] float detectRadius = 1f;
    [SerializeField] float attackCooldown = 1f;
    [SerializeField] float attackDuration = 0.2f;


    SpriteRenderer spriteRenderer;
    Transform swordTransform;
    Vector3 velocity;

    bool isAttacking = false;
    float lastAttackTime = -Mathf.Infinity;

    void Start()
    {
        swordTransform = this.transform;
        spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        velocity = new Vector3 (0, 0, 2);
    }

    void Update()
    {
        FollowTarget();
        DetectEnemies();
    }

    void FollowTarget()
    {
        Vector3 offsetVector = new Vector3(offsetx, offsety, 0);
        if (TargetTransform != null)
        {
            Vector3 targetPosition = TargetTransform.position + offsetVector;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
        if (spriteRenderer != null)
        {
            if (swordTransform.position.x > TargetTransform.position.x)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }
        }

    }

    void DetectEnemies()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, detectRadius, LayerMask.GetMask("Enemy"));
        foreach (var hit in hitEnemies)
        {
            if (hit != null && hit.CompareTag("Enemy"))
            {
                Debug.Log("Enemy Detected: " + hit.name);
                StartCoroutine(AttackRoutine(hit.transform));
                break;
            }
        }
    }

    IEnumerator AttackRoutine(Transform enemy)
    {
        lastAttackTime = Time.time;
        isAttacking = true;

        // simple lunge toward enemy then return
        Vector3 originalPos = transform.position;
        Vector3 direction = (enemy.position - transform.position).normalized;
        Vector3 attackPos = transform.position + direction * 0.5f; // adjust lunge distance as needed

        float elapsed = 0f;
        // lunge forward
        while (elapsed < attackDuration)
        {
            transform.rotation = quaternion.RotateZ(math.radians(360) * elapsed / attackDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = attackPos;

        // short pause at hit position
        yield return new WaitForSeconds(0.05f);

        // return to original (or let FollowTarget snap back on next frame)
        elapsed = 0f;
        while (elapsed < attackDuration)
        {
            transform.position = Vector3.Lerp(attackPos, originalPos, elapsed / attackDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = originalPos;

        isAttacking = false;
    }




    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}
