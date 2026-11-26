using UnityEngine;
public class GhostRED : MonoBehaviour
{
    public Transform pacman;
    public Transform spawn;
    public Vector3 mission;
    //private Vector3 target;
    public Rigidbody rb;
    public float speed; 
    public float gravityStrength;
    public float maxSpeed = 80f;
    public bool tagged = false;
    public float reviveTime = 4.0f;
    public float timeLeft; 
    bool countdown = false;
    Vector3 direction;
    float distance;
    public Material normalMaterial;
    public Material deadGhostMaterial;
    public Material sickGhostMaterial;
    //public enum ghostState { deciding, moving }
    //public ghostState state;
    public bool deciding = true;
    public bool moving = false;
    public Node currentNode;
    public Node targetNode;
    float closestDistance = Mathf.Infinity;
    Node node;
    void Start()
    {
        pacman = BottomBounds.pCenter.transform; // define pacman's center
	    int i = 7;
            node = GameObject.Find("MapNode ("+i+")").GetComponent<Node>();
    }
    void Update()
    {
        // define ghost's target
        //if(tagged){
        //	target = new Vector3(20.0f, 8.0f, -4.0f);
        //} else if(!powerUp.poweredUP){
        //	target = -pacman.position;
        //} //else {
        //	target = pacman.position;
        //}

        
        // Ghost's mission logic
        if(tagged) // died, back to spawn
        {
            //Debug.Log("Go to spawn");
            mission = spawn.position;
        } 
        else if(!powerUp.poweredUP && !tagged) // D good, lets chase pacman
        {
            //Debug.Log("Run towards");
            mission = pacman.position;
        }
        else // feeling sick, pacman is powered up
        {
            //Debug.Log("Run away");
            Vector3 awayDirection = (transform.position - pacman.position).normalized;
            mission = transform.position + awayDirection * 50f;
        }

        // Define target (next place to go, either node or smth else if airbone)
        if(deciding == true) // go through next step logic
        {
            // distance conditions
            foreach(Transform neighbor in currentNode.neighbors){
                distance = Vector3.Distance(neighbor.position, mission);
                if(distance <= closestDistance){
                    closestDistance = distance;
                    targetNode = neighbor.GetComponent<Node>();
                }	
            }
            deciding = false;
            moving = true;
        }
        direction = (targetNode.transform.position - transform.position).normalized; // if target is at 10 and im at 0, i need to walk 10 steps [target(10) - t.p(0) = 10)]

    }

    void FixedUpdate()
    {
        // Gravity
        rb.AddForce(Vector3.down * gravityStrength, ForceMode.Acceleration);
        
	// Move
	if(moving && Vector3.Distance(transform.position, targetNode.transform.position) < 8.5f){
		moving = false;
		deciding = true;
		currentNode = targetNode; // reached the target node, change current to target 
		closestDistance = Mathf.Infinity; // reset closest distance
		
	}
	else if(moving)
	{
		rb.AddForce(direction * speed, ForceMode.Force); // move ghost
        rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxSpeed); // clamp speed

	}
	/*
        if(tagged){ // ghost died, go back to checkpoint
	        target = new Vector3(20f,8f,-4f);
            //direction = new Vector3(20f,8f,-4f) - transform.position; // TOWARDS checkpoint
            //rb.AddForce(direction * speed/2, ForceMode.Force); // move ghost
            //rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, 20); // cap velocity
        } else if(!powerUp.poweredUP && !tagged){ // player is powered up
            target = -pacman.position;
	        //direction = target.position - transform.position; // direction TOWARDS target
            //rb.AddForce(direction * speed, ForceMode.Force); // move ghost
            //rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxSpeed); // cap velocity
        } else { // ghost is sick
            target = pacman.position;
            //direction =  transform.position - target.position; // direction AWAY from target
            //rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, 15); // cap more, slow ghosts down
            //rb.AddForce(direction * speed, ForceMode.Force); // move ghost
            //rb.AddForce(Vector3.down * gravityStrength * 5, ForceMode.Acceleration);
        } */


        // ghost texture states
        if(tagged){
            GetComponentInChildren<Renderer>().material = deadGhostMaterial;
        } else if(powerUp.poweredUP){
            GetComponentInChildren<Renderer>().material = sickGhostMaterial;
        } else {
            GetComponentInChildren<Renderer>().material = normalMaterial;
        }
       	// respawn logic 
        if(countdown && tagged && timeLeft > 0.0f){
            timeLeft -= Time.deltaTime;
        } else if(tagged && countdown){
            timeLeft = reviveTime;
            countdown = false;
            tagged = false;
        }
        Debug.DrawLine(
            transform.position,
            transform.position+direction,
            Color.red,
            0.1f,
            false
        );
        Debug.DrawLine(
            transform.position,
            targetNode.transform.position,
            Color.blue,
            0.1f,
            false
        );
        Debug.DrawLine(
            transform.position,
            mission,
            Color.green,
            0.2f,
            false
        );
    }
    // respawn logic
    void OnTriggerEnter(Collider other)
    { // is inside spawn
        if(tagged && other.CompareTag("Spawn")){
            countdown = true;
        }
    }
    void OnTriggerExit(Collider other)
    { // exited spawn
        if(tagged && other.CompareTag("Spawn")){
            countdown = false;
            timeLeft = reviveTime;
        }
    }
}
