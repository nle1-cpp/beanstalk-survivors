using UnityEngine;
using UnityEngine.SceneManagement; // Used to scene switch

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public WaveManager activeWaveManager { get; private set; }
    public Health activePlayer { get; private set; }

    public GameObject gameOverUI;
    public int highScoreWave;

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
    public void RegisterPlayer(Health player)
    {
        activePlayer = player;
        Debug.Log("Player successfully registered to GameManager.");
    }

    public void StartRun()
    {
        // Switch to the Arena scene
        SceneManager.LoadScene("ArenaScene");
    }

    public void StartArena()
    {
        if (activeWaveManager != null)
        {
            activeWaveManager.StartNextWave();
        }
    }

    public void GameOver()
    {
        if (gameOverUI != null) // If UI is NOT active and UI exists (doesn't exist in title screen)
        {
            gameOverUI.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void Retry()
    {
        if (gameOverUI != null) gameOverUI.SetActive(false); // Hide GameOver Panel
        Time.timeScale = 1f;
        SceneManager.LoadScene("ArenaScene");
    }

    public void ResetToTitle()
    {
        UpdateHighScore(activeWaveManager.CurrentWave);
        Time.timeScale = 1f;
        SceneManager.LoadScene("TitleScreen");
        
    }

    public void UpdateHighScore(int wavesReached)
    {
        if (wavesReached > highScoreWave)
        {
            highScoreWave = wavesReached;
        }
    }



    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }

    void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "ArenaScene")
        {
            // Look for Canvas first (more efficient)
            Canvas sceneCanvas = FindFirstObjectByType<Canvas>();
            if (sceneCanvas != null)
            {
                // Search the children of Canvas for GameOver panel
                Transform target = sceneCanvas.transform.Find("GameOver");
                if (target != null)
                {
                    gameOverUI = target.gameObject;
                    gameOverUI.SetActive(false);
                    Debug.Log("GameManager found new GameOver UI.");
                }
            }

            // Reset Player Properties
            if (activePlayer != null)
            {
                activePlayer.Reset();
            }
        }
    }
}