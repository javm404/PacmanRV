using UnityEngine;
using System.Collections.Generic;

public class pointSystem : MonoBehaviour
{
    public static int totalPoints = 0;
<<<<<<< HEAD
    public int points;
=======
    public static List<Transform> points = new List<Transform>();
    public Transform pointsParent;
>>>>>>> dev
    void Start()
    {
        totalPoints = 0;
	points.AddRange(pointsParent.GetComponentsInChildren<Transform>());
    }
    void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.CompareTag("Point")){
            totalPoints++;
            Destroy(collision.gameObject);
	    points.Remove(collision.gameObject.transform); // remove touched point from list
            //Debug.Log("Point hit! Total points: "+totalPoints);
        }
    }
    void Update(){
        points = totalPoints;
    }
}
