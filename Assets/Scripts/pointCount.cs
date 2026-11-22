using UnityEngine;

public class pointCount : MonoBehaviour
{
    static public bool noMorePoints;
    public int pointsLeft;

    void Start()
    {
        noMorePoints = false;
    }    
    void Update()
    {
        pointsLeft = 0;//transform.childCount;
        if(pointsLeft == 0){
            noMorePoints = true;
        }
    }
}
