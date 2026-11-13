using UnityEngine;

public class outOfBounds : MonoBehaviour
{
    public Transform player; // to manage n monitor player's position
    private Vector3 pastPosition;

    void FixedUpdate()
    {
        pastPosition = player.position; // store player's past frame position
    }
}
