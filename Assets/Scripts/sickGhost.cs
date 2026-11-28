using UnityEngine;

public class sickGhost : MonoBehaviour
{
    public Material deadGhostMaterial;
    public Rigidbody rb;
    public GhostBase ghostScript;
    void FixedUpdate()
    {
        if(powerUp.poweredUP){
            //slow ghost down
            if(!ghostScript.tagged){
                rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, 20); // clamp speed
                // add even more gravity
                rb.AddForce(Vector3.down * 200f, ForceMode.Acceleration);
            } else {
                Debug.Log(name+" is tagged, no longer slowed");
            }
        }
    }
}
