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
        if (powerUp.poweredUP){
            rb.AddForce(Vector3.down * 600, ForceMode.Acceleration);
        } else {
            rb.AddForce(Vector3.down * gravityStrength, ForceMode.Acceleration);
        }
        // Ghost movement logic
        if(!powerUp.poweredUP){
            Vector3 direction = target.position - transform.position; // direction to target
            rb.AddForce(direction * speed, ForceMode.Force); // move ghost
            rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxSpeed); // cap velocity
        } else {
            rb.AddForce(Vector3.down * 100, ForceMode.AddForce);
            Vector3 direction =  transform.position - target.position; // direction away from target
            rb.AddForce(direction * speed/2, ForceMode.Force); // move ghost
            rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, 5); // cap more, slow ghosts down
        }
        // kill ghost
        if (pointSystem.totalPoints > 242){
            Destroy(gameObject);
        }
        /*
        Debug.DrawLine(
            target.position,
            transform.position,
            Color.pink,
            0.02f,
            false
        );
        */
    }
}
