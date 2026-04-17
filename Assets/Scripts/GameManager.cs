using UnityEngine;
using UnityEngine.SceneManagement; // Used to scene switch

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
    }

    public void ResetToTitle()
    {
        SceneManager.LoadScene("TitleScreen");
    }
}