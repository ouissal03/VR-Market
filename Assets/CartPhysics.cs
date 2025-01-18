using UnityEngine;

public class CartPhysics : MonoBehaviour
{
    public float maxSpeed = 5f;         // Maximum speed the cart can go
    public float friction = 0.1f;       // Friction applied when the cart is not being pushed
    public float pushForce = 10f;       // Force applied when the user pushes the cart
    public float rotationTorque = 50f;  // Torque applied to rotate the cart when pushing

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Ensure the Rigidbody's angular drag is high enough to slow down unwanted rotation
        rb.angularDamping = 2f;
    }

    void FixedUpdate()
    {
        // Get the input for pushing the cart forward/backward (for now using keyboard input as an example)
        float pushInput = Input.GetAxis("Vertical"); // Replace with VR input (e.g., forward movement)
        float rotationInput = Input.GetAxis("Horizontal"); // Replace with VR input (e.g., turning)

        // Apply a force for moving forward/backward (relative to the cart's orientation)
        if (pushInput != 0)
        {
            // Forward/backward movement based on the cart's orientation
            Vector3 forwardDirection = transform.forward;  // Get the cart's local forward direction
            rb.AddForce(forwardDirection * pushInput * pushForce, ForceMode.Acceleration);
        }

        // Apply rotational torque to simulate turning the cart (use right direction for turning)
        if (rotationInput != 0)
        {
            // Apply torque to rotate the cart around the Y-axis (vertical axis)
            rb.AddTorque(Vector3.up * rotationInput * rotationTorque, ForceMode.Acceleration);
        }

        // Clamp the cart's speed to avoid unrealistic movement
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }

        // Apply manual friction when the cart is not being pushed
        if (pushInput == 0 && rb.linearVelocity.magnitude > 0.01f)
        {
            // Gradually reduce the cart's velocity over time to simulate friction
            rb.linearVelocity *= (1 - friction);
        }
    }
}
