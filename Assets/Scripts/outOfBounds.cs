using UnityEngine;

public class BottomBounds : MonoBehaviour
{
    public Transform player; // to manage n monitor player's position
    private Vector3 pastPosition;
    Bounds b;
    BoxCollider boundsCollider;
    Vector3 halfExt;
    Vector3 localPos; 
    public Transform board;
    void Start()
    {
        //b = GetComponent<Collider>().bounds;
        boundsCollider = GetComponent<BoxCollider>();
        halfExt = boundsCollider.size * 0.5f; // halfExt.y and halfExt.z to get half the bounds size
        localPos = board.InverseTransformPoint(player.position); // call localPos at least once without the else condition, just so it isnt empty ever
    }

    void FixedUpdate()
    {
        bool insideX = localPos.x > -halfExt.x && localPos.x < halfExt.x;
        bool insideZ = localPos.z > -halfExt.z && localPos.z < halfExt.z;
        bool insideY = localPos.y > -halfExt.y && localPos.y < halfExt.y;
        bool inBounds = insideX && insideZ && insideY;
        if(!inBounds){
            Debug.Log("not inBounds trigger");
            Vector3 opposite = board.TransformPoint(-localPos); // get pacman's opposite current position
            player.position = opposite; // teleport pacman to opposite side of where he left (if left bounds in top, get teleported to bottom)
            Debug.Log(localPos + " -> " + opposite);
        } else {
            //pastPosition = player.position; // grabs player's past position
            localPos = board.InverseTransformPoint(player.position); // grabs the player's past position RELATIVE to the bounds' XYZ axis, therefore, board rotation (which rotates the bounds) doesnt bug it
        }
    }

    void OnTriggerExit()
}
