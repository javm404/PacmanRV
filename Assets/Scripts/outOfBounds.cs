using UnityEngine;
public class BottomBounds : MonoBehaviour
{
    public Transform player;    // grab pacman(player)
    public BoxCollider pacmanCenter; // #patch#: grab pacman's real center axis
    static public BoxCollider pCenter;
    float radius = 0.85f * 6; // define player's collider radius
    public Transform[] ghosts;    // grab ghosts
    public Transform bounds;    // grab game bounds Transform (for position and scale)
    private BoxCollider boundsCollider;     // grab bounds collider ??? is this even being used ???
    private Vector3 halfExt;

    // Pacman's position and tpLock vars
    private Vector3 pastLocalPosition_pacman;
    private bool teleportLocked_pacman = false;

    private Rigidbody rb;
    private Vector3 worldTarget;
    void Start()
    {   
        // bounds related
        boundsCollider = GetComponent<BoxCollider>();

        // pacman related
        pacmanCenter = player.GetComponent<BoxCollider>();
        pCenter = pacmanCenter;
        rb = player.GetComponent<Rigidbody>();
        Vector3 pacmanWorld = pacmanCenter.transform.TransformPoint(pacmanCenter.center);
        pastLocalPosition_pacman = bounds.InverseTransformPoint(pacmanWorld) - boundsCollider.center;
    }

    void FixedUpdate()
    {
        // get box's half dimentions, to find its center and define it's coordinates
        halfExt = boundsCollider.size * 0.5f;
        float margin = 1f; // margin for pacman tp 
        // ?(needs to be bigger than that* get the radius somehow)?

        // Declare/command/define a teleport
        Vector3 pacmanWorld = pacmanCenter.transform.TransformPoint(pacmanCenter.center);
        CheckAndTeleport(pacmanWorld, player.GetComponent<Rigidbody>(), halfExt, margin, ref pastLocalPosition_pacman, ref teleportLocked_pacman);

        Vector3 centre = bounds.TransformPoint(boundsCollider.center);
    }

    void CheckAndTeleport(Vector3 worldPos, Rigidbody rb, Vector3 halfExt, float margin, ref Vector3 pastLocal, ref bool locked)
    {
        Vector3 localPos = bounds.InverseTransformPoint(worldPos) - boundsCollider.center; // define pacman's position relative to the board's center axis
        bool insideX = Mathf.Abs(localPos.x) < halfExt.x;
        bool insideZ = Mathf.Abs(localPos.z) < halfExt.z;
        bool insideY = localPos.y > -radius;

        // BTW, JUST SO YK:
        // TransformPoint: takes a local coord, outputs the equivalent world coord.
        // InverseTransformPoint: takes a world coord, outputs the equivalent local coord.

        Vector3 worldTarget = worldPos; // default: if nothing teleports, draw line to current pos
        float x = Mathf.Abs(localPos.x);
        float z = Mathf.Abs(localPos.z);
        if (insideX && insideZ && insideY)
        {
            pastLocal = localPos; // cache pastFrame position
            locked = false;
        }
        else //if (!locked) // teleport
        {
            //Debug.Log("Not safe");
            Vector3 mirrored = pastLocal; margin = 6f;
            float maxX = halfExt.x - radius - margin;
            float maxY = halfExt.y - radius - margin;
            float maxZ = halfExt.z - radius - margin;
            mirrored.x = Mathf.Clamp(mirrored.x, -maxX, maxX);
            mirrored.y = Mathf.Clamp(mirrored.y, -maxY, maxY);
            mirrored.z = Mathf.Clamp(mirrored.z, -maxZ, maxZ);

            if (!insideX) mirrored.x *= -1;
            if (!insideZ) mirrored.z *= -1;
            if (!insideY) mirrored.y *= -1;
            // CLAUDE: replace the line above with this, to always recover above the board instead of mirroring Y
            if (!insideY) mirrored.y = maxY;

            worldTarget = bounds.TransformPoint(mirrored + boundsCollider.center); // define location
            rb.position = worldTarget; // do the teleport
            rb.linearVelocity = Vector3.zero;
            locked = true;
        }
    }
}