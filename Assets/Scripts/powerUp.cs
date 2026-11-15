using UnityEngine;

public class powerUp : MonoBehaviour
{
    void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.CompareTag("PowerUP")){
            Destroy(collision.gameObject);
            Debug.Log("pacman hit power up!");
        }
    }
}
