using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class deathBehaviour : MonoBehaviour
{
    public bool active = true;
    public int lives = 2;
    public GameObject[] lifes;
    public TextMeshPro lifeCounterText;
    public PacmanMove moveScript;
    public Rigidbody rb;
    public scoreCount scoreCount; // to fetch scoreCount UpdateScoreCounter() method
    void Start()
    {
        UpdateLifeCounter();
    }
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
                        Debug.Log("Game over!");
                        // reset score
                        scoreCount.score = 0;
                        scoreCount.UpdateScoreCounter();
                        // restart scene
                        SceneManager.LoadScene(0); 
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
                        // TODO Destroy that life's gameObject
                        UpdateLifeCounter(); // update counter

                        // divide score
                        scoreCount.score = scoreCount.score / 2;
                        scoreCount.UpdateScoreCounter();
                    }
                }
                else if(powerUp.poweredUP)
                {
                    ghost.tagged = true;
                }
            }
        }
    }
    void UpdateLifeCounter()
    {
        if (lifeCounterText != null)
        {
            lifeCounterText.text = "X" + lives;
        }
    }
}

