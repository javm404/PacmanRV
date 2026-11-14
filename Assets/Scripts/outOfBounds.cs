
using UnityEngine;

public class BottomBounds : MonoBehaviour
{
    public Transform player;
    public Transform bounds;

    private Vector3 pastPosition;
    private BoxCollider boundsCollider;
    private bool teleportLocked = false;
    float radius = 1.6f; // get player's collider radius
    private Rigidbody rb;
    void Start()
    {
        boundsCollider = GetComponent<BoxCollider>();
        pastPosition = player.position;
        rb = player.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 localPos = bounds.InverseTransformPoint(player.position) - boundsCollider.center;

        // get box's half dimensions
        Vector3 halfExt = boundsCollider.size * 0.5f;

        float margin = 2f;

        bool insideX = Mathf.Abs(localPos.x) < halfExt.x - radius - margin;
        bool insideZ = Mathf.Abs(localPos.z) < halfExt.z - radius - margin;
        bool insideY = localPos.y > -8.01f - margin;
        /*
        Debug.Log(
            $"Local: {localPos} | Inside X: {insideX} | Inside Z: {insideZ} | Inside Y: {insideY} | TPlock: {teleportLocked}"
        );
        */

        if (insideX && insideZ && insideY){
            pastPosition = player.position; // log player's safe position
            teleportLocked = false;
        } else if(!teleportLocked){
            Vector3 lastGoodLocalPos = bounds.InverseTransformPoint(pastPosition) - boundsCollider.center;      // Convert the last valid position into bounds-local space.
            Vector3 mirrored = lastGoodLocalPos;
            float maxX = halfExt.x - radius;
            float maxY = halfExt.y - radius;
            float maxZ = halfExt.z - radius;
            mirrored.x = Mathf.Clamp(mirrored.x, -maxX, maxX);
            mirrored.y = Mathf.Clamp(mirrored.y, -maxY, maxY);
            mirrored.z = Mathf.Clamp(mirrored.z, -maxZ, maxZ);
            
            if(!insideX){
                mirrored.x *= -1;
                Debug.Log($"Teleporting X from {player.position.x} to {mirrored.x}");
            } 
            if(!insideZ){
                mirrored.z *= -1;
                Debug.Log($"Teleporting Z from {player.position.z} to {mirrored.z}");
            } 
            if(!insideY){
                mirrored.y *= -1;
                Debug.Log($"Teleporting Y from {player.position.y} to {mirrored.y}");
            }
            Vector3 worldTarget = bounds.TransformPoint(mirrored + boundsCollider.center);
            rb.position = worldTarget; // teleport player
            teleportLocked = true;
            Debug.Log($"Last safe: {lastGoodLocalPos}");
            Debug.Log($"Mirrored: {mirrored}");
            Debug.Log($"World target: {worldTarget}");
        }
    }
}
