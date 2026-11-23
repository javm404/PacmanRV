using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class PacmanMove : MonoBehaviour
{   
    public Rigidbody rb;
    public float speed = 100f;
    public float groundedSpeed = 500f;
    public float maxSpeed = 100f;
    public float groundedMaxSpeed = 500f;
    public Transform model;
    public float bumpStrength;
    public float gravityStrength;
    static public Vector3 centerAxis;
    Vector3 directionR;
    Vector3 direction;
    float inputHr;
    float inputVr;
    float inputH;
    float inputV;
    // raycast for groundcheck
    public LayerMask groundLayer;
    public float groundCheckDistance = 7.0f; // tune to Pacman's collider size
    public float groundCheckRadius = 7.0f;
    bool grounded()
    {
 	return Physics.OverlapSphere(centerAxis + Vector3.down * groundCheckDistance, groundCheckRadius, groundLayer).Length > 0; // Physics.OverlapSphere returns an Array of information on the object it hit, if there's something it the array (if > 0) then it hit something! :D
    } 	
    public bool instantStop = true;
    /*
    RaycastHit gHit;
    bool willHitGround(){return Physics.SphereCast(centerAxis, groundCheckRadius, Vector3.down, out gHit, groundCheckDistance, groundLayer);}
    */
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Update(){
        // get player input loose
        inputH = Input.GetAxis("Horizontal");
        inputV = Input.GetAxis("Vertical");
        // get player input tight
        inputHr = Input.GetAxisRaw("Horizontal");
        inputVr = Input.GetAxisRaw("Vertical");
	directionR = new Vector3(inputHr, 0, inputVr).normalized; // Get p. raw inpt
        direction = new Vector3(inputH, 0, inputV); // Get player input
        centerAxis = GetComponent<BoxCollider>().transform.TransformPoint(GetComponent<BoxCollider>().center);
	Debug.DrawLine(centerAxis, centerAxis + rb.linearVelocity, Color.red, 0.1f, false);
    }
    void FixedUpdate()
    {

        // Gravity
        if(rb.linearVelocity.y > 0.3f && !grounded()){ // going up
            Debug.Log("going up!");
            rb.AddForce(Vector3.down * gravityStrength * 1f, ForceMode.Acceleration);
        } else if(!grounded()){ // going down
            Debug.Log("going down");
            rb.AddForce(Vector3.down * gravityStrength * 3f, ForceMode.Acceleration);
        } else {
            rb.AddForce(Vector3.down * gravityStrength * 1f, ForceMode.Acceleration);
        }


        // Movement
        if(grounded()){ // make it tight
            if(directionR.sqrMagnitude > 0.1f){ // player is pressing keys
                //Debug.Log("Grounded");
                // define damping level
                if(BoardRotate.rotatingBoard){
                    rb.linearDamping = 1f;
                } else {
                    rb.linearDamping = 5f;
                }
                // Make pacman move
                if(Mathf.Abs(inputHr) >= Mathf.Abs(inputVr)) // is pressing Horizontal keys 
                {
                    rb.AddForce(directionR * groundedSpeed, ForceMode.Force);
                    //rb.AddForce(new Vector3(inputHr * gSpeed * Time.deltaTime, 0, 0), ForceMode.VelocityChange);
                }
                if(Mathf.Abs(inputVr) >= Mathf.Abs(inputHr))
                {
                    rb.AddForce(directionR * groundedSpeed, ForceMode.Force);
                    //rb.AddForce(new Vector3(0, 0, inputVr * gSpeed * Time.deltaTime), ForceMode.VelocityChange);

                }
            } else if(instantStop && !BoardRotate.rotatingBoard){ // stop player movement completly
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
                rb.linearDamping = 5f;
            } else {
                rb.linearDamping = 1f;
            }
        }else{ // let it loose
            //Debug.Log("Airbone");	
            rb.linearDamping = 1f; // reduce "atrito"
            rb.AddForce(direction * speed, ForceMode.Force); // Move pacman physics accurate
        }	
	//Debug.Log("Y: " + transform.position.y + " | Grounded: " + grounded());
	//rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxSpeed); // clamp speed
    }

    // physics, add counter force when hitting wall
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Wall")){
            Vector3 bumpDirection = collision.contacts[0].normal;
            rb.AddForce(bumpDirection * bumpStrength, ForceMode.Force);
        }
    }
    // DEBUG
    void OnDrawGizmos() {
        Gizmos.color = Color.blue;
	    //Gizmos.DrawWireSphere(castOrigin, groundCheckRadius);
	    Gizmos.DrawWireSphere(centerAxis + Vector3.down * groundCheckDistance, groundCheckRadius);
        // draw wire sphere at origin, then another at origin + down*distance
    }
}
