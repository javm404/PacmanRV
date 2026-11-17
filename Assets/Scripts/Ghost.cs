using UnityEngine;

public class Ghost : MonoBehaviour
{
    public Transform target;
    public Rigidbody rb;
    public float speed; 
    public float gravityStrength;
    public float maxSpeed = 80f;
    public bool tagged = false;
    public float reviveTime = 4.0f;
    public float timeLeft; 
    bool countdown = false;
    Vector3 direction;
    public Material normalMaterial;
    public Material deadGhostMaterial;
    public Material sickGhostMaterial;

    void FixedUpdate()
    {
        // Gravity
        rb.AddForce(Vector3.down * gravityStrength, ForceMode.Acceleration);
        
        // Ghost movement logic
        if(tagged){ // ghost died, go back to checkpoint
            direction = new Vector3(20f,8f,-4f) - transform.position; // TOWARDS checkpoint
            rb.AddForce(direction * speed/2, ForceMode.Force); // move ghost
            rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, 20); // cap velocity
        } else if(!powerUp.poweredUP && !tagged){ // player is powered up
            direction = target.position - transform.position; // direction TOWARDS target
            rb.AddForce(direction * speed, ForceMode.Force); // move ghost
            rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxSpeed); // cap velocity
        } else { // ghost is sick
            direction =  transform.position - target.position; // direction AWAY from target
            rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, 15); // cap more, slow ghosts down
            rb.AddForce(direction * speed, ForceMode.Force); // move ghost
            rb.AddForce(Vector3.down * gravityStrength * 5, ForceMode.Acceleration);
        } 


        // ghost texture states
        if(tagged){
            GetComponentInChildren<Renderer>().material = deadGhostMaterial;
        } else if(powerUp.poweredUP){
            GetComponentInChildren<Renderer>().material = sickGhostMaterial;
        } else {
            GetComponentInChildren<Renderer>().material = normalMaterial;
        }
        
        if(countdown && tagged && timeLeft > 0.0f){
            timeLeft -= Time.deltaTime;
        } else if(tagged && countdown){
            timeLeft = reviveTime;
            countdown = false;
            tagged = false;
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
    void OnTriggerEnter(Collider other)
    {
        if(tagged && other.CompareTag("Spawn")){
            Debug.Log("Spawning ghost "+name);
            countdown = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if(tagged && other.CompareTag("Spawn")){
            Debug.Log(name+" left area! Reseting respawn time");
            countdown = false;
            timeLeft = reviveTime;
        }
    }
}
