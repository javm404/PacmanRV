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
        if (scoreCounterText != null)
        {
            scoreCounterText.text = "SCORE: " + score;
        }
    }


}
