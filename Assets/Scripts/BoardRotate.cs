using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class BoardRotate : MonoBehaviour
{
    public Rigidbody rb;
    float xAngle = 0f;
    float zAngle = 0f;
    public float rotateAmnt = 5f;
    public float maxAngle = 30f;
    public static bool rotatingBoard = false;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)){
            rotatingBoard = true;
            if (Input.GetKey(KeyCode.W)){
                xAngle +=rotateAmnt;
                //Quaternion delta = Quaternion.Euler(1 * Vector3.right);
                //rb.MoveRotation(rb.rotation * delta);
            }
            if (Input.GetKey(KeyCode.S)){
                xAngle -=rotateAmnt;
                //Quaternion delta = Quaternion.Euler(-1 * Vector3.right);
                //rb.MoveRotation(rb.rotation * delta);
            }
            if (Input.GetKey(KeyCode.D)){
                zAngle -=rotateAmnt;
                //Quaternion delta = Quaternion.Euler(1 * Vector3.back);
                //rb.MoveRotation(rb.rotation * delta);
            }
            if (Input.GetKey(KeyCode.A)){
                zAngle +=rotateAmnt;
                //Quaternion delta = Quaternion.Euler(-1 * Vector3.back);
                //rb.MoveRotation(rb.rotation * delta);
            }
            // apply clamp
            xAngle = Mathf.Clamp(xAngle, -maxAngle, maxAngle);
            zAngle = Mathf.Clamp(zAngle, -maxAngle, maxAngle);
            // apply rotation
            Quaternion trgtRotation = Quaternion.Euler(xAngle,0,zAngle);
            rb.MoveRotation(trgtRotation);
        } else {
            if(zAngle != 0){
                zAngle = Mathf.MoveTowards(zAngle, 0, 2);
            }
            if(xAngle != 0){
                xAngle = Mathf.MoveTowards(xAngle, 0, 2);
            }
            // apply rotation
            Quaternion trgtRotation = Quaternion.Euler(xAngle,0,zAngle);
            rb.MoveRotation(trgtRotation);
        }
        if(transform.eulerAngles == Vector3.zero){
            rotatingBoard = false;
        }
    }
}
