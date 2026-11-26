using UnityEngine;
using TMPro;

public class scoreCount : MonoBehaviour
{
    public TextMeshPro scoreCounterText;
    public static int score = 0;
    void Start()
    {
        UpdateScoreCounter();
        score = 0;
    }

    public void UpdateScoreCounter()
    {
        if(score < 0){
            score = 0;
        }
        if (scoreCounterText != null)
        {
            scoreCounterText.text = "SCORE: " + score;
        }
    }


}
