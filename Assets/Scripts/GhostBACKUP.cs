using UnityEngine;

// change GhostBackup to Ghost for it to work
public class GhostBackup : MonoBehaviour
{
    public Transform target;
    public Rigidbody rb;
    public float speed; 
    public float gravityStrength;
    
    void FixedUpdate()
    {
        // Gravity
        rb.AddForce(Vector3.down * gravityStrength, ForceMode.Acceleration);
        
        // Ghost movement logic
        Vector3 direction = target.position - transform.position; // direction to target
        rb.AddForce(direction * speed, ForceMode.Force); // move ghost
    }
}
