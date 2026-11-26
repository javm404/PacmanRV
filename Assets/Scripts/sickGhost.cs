using UnityEngine;

public class sickGhost : MonoBehaviour
{
    public Material deadGhostMaterial;
    public Rigidbody rb;
    void FixedUpdate()
    {
        if(powerUp.poweredUP && GetComponentInChildren<Renderer>().material != deadGhostMaterial){
            //slow ghost down
            rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, 20); // clamp speed
        }
    }
}
