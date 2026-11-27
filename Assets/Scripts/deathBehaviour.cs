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
    public static bool respawning;
    public Rigidbody rb;
    public bool deadOnStart = true;
    public GameObject boardBounds;
    public scoreCount scoreCount; // to fetch scoreCount UpdateScoreCounter() method
    void Start()
    {
        UpdateLifeCounter();
        respawning = false;
    }

    void FixedUpdate()
    {
        if(respawning){
            rb.AddForce(Vector3.down * 400f, ForceMode.Acceleration); // add gravity counter force
            rb.AddForce(Vector3.up * 20f, ForceMode.Acceleration); // add a slight counter force, to ease landing
            active = false;
        }
	if(deadOnStart){
		//remove life counter
                lives -= 1;
                // tp player
                StartCoroutine("Teleport");
		deadOnStart = false;
                // UI move 1 life UPWARDS
                Rigidbody lifeRb = lifes[lives].GetComponentInChildren<Rigidbody>();
                lifeRb.useGravity = true;
                lifeRb.linearVelocity = Vector3.zero;
                lifeRb.AddForce(Vector3.up * 700, ForceMode.Impulse); // Move life physics accurate
                // TODO Destroy that life's gameObject
                UpdateLifeCounter(); // update counter
	}
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
				
                        // tp player
                        StartCoroutine("Teleport");
                        // UI move 1 life UPWARDS
                        Rigidbody lifeRb = lifes[lives].GetComponentInChildren<Rigidbody>();
                        lifeRb.useGravity = true;
                        lifeRb.linearVelocity = Vector3.zero;
                        lifeRb.AddForce(Vector3.up * 700, ForceMode.Impulse); // Move life physics accurate
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
    IEnumerator Teleport(){
        //disable movment
        moveScript.enabled = false;
        active = false; // disable death
        boardBounds.SetActive(false); // disable board bounds
        yield return new WaitForSeconds(0.05f);
        respawning = true;
        // teleport pacman
        rb.position = new Vector3(14.0f , 1400.0f, 0.0f);
        Vector3 vel = Vector3.zero;
        rb.linearVelocity = vel;
        yield return new WaitForSeconds(0.5f);
        // re-enable movement
        moveScript.enabled = true;
    }
}

