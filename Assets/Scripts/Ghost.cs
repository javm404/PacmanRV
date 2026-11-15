using UnityEngine;

public class Ghost : MonoBehaviour
{
    public Transform target;
    public Rigidbody rb;
    public float speed; 
    public float gravityStrength;
    public float maxSpeed = 80f;

    void FixedUpdate()
    {
        // Gravity
        rb.AddForce(Vector3.down * gravityStrength, ForceMode.Acceleration);
        
        // Ghost movement logic
        Vector3 direction = target.position - transform.position; // direction to target
        rb.AddForce(direction * speed, ForceMode.Force); // move ghost
        rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxSpeed); // cap velocity
    
        // kill ghost
        if (pointSystem.totalPoints > 242){
            Destroy(gameObject);
        }
        Debug.DrawLine(
            target.position,
            transform.position,
            Color.pink,
            0.02f,
            false
        );
    }
}
