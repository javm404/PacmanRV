using UnityEngine;

public class pointSystem : MonoBehaviour
{
    public static int totalPoints = 0;
    void Start()
    {
        totalPoints = 0;
    }
    void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.CompareTag("Point")){
            totalPoints++;
            Destroy(collision.gameObject);
            Debug.Log("Point hit! Total points: "+totalPoints);
        }
    }
}
