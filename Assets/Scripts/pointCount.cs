using UnityEngine;

public class pointCount : MonoBehaviour
{
    static public bool noMorePoints;
    public int pointsLeft;
<<<<<<< HEAD

=======
>>>>>>> dev
    void Start()
    {
        noMorePoints = false;
    }    
    void Update()
    {
        pointsLeft = transform.childCount;
        if(pointsLeft == 0){
            noMorePoints = true;
        }
    }
}
