using UnityEngine;
using UnityEngine.AI;
public class GhostRED : MonoBehaviour
{
    public Transform target;
    public UnityEngine.AI.NavMeshAgent agent;
    
    void FixedUpdate()
    {

        // Ghost movement logic
        agent.destination = target.position;
        //agent.SetDestination(target.position);
    }
}
