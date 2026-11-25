using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class deathBehaviour : MonoBehaviour
{
    public bool active = true;
    public int lives = 2;
    public GameObject[] lifes;
    public PacmanMove moveScript;
    public Rigidbody rb;
    
    void OnCollisionEnter(Collision collision) // trigger on collision
    { 
        if (active && collision.gameObject.CompareTag("Ghost"))
        {
            GhostBase ghost = collision.gameObject.GetComponent<GhostBase>();
            if (ghost != null && !ghost.tagged)
            {
                if(!powerUp.poweredUP)
                {
                    if(lives <=0){
                        Debug.Log("Pacman died!");
                        SceneManager.LoadScene(0); // restart scene
                    } else {
                        //remove life counter
                        lives -= 1;
                        // teleport pacman
                        rb.position = new Vector3(14.0f , 2000.0f, 0.0f);
                        Vector3 vel = Vector3.zero;
                        vel.x = 0;
                        vel.z = 0;        
                        rb.linearVelocity = vel;
                        // UI move 1 life UPWARDS
                        Rigidbody lifeRb = lifes[lives].GetComponent<Rigidbody>();
                        lifeRb.useGravity = true;
                        lifeRb.AddForce(new Vector3(0.5f,1f,0.5f) * 400, ForceMode.Impulse); // Move pacman physics accurate
                        // Destroy that life

                    }
                }
                else if(powerUp.poweredUP)
                {
                    ghost.tagged = true;
                }
            }
        }
    }
}

