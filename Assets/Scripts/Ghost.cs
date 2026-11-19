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
    //public enum ghostState { deciding, moving }
    //public ghostState state;
    public bool deciding = true;
    public bool moving = false;
    public Node currentNode;
    public Node targetNode;
    float closestDistance = Mathf.Infinity;
    Node node;
    float debugCounter = 1.0f;
    float debugCount = 0.0f;
    // Define the ghost's next step with Breadth First Search
    Node chooseNextNode(Node current, Node target) // returns next viable node towards target
    {
        Queue<Node> toVisit = new Queue<Node>(); // create a queue LIST called toVisit
        HashSet<Node> popped = new HashSet<Node>(); // create a HashSet LIST called popped
        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>(); // create a Dictionary LIST called cameFrom

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
        pacman = BottomBounds.pCenter.transform; // define pacman's center
	    int i = 7;
            node = GameObject.Find("MapNode ("+i+")").GetComponent<Node>();
    }
    void Update()
    {
        // Ghost's mission logic
            if(tagged) // died, back to spawn
            {
                //Debug.Log("Go to spawn");
                mission = spawn.position;
            } 
            else if(!powerUp.poweredUP && !tagged) // feeling good, lets chase pacman
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
                Node target = findNearestNode(mission); // get closest node to mission(current Target position)
                Debug.Log(findNearestNode(mission).name);
                targetNode = chooseNextNode(currentNode, target); // choose next node to go towards
                Debug.Log("Next node: " + targetNode.name);
                deciding = false;
                moving = true;
            }
            // & direction
            // check if node is a warp or not
            direction = (targetNode.transform.position - transform.position).normalized; // if target is at 10 and im at 0, i need to walk 10 steps [target(10) - t.p(0) = 10)]

    }

    void FixedUpdate()
    {
        // Gravity
        rb.AddForce(Vector3.down * gravityStrength, ForceMode.Acceleration);
        
        // Move
        if(moving && Vector3.Distance(transform.position, targetNode.transform.position) < 6.0f){
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

        // DEBUG
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
        if(debugCount > 0){
            debugCount -= Time.deltaTime;
        } else if (moving){
            Debug.Log(name+" IS MOVING");
            //Debug.Log("distance from "+distance+" to "+target);
            //Debug.Log("Direction: "+target);
            debugCount = debugCounter;
        } else if (deciding){
            Debug.Log(name+" IS DECIDING");
            //Debug.Log("distance from "+distance+" to "+target);
            //Debug.Log("Current position: "+currentNode);
            debugCount = debugCounter;
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
        }
    }
}
