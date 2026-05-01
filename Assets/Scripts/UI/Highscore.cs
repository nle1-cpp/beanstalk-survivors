using UnityEngine;



public class Highscore : MonoBehaviour
{
    public TMPro.TextMeshProUGUI highScoreText;
    void Start() { 

        if (GameManager.Instance != null)
        {
            highScoreText.text = "Record: " + (GameManager.Instance.highScoreWave+1) + " Waves";
        }
    }
}