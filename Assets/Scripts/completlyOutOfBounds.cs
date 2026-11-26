using UnityEngine;

public class completlyOutOfBounds : MonoBehaviour
{
    void OnTriggerExit(Collider collision)
    {
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
       	// if(collision.gameObject.CompareTag("Player")){
       	//     Debug.Log("Player is completly out of bounds");
       	//     powerUp.poweredUP = true; // reward the player for managing to get out of bounds 
       	//     rb.position = new Vector3(14.0f , 1200.0f, 0.0f);
       	//     Vector3 vel = Vector3.zero;
       	//     vel.x = 0;
       	//     vel.z = 0;        
       	//     rb.linearVelocity = vel;

       	// }
	if(collision.gameObject.CompareTag("Ghost")){
            Debug.Log("Ghost is completly out of bounds!");
	    rb.position = new Vector3(14f, 1000f, 0f);
        }
    }

}
