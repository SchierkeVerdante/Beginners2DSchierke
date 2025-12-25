using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour {
    Rigidbody body, groundBody, prevGroundBody;
    float minGroundDotProduct;

    [SerializeField]
    float maxSpeed = 5, maxAcceleration = 20, maxAirAcceleration = 2;
    //[SerializeField, Range(0f, 90f)]
    //float groundContactAngle = 25f;
    //[SerializeField]
    //int maxAirJumps = 0;
    //[SerializeField]
    //bool angleJumps = true;
    [SerializeField]
    public bool canMove = true;


    [Header("Out (ReadOnly)")]

    [SerializeField]
    Vector3 targetVelocity, velocity;
    [SerializeField]
    bool onGround;
    [SerializeField]
    Transform target;

    private void Start() {
        body = GetComponent<Rigidbody>();
        target = LDirectory.Instance.player.transform;
        OnValidate();
    }

    void Update() {
        Vector3 targetDirection = target.transform.position - transform.position;
        targetVelocity = targetDirection * maxSpeed;
    }

    private void OnValidate() {
        //minGroundDotProduct = Mathf.Cos(groundContactAngle * Mathf.Deg2Rad);
    }

    private void FixedUpdate() {
        if (!canMove) {
            body.linearVelocity = Vector3.zero;
            return;
        }
        velocity = body.linearVelocity;
        //if (onGround) {
        //    jumpPhase = 0;
        //} else {
        //    contactNormal = Vector3.up;
        //    groundVelocity = Vector3.zero;
        //    groundBody = null;
        //}

        float acceleration = onGround ? maxAcceleration : maxAirAcceleration;
        float maxSpeedChange = acceleration * Time.fixedDeltaTime;
        velocity.x =
            Mathf.MoveTowards(velocity.x, targetVelocity.x, maxSpeedChange);
        velocity.z =
            Mathf.MoveTowards(velocity.z, targetVelocity.z, maxSpeedChange);

        body.linearVelocity = velocity;
        onGround = false;
    }

    private void OnCollisionEnter(Collision collision) {

    }

    private void OnCollisionStay(Collision collision) {
        for (int i = 0; i < collision.contactCount; i++) {
            Vector3 normal = collision.GetContact(i).normal;
            if (normal.y >= minGroundDotProduct) {
                onGround = true;
                //if (angleJumps)
                //    contactNormal = normal;
                groundBody = collision.rigidbody;
            }
        }
    }
}
