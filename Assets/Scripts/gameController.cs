using UnityEngine;

public class gameController : MonoBehaviour
{
    public GameObject winText;
    public GameObject tyText;
    public Transform walls;
    public Transform ghosts;
    public Material floorNormal;
    public Material floorWin;
    public Renderer floor;
    private Renderer rend;
    private Material blend;
    public float trans;
    public float mod;
    public float mod2;
    public float y;
    public float y2;
    public float delay = 1.0f;
    public bool shine = false;

    void Start()
    {
        y = walls.position.y;
        mod = 0.0f;
        rend = floor;
        blend = new Material(floorNormal.shader);
        blend.EnableKeyword("_EMISSION");
        trans = 0f;
        winText.SetActive(false);
        tyText.SetActive(false);
    }
    void FixedUpdate()
    {
        if(pointCount.noMorePoints){
            winText.SetActive(true);
            tyText.SetActive(true);
            Debug.Log("Game over!");
            if(walls.position.y > -22){
                mod += Time.deltaTime / 10;
                y = y - mod;
                // change walls
                Vector3 position = walls.position;
                position.y = y;
                walls.position = position;
            } 

            if(delay > 0){
                delay -= Time.deltaTime;
            } else if(trans < 1){
                trans += Time.deltaTime / 100;
                blend.Lerp(floorNormal, floorWin, trans);
                rend.material = blend; // apply the fade
            }
            if(ghosts.position.y < 1000){
                mod2 += Time.deltaTime;
                y2 = y2 + mod2;
                // change ghosts
                Vector3 position = ghosts.position;
                position.y = y2;
                ghosts.position = position;
            } else {
                foreach( Transform g in ghosts.GetComponentsInChildren<Transform>()) {
                    g.gameObject.SetActive(false);
                }
            }
        }
    }
}
