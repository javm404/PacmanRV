using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
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
    // gamepad
    private InputAction RotateBoard;
    private Vector2 stickInpt;
    void Start()
    {
        rb = GetComponent<Rigidbody>();

    }
    // avoid input leakage
    void OnEnable()
    {
	    InputSystem.actions.FindActionMap("Player").Enable();
    }
    void OnDisable()
    {
	    InputSystem.actions.FindActionMap("Player").Disable();
    }
    // gamepad input too
    void Awake()
    {
    	RotateBoard = InputSystem.actions.FindAction("RotateBoard");
<<<<<<< HEAD
	stickInpt = RotateBoard.ReadValue<Vector2>();
=======
	    stickInpt = RotateBoard.ReadValue<Vector2>();
>>>>>>> dev
    }
    void Update()
    {
    }
    // keyboard input
    void FixedUpdate()
    {
	stickInpt = RotateBoard.ReadValue<Vector2>();
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || stickInpt != Vector2.zero){
            rotatingBoard = true;
	    
            // keyboard
	    if (Input.GetKey(KeyCode.W)){
                xAngle +=rotateAmnt;
            }
            if (Input.GetKey(KeyCode.S)){
                xAngle -=rotateAmnt;
            }
            if (Input.GetKey(KeyCode.D)){
                zAngle -=rotateAmnt;
            }
            if (Input.GetKey(KeyCode.A)){
                zAngle +=rotateAmnt;
            }

	    // gamepad logic
	    if(stickInpt != Vector2.zero){
	    	zAngle -=rotateAmnt*stickInpt.x;
	    	xAngle +=rotateAmnt*stickInpt.y;
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
