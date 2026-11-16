using UnityEngine;

public class Ghost : MonoBehaviour
{
    public Transform target;
    public Rigidbody rb;
    public float speed; 
    public float gravityStrength;
    public float maxSpeed = 80f;
    Vector3 direction;

    void FixedUpdate()
    {
        // Gravity
        rb.AddForce(Vector3.down * gravityStrength, ForceMode.Acceleration);
        
        // Ghost movement logic
        if(!powerUp.poweredUP){
            direction = target.position - transform.position; // direction to target
            rb.AddForce(direction * speed, ForceMode.Force); // move ghost
            rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxSpeed); // cap velocity
        } else {
            direction =  transform.position - target.position; // direction away from target
            rb.AddForce(direction * speed, ForceMode.Force); // move ghost
            rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, 5); // cap more, slow ghosts down
        }
        // kill ghost
        if (pointSystem.totalPoints > 242){
            Destroy(gameObject);
        }


        /*
        Debug.DrawLine(
            direction,
            transform.position,
            Color.red,
            0.02f,
            false
        );
        */
    }
}
