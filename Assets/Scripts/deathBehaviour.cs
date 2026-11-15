using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class deathBehaviour : MonoBehaviour
{
    void OnCollisionEnter(Collision collision) // trigger on collision
    { 
        if(collision.gameObject.CompareTag("Ghost") && !powerUp.poweredUP){
            Debug.Log("Pacman died!");
            SceneManager.LoadScene(0); // restart scene
        }
    }
}
