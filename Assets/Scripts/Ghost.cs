using UnityEngine;
using System.Collections.Generic;

public class Ghost : MonoBehaviour
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
    public enum ghostState { deciding, moving, attacking }
    public ghostState state;
    public bool rise = false;
    public Node currentNode;
    public Node targetNode;
    float closestDistance = Mathf.Infinity;
    Node node;
    float debugCounter = 1.0f;
    float debugCount = 0.0f;
    public LayerMask wallLayer;
    // Define the ghost's next step with Breadth First Search
    Node chooseNextNode(Node current, Node target) // returns next viable node towards target
    {
        Queue<Node> toVisit = new Queue<Node>(); // create a queue LIST called toVisit
        HashSet<Node> popped = new HashSet<Node>(); // create a HashSet LIST called popped
        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>(); // create a Dictionary LIST called cameFrom
        if(current == target){
            return null; // no movement needed, ghost is at target node
        }
	toVisit.Enqueue(current);
        popped.Add(current); // saw all the neighbours, crrnt node has been visited.
        while (toVisit.Count > 0)
        {
            Node crrnt = toVisit.Dequeue(); // Dequeue() removes the most recent value inside Queue and outputs it, therefore holding it in Node "it")
            if(crrnt == target){
                Node step = target; // set step at the last node (end goal)
                while (cameFrom[step] != current){ // check the links all the way back to current node
                    step = cameFrom[step];
                }
                return step; // return the next step node towards target
            }
            foreach(Transform neighbourTransform in crrnt.neighbors){ // loop through neighbours
                Node neighbour = neighbourTransform.GetComponent<Node>(); // convert Transform into Node
                if(!popped.Contains(neighbour)) // node has not been checked!
                {
                    toVisit.Enqueue(neighbour); // add this neighbour to queue
                    popped.Add(neighbour);
                    cameFrom[neighbour] = crrnt; // neighbour Node exists and it came from crrnt
                }
            }
        }
        return null; // fallback, taget is unreachable
    }
    Node findNearestNode(Vector3 missionPosition) // returns closest node to target's position
    {
        Node nearest = null;
        closestDistance = Mathf.Infinity;

        foreach (Node node in NodesManager.allNodes)
        {
            float distance = Vector3.Distance(node.transform.position, missionPosition);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearest = node;
            }
        }

        return nearest;
    }
    void Start()
    {
	    int i = 7;
            node = GameObject.Find("MapNode ("+i+")").GetComponent<Node>();
    }
    void Update()
    {
        // Ghost's mission logic
        if(tagged) // died, back to spawn
        {
            mission = spawn.position;
        } 
        else if(!powerUp.poweredUP && !tagged) // feeling good, lets chase pacman
        {
            mission = PacmanMove.centerAxis;
        }
        else // feeling sick, pacman is powered up
        {
            Vector3 awayDirection = (transform.position - pacman.position).normalized;
            mission = transform.position + awayDirection * 50f;
        }

        // Define target (next place to go, either node or smth else if airbone)
        if(state == ghostState.deciding) // go through next step logic
        {
            Node target = findNearestNode(mission); // get closest node to mission(current Target position)
            //Debug.Log(findNearestNode(mission).name);
            targetNode = chooseNextNode(currentNode, target); // choose next node to go towards
            //Debug.Log("Next node: " + targetNode.name);
            state = ghostState.moving;
        }
        // & direction
        // check if node is a warp or not
        if(targetNode == null)
        {
		state = ghostState.deciding;
        } else {
            direction = (targetNode.transform.position - transform.position).normalized; // if target is at 10 and im at 0, i need to walk 10 steps [target(10) - t.p(0) = 10)
            float disFromWarp = Vector3.Distance(transform.position, targetNode.transform.position);
            if(targetNode.isWarp && disFromWarp > 50){
                direction = (targetNode.transform.position + transform.position).normalized;
            }
        }

        Debug.DrawLine(
            transform.position,
            mission,
            Color.green,
            0.2f,
            false
            );
    }

    void FixedUpdate()
    {
        // Gravity
        rb.AddForce(Vector3.down * gravityStrength, ForceMode.Acceleration);
//	Vector3 aY = PacmanMove.centerAxis;aY.z=0;aY.x=0; Vector3 bY = transform.position;bY.z=0;bY.x=0;
//	if(state == ghostState.attacking && Vector3.Distance(aY,bY) > 1.0f){
//		//Debug.Log("Go back down");
//        	rb.AddForce(Vector3.down * gravityStrength * 4, ForceMode.Acceleration);
//	}
        // Move
        if(targetNode){
            Vector3 a = transform.position;a.y=0;
            Vector3 b = targetNode.transform.position;b.y=0;
            if(state == ghostState.moving && Vector3.Distance(a, b) < 6.0f){
		state = ghostState.deciding;
                currentNode = targetNode; // reached the target node, change current to target 
                closestDistance = Mathf.Infinity; // reset closest distance
            }
            else if(state == ghostState.moving)
            {
                rb.AddForce(direction * speed, ForceMode.Force); // move ghost
                rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxSpeed); // clamp speed

            }
        }
	// Attack behaviour
	if(Vector3.Distance(PacmanMove.centerAxis, transform.position) < 35.0f && !tagged && name == "REDghost" && !tagged)
	{
		// Red ghost only!! (he is more agressive)
		state = ghostState.attacking;
		direction = PacmanMove.centerAxis - transform.position;
                rb.AddForce(direction * 10, ForceMode.Force); // move ghost
                rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxSpeed); // clamp speed
		
	} else if(Vector3.Distance(PacmanMove.centerAxis, transform.position) < 15.0f && !tagged){
		// all the other ghosts
		state = ghostState.attacking;
		direction = PacmanMove.centerAxis - transform.position;
                rb.AddForce(direction * 10, ForceMode.Force); // move ghost
                rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxSpeed); // clamp speed
	} else if(state == ghostState.attacking) {
		state = ghostState.deciding;
	}
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
	// Fallback for if targetNode is behind a wall
	if(targetNode){
		RaycastHit hit2;
		if(state == ghostState.moving && Physics.Linecast(transform.position, targetNode.transform.position, out hit2, wallLayer)){
			//Debug.Log("target node is behind a wall");	
	            	rb.AddForce(Vector3.up * 200, ForceMode.Force); // move ghost
		} else if(Vector3.Distance(transform.position, targetNode.transform.position) > 45.0f){
	            	rb.AddForce(Vector3.up * 200, ForceMode.Force); // move ghost
		}
	}

	// Individuality
	if(name == "REDghost" && state == ghostState.attacking)
	{
		RaycastHit hit;
		if(Physics.Linecast(transform.position, PacmanMove.centerAxis, out hit, wallLayer)){//raycast check
			//Debug.Log("Mission is behing a wall!");	
                	rb.AddForce(Vector3.up * 200, ForceMode.Force); // move ghost
		}
	}
        // DEBUG
        Debug.DrawLine(
            transform.position,
            transform.position+direction,
            Color.red,
            0.1f,
            false
        );
       	if(targetNode != null){
		Debug.DrawLine(
        	    transform.position,
        	    targetNode.transform.position,
        	    Color.blue,
        	    0.1f,
        	    false
        	);
	}
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
	    state = ghostState.deciding;
        }
	
    }
}
