using UnityEngine;
using System.Collections.Generic;

public class pointSystem : MonoBehaviour
{
    public static int totalPoints = 0;
    public static List<Transform> points = new List<Transform>();
    public Transform pointsParent;
    public scoreCount scoreCount; // to fetch scoreCount UpdateScoreCounter() method
    void Start()
    {
        totalPoints = 0;
	    points.AddRange(pointsParent.GetComponentsInChildren<Transform>());
    }
    void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.CompareTag("Point")){
            scoreCount.score += 100;
            scoreCount.UpdateScoreCounter();
	        points.Remove(collision.gameObject.transform); // remove touched point from list
            Destroy(collision.gameObject); // THEN destroy it
            //Debug.Log("Point hit! Total points: "+totalPoints);
        }
    }
}
