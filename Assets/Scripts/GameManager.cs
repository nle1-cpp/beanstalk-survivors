using UnityEngine;
using UnityEngine.SceneManagement; // Used to scene switch

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public WaveManager activeWaveManager { get; private set; }

    [Header("Temporary Respawn Settings")]
    public Transform playerRespawnPoint;

    public GameObject gameOverUI;

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

    // TEMPORARILY SENDS THEM BACK TO THE ARENA
    public void HandlePlayerDeath(GameObject player)
    {
        if (playerRespawnPoint != null)
        {
            player.transform.position = playerRespawnPoint.position;
            player.transform.rotation = playerRespawnPoint.rotation;

            Debug.Log("Player respawned at: " + playerRespawnPoint.position);
        }
    }

    public void GameOver()
    {
        gameOverUI.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
    }

    public void ResetToTitle()
    {
        SceneManager.LoadScene("TitleScreen");
    }
}