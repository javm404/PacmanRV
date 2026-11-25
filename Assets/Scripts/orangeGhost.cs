using UnityEngine;
using System.Collections.Generic;

public class orangeGhost : GhostBase
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
    float debugCounter = 1.0f;
    float debugCount = 0.0f;
    public LayerMask wallLayer;
    public GameObject orangeSpots;
    public Node[] cornerNodes;
    Transform closestSpot;
    public float retreatCount = 0f;
    public float retreatAmnt = 3.0f;
    public bool inRetreat = false;
    public bool retreating = false;
    public Transform trgtPoint;
    public bool reachedPoint;
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
    }
    void Update()
    {
        //Debug.Log(name+" pos:"+transform.position+" retreating:"+retreating+" state:"+state+" dist:"+Vector3.Distance(transform.position, PacmanMove.centerAxis));
        // define mission logic
        if(tagged) // died, back to spawn
        {
            mission = spawn.position;
        } 
        else if(!powerUp.poweredUP && !tagged && !retreating) // feeling good, lets chase pacman
        {
            if(name != "PINKghost" && name != "GREENghost"){
                mission = PacmanMove.centerAxis;
            } else if(name == "PINKghost") { // apply only on pink ghost
                if(Vector3.Distance(transform.position, PacmanMove.centerAxis) <= 60.0f){
                    mission = findNearestNode(PacmanMove.centerAxis+PacmanMove.pacmanRb.linearVelocity.normalized * 55).transform.position;
                }else{
                    mission = PacmanMove.centerAxis;
                }
            }
        }
        else if(retreating)
        {
            mission = closestSpot.position;
        }
        else if(name != "GREENghost") // feeling sick, pacman is powered up
        {
            Vector3 awayDirection = (transform.position - pacman.position).normalized;
            mission = transform.position + awayDirection * 50f;
        }
        // green ghost logic
        if(trgtPoint == null && name == "GREENghost"){
            trgtPoint = pointSystem.points[Random.Range(0, pointSystem.points.Count)];
            mission = findNearestNode(trgtPoint.position).transform.position;
        }else if(name == "GREENghost" && Vector3.Distance(transform.position, mission) < 10f){
            trgtPoint = pointSystem.points[Random.Range(0, pointSystem.points.Count)];
            mission = findNearestNode(trgtPoint.position).transform.position;
        }
	Debug.DrawLine(PacmanMove.centerAxis, PacmanMove.centerAxis+PacmanMove.pacmanRb.linearVelocity, Color.orange, 0.1f, false);
        // Define target (next place to go, either node or smth else if airbone)
        if(state == ghostState.deciding) // go through next step logic
        {
            Node target = findNearestNode(mission); // get closest node to mission(current Target position)
            //Debug.Log(findNearestNode(mission).name);
            targetNode = chooseNextNode(currentNode, target); // choose next node to go towards
            Debug.Log(currentNode.name + " vs " + findNearestNode(mission).name);
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
	// check if airbone
	if(transform.position.y > 30.0f){
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
        	rb.AddForce(Vector3.down * gravityStrength * 30f, ForceMode.Acceleration);
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
	if(Vector3.Distance(PacmanMove.centerAxis, transform.position) < 34.0f && !tagged && name == "REDghost" && !powerUp.poweredUP)
	{
		// Red attack! (he is more agressive)
		state = ghostState.attacking;
		direction = PacmanMove.centerAxis - transform.position;
        rb.AddForce(direction * 12f, ForceMode.Force); // move ghost
        rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxSpeed); // clamp speed
	} else if(Vector3.Distance(PacmanMove.centerAxis, transform.position) < 20.0f && !tagged && !powerUp.poweredUP && state != ghostState.tackling && !retreating){
		// all the other ghosts
		state = ghostState.attacking;
		direction = PacmanMove.centerAxis - transform.position;
        if(name == "GREENghost"){
            rb.AddForce(direction.normalized * 0.5f, ForceMode.Force); // move ghost
        } else {
            rb.AddForce(direction * 11f, ForceMode.Force); // move ghost
        }
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

       	// respawn logic 
        if(countdown && tagged && timeLeft > 0.0f){
            timeLeft -= Time.deltaTime;
        } else if(tagged && countdown){
            timeLeft = reviveTime;
            countdown = false;
            tagged = false;
        }
	if(outOfSpawn && state == ghostState.deciding){
        currentNode = findNearestNode(transform.position);

	}
	// Fallback for if targetNode is behind a wall
	if(targetNode){
        Vector3 myPosXZ = transform.position; myPosXZ.y = 0;
        Vector3 trgtPosXZ = targetNode.transform.position; trgtPosXZ.y = 0;
		RaycastHit hit2;
		if(state == ghostState.moving && Physics.Linecast(transform.position, targetNode.transform.position, out hit2, wallLayer)){
			//Debug.Log("target node is behind a wall");	
            if(name == "GREENghost"){
                rb.AddForce(Vector3.up * 10, ForceMode.Force); // move GREENghost
            } else {
                rb.AddForce(Vector3.up * 200, ForceMode.Force); // move ghost
            }
		} 
        else if(Vector3.Distance(myPosXZ, trgtPosXZ) > 45.0f){
            if(name == "GREENghost"){
                rb.AddForce(Vector3.up * 10, ForceMode.Force); // move GREENghost
            } else {
                rb.AddForce(Vector3.up * 200, ForceMode.Force); // move ghost
            }
        }
	}

	// Individuality
	if(name == "REDghost")
	{
		if(state == ghostState.attacking){
			RaycastHit hit;
			if(Physics.Linecast(transform.position, PacmanMove.centerAxis, out hit, wallLayer)){//raycast check
				Debug.Log("Mission is behing a wall!");	
                rb.AddForce(Vector3.up * 600, ForceMode.Force); // move ghost
			}
		}
	} 
	else if (name == "PINKghost")
	{
        bool pAirbone;
        if(transform.position.y < 60.0f){
            pAirbone = false;
        } else {
            pAirbone = true;
        }
		if(pAirbone == true){
        		rb.AddForce(Vector3.down * gravityStrength * -10f, ForceMode.Acceleration);
			state = ghostState.tackling;
		}
		if(state == ghostState.tackling && airbone == true && Vector3.Distance(transform.position, PacmanMove.centerAxis) > 10f){
			if(tackleCount > 0){
				direction = Vector3.zero;
				tackleCount -= Time.deltaTime;
			} else {
				direction = PacmanMove.centerAxis - transform.position;
                rb.AddForce(direction * 0.2f, ForceMode.Impulse); // move ghost
                rb.AddForce(Vector3.down * gravityStrength * 20f, ForceMode.Acceleration);
			}
		} else if(state == ghostState.tackling) {
			currentNode = findNearestNode(transform.position);
			state = ghostState.deciding;
			tackleCount = tackleDelay;
		}
	}
	else if (name == "ORANGEghost")
	{
		float minDist = Mathf.Infinity;
		if(Vector3.Distance(transform.position, PacmanMove.centerAxis) < 68f)
		{
			Debug.Log("oh no! invading personal space!");
			// evaluate closest border spot
            		if(closestSpot){
                	mission = closestSpot.position;
            		}
			foreach (Node spot in cornerNodes)
			{
				float dist = Vector3.Distance(transform.position, spot.transform.position);
				if(dist < minDist)
				{
					minDist = dist;
					closestSpot = spot.transform;
				}
			}
			// define closest as mission
            		if(!retreating){
			    mission = closestSpot.position;
            		}
			retreating = true;
		}
        	if(closestSpot){
        	    Debug.DrawLine(
        	        transform.position,
        	        closestSpot.position,
        	        Color.orange,
        	        0.1f,
        	        false
        	    );
        	}
        	// retreat logic
        	if (retreating && retreatCount < retreatAmnt && inRetreat){
        	    retreatCount += Time.deltaTime;
        	} else if(retreating && retreatCount >= retreatAmnt){
        	    inRetreat = false;
        	    retreating = false;
        	    retreatCount = 0;
        	    state = ghostState.deciding;
        	}
	}
    	else if(name == "GREENghost")
    	{
		if(airbone == true){ // triple green ghost's gravity when thrown upwards
        		rb.AddForce(Vector3.down * gravityStrength * 10, ForceMode.Acceleration);
		}
        if(trgtPoint){
            Debug.DrawLine(
                transform.position,
                trgtPoint.position,
                Color.violet,
                0.1f,
                false
            );
        }
			
	}

        // pick a random point

        // check closest node to that point

        //set that node as the mission

        // after arriving pick another
    // end of individual ghosts logic
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
	if(other.CompareTag("OrangeSpot") && retreating){
        retreatCount = 0;
        inRetreat = true;
	}
    }
    void OnTriggerExit(Collider other)
    { // exited spawn
        if(tagged && other.CompareTag("Spawn")){
            countdown = false;
            timeLeft = reviveTime;
            state = ghostState.deciding;
            outOfSpawn = true;

        }
    }
}
