using UnityEngine;
public class BottomBounds : MonoBehaviour
{
    public Transform player;    // grab pacman(player)
    public BoxCollider pacmanCenter; // #patch#: grab pacman's real center axis
    float radius = 1.6f; // define player's collider radius
    public Transform[] ghosts;    // grab ghosts
    public Transform bounds;    // grab game bounds Transform (for position and scale)
    private BoxCollider boundsCollider;     // grab bounds collider ??? is this even being used ???

    // Pacman's position and tpLock vars
    private Vector3 pastLocalPosition_pacman;
    private bool teleportLocked_pacman = false;

    // Ghosts' position and tpLock vars (one slot per ghost)
    private Vector3[] pastLocalPosition_ghosts;
    private bool[] teleportLocked_ghosts;

    private Rigidbody rb;
    private Vector3 worldTarget;
    void Start()
    {   
        // bounds related
        boundsCollider = GetComponent<BoxCollider>();

        // pacman related
        pacmanCenter = player.GetComponent<BoxCollider>();
        rb = player.GetComponent<Rigidbody>();
        Vector3 pacmanWorld = pacmanCenter.transform.TransformPoint(pacmanCenter.center);
        pastLocalPosition_pacman = bounds.InverseTransformPoint(pacmanWorld) - boundsCollider.center;

        // ghosts related
        pastLocalPosition_ghosts = new Vector3[ghosts.Length];
        teleportLocked_ghosts = new bool[ghosts.Length];
        for (int i = 0; i < ghosts.Length; i++){
            pastLocalPosition_ghosts[i] = bounds.InverseTransformPoint(ghosts[i].position) - boundsCollider.center;
        }
    }

    void FixedUpdate()
    {
        // get box's half dimentions, to find its center and define it's coordinates
        Vector3 halfExt = boundsCollider.size * 0.5f;
        float margin = 2f; // margin for pacman tp

        
        // --- Pacman (special case: uses offset collider center) ---
        Vector3 pacmanWorld = pacmanCenter.transform.TransformPoint(pacmanCenter.center);
        CheckAndTeleport(pacmanWorld, player.GetComponent<Rigidbody>(), halfExt, margin, ref pastLocalPosition_pacman, ref teleportLocked_pacman);

        Vector3 centre = bounds.TransformPoint(boundsCollider.center);

        // --- Ghosts (loop, indexed by array position) ---
        for (int i = 0; i < ghosts.Length; i++)
        {
            Vector3 ghostWorld = ghosts[i].position;
            CheckAndTeleport(ghostWorld, ghosts[i].GetComponent<Rigidbody>(), halfExt, margin,
                ref pastLocalPosition_ghosts[i], ref teleportLocked_ghosts[i]);

            // purple line: each ghost's current position relative to the board center
            Debug.DrawLine(centre, ghostWorld, new Color(0.6f, 0f, 1f), 0.05f, false);
        }
    }
    void CheckAndTeleport(Vector3 worldPos, Rigidbody rb, Vector3 halfExt, float margin, ref Vector3 pastLocal, ref bool locked)
    {
        Vector3 localPos = bounds.InverseTransformPoint(worldPos) - boundsCollider.center;

        bool insideX = Mathf.Abs(localPos.x) < halfExt.x - radius - margin;
        bool insideZ = Mathf.Abs(localPos.z) < halfExt.z - radius - margin;
        bool insideY = localPos.y > -8.01f - margin;

        Vector3 worldTarget = worldPos; // default: if nothing teleports, draw line to current pos

        if (insideX && insideZ && insideY)
        {
            pastLocal = localPos;
            locked = false;
        }
        else if (!locked)
        {
            Vector3 mirrored = pastLocal;
            float maxX = halfExt.x - radius - margin;
            float maxY = halfExt.y - radius - margin;
            float maxZ = halfExt.z - radius - margin;
            mirrored.x = Mathf.Clamp(mirrored.x, -maxX, maxX);
            mirrored.y = Mathf.Clamp(mirrored.y, -maxY, maxY);
            mirrored.z = Mathf.Clamp(mirrored.z, -maxZ, maxZ);

            if (!insideX) mirrored.x *= -1;
            if (!insideZ) mirrored.z *= -1;
            if (!insideY) mirrored.y *= -1;

            worldTarget = bounds.TransformPoint(mirrored + boundsCollider.center);
            rb.position = worldTarget;
            rb.linearVelocity = Vector3.zero;
            locked = true;
        }
            // Debug lines
            /*
                Vector3 centre = bounds.TransformPoint(boundsCollider.center);
                Debug.DrawLine(
                    centre,
                    worldTarget,
                    Color.cyan,
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
                */

    }
}