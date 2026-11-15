using UnityEngine;

public class completlyOutOfBounds : MonoBehaviour
{
    void OnTriggerExit(Collider collision)
    {
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        if(collision.gameObject.CompareTag("Player")){
            Debug.Log("Player is completly out of bounds");
            rb.position = new Vector3(14.0f , 1200.0f, 0.0f);            
            rb.linearVelocity *= 200 ;

        } else if(collision.gameObject.CompareTag("Ghost")){
            Debug.Log("Ghost is completly out of bounds!");
        }
    }

}
