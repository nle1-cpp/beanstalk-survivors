using UnityEngine;
using UnityEngine.SceneManagement; // Used to scene switch

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public WaveManager activeWaveManager { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Keep GameManager across scenes
    }

    public void RegisterWaveManager(WaveManager manager)
    {
        activeWaveManager = manager;
        Debug.Log("WaveManager successfully registered to GameManager.");
    }

    public void StartArena()
    {
        if (activeWaveManager != null)
        {
            activeWaveManager.StartNextWave();
        }
    }

    public void ResetToTitle()
    {
        SceneManager.LoadScene("TitleScreen");
    }
}