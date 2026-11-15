
using UnityEngine;

public class BottomBounds : MonoBehaviour
{
    public Transform player;
    public Transform bounds;
    public BoxCollider pacmanCenter;
    
    private Vector3 pastLocalPosition;
    private BoxCollider boundsCollider;
    private bool teleportLocked = false;
    float radius = 1.6f; // get player's collider radius
    private Rigidbody rb;
    void Start()
    {
        boundsCollider = GetComponent<BoxCollider>();
        pacmanCenter = player.GetComponent<BoxCollider>();
        rb = player.GetComponent<Rigidbody>();

        Vector3 pacmanCenterRel = pacmanCenter.transform.TransformPoint(pacmanCenter.center);
        pastLocalPosition = bounds.InverseTransformPoint(pacmanCenterRel) - boundsCollider.center;
    }

    void FixedUpdate()
    {
        Vector3 pacmanCenterRel = pacmanCenter.transform.TransformPoint(pacmanCenter.center);
        Vector3 localPos = bounds.InverseTransformPoint(pacmanCenterRel) - boundsCollider.center;

        // get box's half dimentions, to find its center and define it's coordinates
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
            //pastPosition = player.position; // log player's safe position
            pastLocalPosition = bounds.InverseTransformPoint(pacmanCenterRel) - boundsCollider.center;
            teleportLocked = false;
        } else if(!teleportLocked){
            Vector3 lastGoodLocalPos = pastLocalPosition;      // Convert the last valid position into bounds-local space.
            Vector3 mirrored = lastGoodLocalPos;
            float maxX = halfExt.x - radius - margin;
            float maxY = halfExt.y - radius - margin;
            float maxZ = halfExt.z - radius - margin;
            mirrored.x = Mathf.Clamp(mirrored.x, -maxX, maxX);
            mirrored.y = Mathf.Clamp(mirrored.y, -maxY, maxY);
            mirrored.z = Mathf.Clamp(mirrored.z, -maxZ, maxZ);
            
            if(!insideX){
                mirrored.x *= -1;
                Debug.Log($"Teleporting X: {localPos.x} -> {mirrored.x} (limit: {maxX})");
            } 
            if(!insideZ){
                mirrored.z *= -1;
                Debug.Log($"Teleporting Z: {localPos.z} -> {mirrored.z} (limit: {maxZ})");
            } 
            if(!insideY){
                mirrored.y *= -1;
                Debug.Log($"Teleporting Y: {localPos.y} -> {mirrored.y} (limit: {maxY})");

            }
            // teleport pacman
            Vector3 worldTarget = bounds.TransformPoint(mirrored + boundsCollider.center); // define teleport location
            rb.position = worldTarget - (pacmanCenterRel - rb.position); // teleport pacman
            teleportLocked = true;
        }
        // Debug lines
            Vector3 centre = bounds.TransformPoint(boundsCollider.center);

            Debug.DrawLine(
                centre,
                pacmanCenterRel,
                Color.cyan,
                0.05f,
                false
            );
            Vector3 localPosWorld = bounds.TransformPoint(localPos + boundsCollider.center);
            Debug.DrawLine(
                centre,
                localPosWorld,
                Color.yellow,
                0.05f,
                false
            );
            // X axis (width) - RED
            Debug.DrawLine(centre,
                bounds.TransformPoint(boundsCollider.center + Vector3.right * halfExt.x),
                Color.red, 0.05f, false);

            // Y axis (height) - GREEN
            Debug.DrawLine(centre,
                bounds.TransformPoint(boundsCollider.center + Vector3.up * halfExt.y),
                Color.green, 0.05f, false);

            // Z axis (depth) - BLUE
            Debug.DrawLine(centre,
                bounds.TransformPoint(boundsCollider.center + Vector3.forward * halfExt.z),
                Color.blue, 0.05f, false);

    }
}
