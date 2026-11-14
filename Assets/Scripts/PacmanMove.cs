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


    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Gravity
        rb.AddForce(Vector3.down * gravityStrength, ForceMode.Acceleration);
        // Movement
        float inputH = Input.GetAxis("Horizontal");
        float inputV = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(inputH, 0, inputV); // Get player input
        //rb.linearVelocity =  direction * speed; // Move pacman
        rb.AddForce(direction * speed, ForceMode.Force); // Move pacman physics accurate
        //if (direction != Vector3.zero){ // Rotate model
        //    model.rotation = (Quaternion.LookRotation(-1*direction));
        //}
        rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxSpeed); // clamp speed
    }

    // physics, add counter force when hitting wall
    void OnCollisionEnter(Collision collision)
    {
        Vector3 bumpDirection = collision.contacts[0].normal;
        rb.AddForce(bumpDirection * bumpStrength, ForceMode.Force);
    }

}