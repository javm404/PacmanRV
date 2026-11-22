using UnityEngine;

public class gameController : MonoBehaviour
{
    public Transform walls;
    public float mod;
    public float y;

    void Start()
    {
        y = walls.position.y;
        mod = 0;
    }
    void FixedUpdate()
    {
        if(pointCount.noMorePoints){
            Debug.Log("Game over!");
            if(walls.position.y > -12){
                mod--;
                y = y - mod;
                Vector3 position = walls.position;
                position.y = y;
                walls.position = position;
            }
        }
    }
}
