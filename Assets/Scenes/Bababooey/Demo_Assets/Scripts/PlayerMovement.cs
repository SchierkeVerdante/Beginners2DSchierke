using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float baseSpeed = 5f;

    Rigidbody2D rb;
    Vector2 input;
    float speedMultiplier = 1f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Called automatically by Input System
    public void OnMove(InputAction.CallbackContext context)
    {
        input = context.ReadValue<Vector2>();
        Debug.Log("erro govnah");
    }

    void FixedUpdate()
    {
        rb.linearVelocity = input.normalized * baseSpeed * speedMultiplier;
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        speedMultiplier = multiplier;
    }
}
