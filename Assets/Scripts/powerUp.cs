using UnityEngine;

public class powerUp : MonoBehaviour
{
    public float time = 5.0f;
    public float timer;
    public static bool poweredUP = false;
    float count;
    Renderer[] pacmanBody;
    Material[] pastMaterial;
    public Material powerUpMaterial;
    public Light powerUpAura; // drag a Light object (child of Pacman) in Inspector, start disabled
    GameObject[] ghosts; // change ghosts texture (safer here, on Ghosts.cs FixedUpdate would be agressive)
    Material[] ghostsPastMaterial;
    public Material sickGhostMaterial;

    void Start()
    {
        poweredUP = false;
        powerUpAura.enabled = false;
        timer = time;
        pacmanBody = GetComponentsInChildren<Renderer>();
        int index = 0;
        pastMaterial = new Material[pacmanBody.Length]; // add array slots
        foreach (Renderer bodyPart in pacmanBody){
            pastMaterial[index] = bodyPart.material; // add masterial to slot
            index++;
        }
        // for ghosts
        ghosts = GameObject.FindGameObjectsWithTag("Ghost"); // fetch every ghost in game (once)
        Debug.Log(ghosts.Length);
        Debug.Log("Picked up ghost: "+ghosts);
        ghostsPastMaterial = new Material[ghosts.Length];
        int i = 0;
        foreach(GameObject ghost in ghosts){
            ghostsPastMaterial[i] = ghost.GetComponentInChildren<Renderer>().material;
            i++;
        }

    }

    void OnTriggerEnter(Collider collision)
    {
        //Debug.Log(collision.gameObject.name);
        if(collision.gameObject.CompareTag("PowerUP")){
            Destroy(collision.gameObject);
            Debug.Log("pacman hit power up!");
            poweredUP = true;
            powerUpAura.enabled = true;
            foreach(GameObject ghost in ghosts){
                ghost.GetComponentInChildren<Renderer>().material = sickGhostMaterial;
                Debug.Log(ghost.name+" is sick!");
            }
        } else if(collision.gameObject.CompareTag("Ghost") && poweredUP){
            // tag ghost as killed
            Destroy(collision.gameObject); // Destroy causes error and its not that fun, added just for testing
        }
    }

    void OnTriggerExit(Collider collision)
    {
        if(collision.gameObject.CompareTag("gameBounds")){
            foreach(GameObject ghost in ghosts){
                ghost.GetComponentInChildren<Renderer>().material = sickGhostMaterial;
                Debug.Log(ghost.name+" is sick!");
            }
        }
    }

    void FixedUpdate()
    {
        if(poweredUP && timer > 0.0f) {
            timer -= Time.deltaTime;
            Debug.Log("Time left for power up: "+timer);
            GetComponent<deathBehaviour>().enabled = false;
            // loop through body parts
            foreach(Renderer bodyPart in pacmanBody){
                bodyPart.material = powerUpMaterial;
            }
        } else if(timer != time){
            poweredUP = false;
            powerUpAura.enabled = false;
            timer = time;
            Debug.Log("PowerUP ended!");
            GetComponent<deathBehaviour>().enabled = true;
            // loop through body parts and apply past textures
            int i = 0;
            foreach(Renderer bodyPart in pacmanBody){
                bodyPart.material = pastMaterial[i];
                i++;
            }
            int j = 0;
            foreach(GameObject ghost in ghosts){
                ghost.GetComponentInChildren<Renderer>().material = ghostsPastMaterial[j];
                j++;
            }
        }
    }
}
