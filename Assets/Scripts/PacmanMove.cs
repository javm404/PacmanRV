using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class PacmanMove : MonoBehaviour
{   
    public Rigidbody rb;
    public float speed;
    public float maxSpeed = 60.60f;
    public float gSpeed;
    public float gMaxSpeed;
    public Transform model;
    public float bumpStrength;
    public float gravityStrength;
    Vector3 pacmanCenter;
    public float inputHr;
    public float inputVr;

    public Vector3 position;
    public Vector3 pastPosition;
    public float pastFrameCount;

    float inputH;
    float inputV;
    Vector3 moveStep;
    Vector3 direction;
    Vector3 velFlat;
    bool wasGrounded;

    // raycast for groundcheck
    public LayerMask groundLayer;
    public float groundCheckDistance = 7f; // tune to Pacman's collider size
    bool grounded(){return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);}

    // variables to check walls
    public LayerMask wallLayer;
    BoxCollider col;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<BoxCollider>();
        pastFrameCount = 2;
    }

    void Update()
    {
        // Movement input
        inputH = Input.GetAxis("Horizontal");
        inputV = Input.GetAxis("Vertical");
        inputHr = Input.GetAxisRaw("Horizontal");
        inputVr = Input.GetAxisRaw("Vertical");
        direction = new Vector3(inputH, 0, inputV); // Get player input    
    }


    void FixedUpdate()
    {
        pacmanCenter = GetComponent<BoxCollider>().transform.TransformPoint(GetComponent<BoxCollider>().center);            
        // Gravity
        float vy = rb.linearVelocity.y;
        float fallThreshold = 2f; 
        // CLAUDE: bigger threshold (was 0.1) so ground jitter doesn't register as rising/falling

        if (grounded() && Mathf.Abs(vy) < fallThreshold) {
            Debug.Log("grounded");
            rb.AddForce(Vector3.down * gravityStrength, ForceMode.Acceleration);
            // CLAUDE: new branch — checks grounded() + near-zero velocity together, takes priority over the velocity-only checks below
        } else if (vy < -fallThreshold) {
            Debug.Log("falling");
            rb.AddForce(Vector3.down * gravityStrength * 1.4f, ForceMode.Acceleration);
        } else if (vy > fallThreshold) {
            Debug.Log("rising");
            rb.AddForce(Vector3.down * gravityStrength * 0.8f, ForceMode.Acceleration);
        } else {
            Debug.Log("apex (airborne, near-zero velocity)");
            // CLAUDE: new catch-all — airborne but vy within threshold, e.g. jump apex
            rb.AddForce(Vector3.down * gravityStrength, ForceMode.Acceleration);
        }

        // Movement
        // apply speed
        if(grounded()){  // make it snappy
            // Get raw input
            Debug.Log("Grounded");
            Vector3 targetDir = new Vector3(inputHr, 0, inputVr).normalized;
            if (targetDir.sqrMagnitude > 0.1f) {
                // Instantly set velocity in the input direction (no gliding!)
                if(Mathf.Abs(inputHr) >= Mathf.Abs(inputVr)) { // Horizontal axis inpt value is higher than Vertical axis input
                    rb.AddForce(new Vector3(inputHr * gSpeed * Time.deltaTime, 0, 0), ForceMode.VelocityChange);
                } 
                if(Mathf.Abs(inputHr) <= Mathf.Abs(inputVr)) { // Vertical axis inpt value is higher than Horizontal axis input
                    rb.AddForce(new Vector3(0, 0, inputVr * gSpeed * Time.deltaTime), ForceMode.VelocityChange);
                }
            } else {
                // Instantly stop the moment you let go of the keys
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            }
            rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, gMaxSpeed); // clamp speed
            //Debug.Log("Speed: "+rb.linearVelocity);
            wasGrounded = true;
        } else { // let it lose
            rb.AddForce(direction * speed, ForceMode.Force); // Move pacman physics accurate
            // Fixed: Cache velocity into a temp variable, use Mathf.Clamp, and reassign
            Vector3 vel = rb.linearVelocity;
            vel.x = Mathf.Clamp(vel.x, -maxSpeed, maxSpeed);
            vel.z = Mathf.Clamp(vel.z, -maxSpeed, maxSpeed);
            rb.linearVelocity = vel;
        }

        Debug.DrawLine(
            pacmanCenter,
            pacmanCenter + Vector3.down * groundCheckDistance,
            Color.blue, 0.5f, false
        );
        Debug.DrawLine(
            pacmanCenter,
            pacmanCenter + direction * 3f,
            Color.yellow,0.05f, false
        );
    }
    
    // physics, add counter force when hitting wall
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Wall")){
            Vector3 bumpDirection = collision.contacts[0].normal;
            rb.AddForce(bumpDirection * bumpStrength, ForceMode.Force);
        }
    }
    void OnCollisionStay(Collision collision)
    {
        rb.linearDamping = 6f; // aplicar atrito
    }
    void OnCollisionExit(Collision collision)
    {
        rb.linearDamping = 1f;
    }
}