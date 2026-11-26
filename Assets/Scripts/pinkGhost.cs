using UnityEngine;
using System.Collections.Generic;

public class pinkGhost : GhostBase
{
    public Transform pacman;
    public Transform spawn;
    public Vector3 mission;
    //private Vector3 target;
    public Rigidbody rb;
    public float speed; 
    public float gravityStrength;
    public float maxSpeed = 80f;
    // REMOVED: public bool tagged = false; (now in GhostBase)
    public float reviveTime = 4.0f;
    public float timeLeft; 
    bool countdown = false;
    Vector3 direction;
    float distance;
    public Material normalMaterial;
    public Material deadGhostMaterial;
    public Material sickGhostMaterial;
    // REMOVED: public enum ghostState { deciding, moving, attacking, tackling } (now in GhostBase)
    public float tackleDelay = 2.5f;
    public float tackleCount;
    public bool airbone;
    // REMOVED: public ghostState state; (now in GhostBase)
    public bool rise = false;
    public bool outOfSpawn = false;
    public Node currentNode;
    public Node targetNode;
    float closestDistance = Mathf.Infinity;
    Node node;
    public LayerMask wallLayer;
    Transform closestSpot;
    public Transform trgtPoint;
    public bool reachedPoint;
    public Transform personalCorner;
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
	    tackleCount = tackleDelay; 
        timeLeft = reviveTime;
    }
    void Update()
    {
        // define mission logic
        if(deathBehaviour.respawning)
        {
            mission = personalCorner.position;
        }
        else if(tagged) // died, back to spawn
        {
            mission = spawn.position;
        } 
        else if(!powerUp.poweredUP && !tagged) // feeling good, lets chase pacman
        {
            mission = findNearestNode(PacmanMove.centerAxis+PacmanMove.pacmanRb.linearVelocity.normalized * 35).transform.position;
            if(Vector3.Distance(transform.position, PacmanMove.centerAxis) <= 50.0f){ // very close, go spot on
                mission = PacmanMove.centerAxis;
            } else if(Vector3.Distance(transform.position, PacmanMove.centerAxis) >= 200.0f){ // very far, predict further
                mission = findNearestNode(PacmanMove.centerAxis+PacmanMove.pacmanRb.linearVelocity.normalized * 200).transform.position;
            }
        }
        else // feeling sick, pacman is powered up
        {
            Vector3 awayDirection = (transform.position - pacman.position).normalized;
            mission = transform.position + awayDirection * 50f;
        }
	    Debug.DrawLine(PacmanMove.centerAxis, PacmanMove.centerAxis+PacmanMove.pacmanRb.linearVelocity, Color.orange, 0.1f, false);
        // Define target (next place to go, either node or smth else if airbone)
        if(state == ghostState.deciding) // go through next step logic
        {
            Node target = findNearestNode(mission); // get closest node to mission(current Target position)
            //Debug.Log(findNearestNode(mission).name);
            targetNode = chooseNextNode(currentNode, target); // choose next node to go towards
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
            Color.pink,
            0.2f,
            false
            );
	// check if airbone
	if(transform.position.y < 45.0f){
		airbone = false;
	} else {
		airbone = true;
	}
    }

    void FixedUpdate()
    {
        // Gravity
	if(airbone == true && name != "PINKghost" && name != "GREENghost"){
        	rb.AddForce(Vector3.down * gravityStrength * 2.5f, ForceMode.Acceleration);
	} else if(powerUp.poweredUP && airbone) { // force ghost grounded when sick
        	rb.AddForce(Vector3.down * gravityStrength * 80f, ForceMode.Acceleration);
	} else {
        	rb.AddForce(Vector3.down * gravityStrength, ForceMode.Acceleration);
	}
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
	if(Vector3.Distance(PacmanMove.centerAxis, transform.position) < 20.0f && !tagged && !powerUp.poweredUP && state != ghostState.tackling){
		state = ghostState.attacking;
		direction = PacmanMove.centerAxis - transform.position;
        rb.AddForce(direction * 10f, ForceMode.Force); // move ghost
        rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxSpeed); // clamp speed
	} else if(state == ghostState.attacking) {
		currentNode = findNearestNode(transform.position);//reset current node
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

       	// respawn logic FixedUpdate
        if(countdown && tagged && timeLeft > 0.0f){
            timeLeft -= Time.deltaTime;
        } else if(tagged && countdown){
            timeLeft = reviveTime;
            countdown = false;
            tagged = false;
        }
        // spot where u are if out of spawn
        if(outOfSpawn && state == ghostState.deciding){
            currentNode = findNearestNode(transform.position);
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

    if(airbone == true){
        rb.AddForce(Vector3.down * gravityStrength * -10f, ForceMode.Acceleration);
        state = ghostState.tackling;
    }
    if(state == ghostState.tackling && airbone == true && Vector3.Distance(transform.position, PacmanMove.centerAxis) > 10f && gameController.win == false && deathBehaviour.respawning == false){
        if(tackleCount > 0){
            direction = Vector3.zero;
            tackleCount -= Time.deltaTime;
        } else {
            direction = PacmanMove.centerAxis - transform.position;
            rb.AddForce(direction * 0.40f, ForceMode.Impulse); // move ghost
        }
    } else if(state == ghostState.tackling) {
        currentNode = findNearestNode(transform.position);
        state = ghostState.deciding;
        tackleCount = tackleDelay;
    }

        // DEBUG
        Debug.DrawLine(
            transform.position,
            transform.position+direction,
            Color.yellow,
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
	        outOfSpawn = false;
        }
    }
    void OnTriggerExit(Collider other)
    { // exited spawn
        if(tagged && other.CompareTag("Spawn")){
            Debug.Log("Respawn canceled");
            countdown = false;
            timeLeft = reviveTime;
            state = ghostState.deciding;
            outOfSpawn = true;

        }
    }
}
