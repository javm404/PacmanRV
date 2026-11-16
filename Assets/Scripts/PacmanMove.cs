using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class PacmanMove : MonoBehaviour
{   
    public Rigidbody rb;
    public float speed;
    public Transform model;
    public float bumpStrength;
    public float gravityStrength;
    public float maxSpeed = 60.60f;
    bool opposing = false;

    Vector3 pacmanCenter; 
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.7f;
    bool isNearGround(){return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);}

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pacmanCenter = GetComponentInChildren<BoxCollider>().transform.TransformPoint(GetComponentInChildren<BoxCollider>().center);
    }

    void FixedUpdate()
    {
        // Gravity
        rb.AddForce(Vector3.down * gravityStrength, ForceMode.Acceleration);
        // Movement
        float inputH = Input.GetAxis("Horizontal");
        float inputV = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(inputH, 0, inputV); // Get player input
        rb.AddForce(direction * speed, ForceMode.Force); // Move pacman physics accurate
        // Counter force
        Vector3 velFlat = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        float opposition = Vector3.Dot(direction.normalized, velFlat.normalized);

        if(opposition < -0.1f){ // always apply counterfoce when player wants to change input 
            // direction is facing opposite of force, apply counter force
            rb.AddForce(direction * speed * 10, ForceMode.Force);
            Debug.Log("Applying force: "+rb.linearVelocity);
            opposing = true;
        } else {
            opposing = false;
        }
        //if (direction != Vector3.zero){ // Rotate model
        //    model.rotation = (Quaternion.LookRotation(-1*direction));
        //}
        //rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxSpeed); // clamp speed

        // DEBUG
        Debug.DrawLine(
            pacmanCenter, // from
            pacmanCenter + direction *3f, // to
            Color.red, 0.05f, false
        );
        Debug.DrawLine(
            transform.position,
            transform.position + Vector3.down * groundCheckDistance,
            Color.blue, 0.5f, false
        );
    }

    // physics, add counter force when hitting wall
    void OnCollisionEnter(Collision collision)
    {
        Vector3 bumpDirection = collision.contacts[0].normal;
        rb.AddForce(bumpDirection * bumpStrength, ForceMode.Force);
    }
}