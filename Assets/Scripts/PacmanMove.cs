using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class PacmanMove : MonoBehaviour
{   
    public Rigidbody rb;
    public float speed;
    public float gSpeed;
    public Transform model;
    public float bumpStrength;
    public float gravityStrength;
    public float maxSpeed = 60.60f;
    Vector3 pacmanCenter;
    public float inputHr;
    public float inputVr;
    public float gMaxSpeed;
    float inputH;
    float inputV;
    Vector3 moveStep;
    Vector3 direction;
    Vector3 velFlat;

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
        // Movement
        // apply speed
        if(grounded()){  // make it snappy
            // Get raw input
            Vector3 targetDir = new Vector3(inputHr, 0, inputVr).normalized;
            if (targetDir.sqrMagnitude > 0.1f) {
                // Instantly set velocity in the input direction (no gliding!)
                if(Mathf.Abs(inputHr) >= Mathf.Abs(inputVr)) {
                    rb.AddForce(new Vector3(inputHr * gSpeed * Time.deltaTime, 0, 0), ForceMode.VelocityChange);
                } 
                if(Mathf.Abs(inputHr) <= Mathf.Abs(inputVr)) {
                    rb.AddForce(new Vector3(0, 0, inputVr * gSpeed * Time.deltaTime), ForceMode.VelocityChange);
                }
            } else {
                // Instantly stop the moment you let go of the keys
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            }
            rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, gMaxSpeed); // clamp speed
            Debug.Log("Speed: "+rb.linearVelocity);
        } else { // let it lose
            rb.AddForce(direction * speed, ForceMode.Force); // Move pacman physics accurate
            
            // Fixed: Cache velocity into a temp variable, use Mathf.Clamp, and reassign
            Vector3 vel = rb.linearVelocity;
            vel.x = Mathf.Clamp(vel.x, -maxSpeed, maxSpeed);
            vel.z = Mathf.Clamp(vel.z, -maxSpeed, maxSpeed);
            rb.linearVelocity = vel;
        }

        // Gravity
        pacmanCenter = GetComponent<BoxCollider>().transform.TransformPoint(GetComponent<BoxCollider>().center);
        if(!grounded()){
            rb.AddForce(Vector3.down * gravityStrength, ForceMode.Acceleration);
        } else {
            rb.linearDamping = 5f; // aplicar atrito
            rb.AddForce(Vector3.down * gravityStrength, ForceMode.Acceleration);
            rb.AddForce(Vector3.up * gravityStrength, ForceMode.Acceleration);
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