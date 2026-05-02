#if PLAYTEST_TOOLS
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public static class PlaytestOverlayBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CreateOverlay()
    {
        GameObject overlay = new GameObject("PlaytestOverlay");
        Object.DontDestroyOnLoad(overlay);
        overlay.AddComponent<PlaytestOverlay>();
    }
}

public sealed class PlaytestOverlay : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private Vector3 spawnPosition;
    private bool hasSpawnPosition;
    private bool isVisible = true;

    private void Awake()
    {
        RefreshPlayerReference();
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.f1Key.wasPressedThisFrame)
        {
            isVisible = !isVisible;
        }

        if (Keyboard.current != null && Keyboard.current.f5Key.wasPressedThisFrame)
        {
            ReloadScene();
        }

        RefreshPlayerReference();
    }

    private void OnGUI()
    {
        if (!isVisible)
        {
            return;
        }

        GUILayout.BeginArea(new Rect(12f, 12f, 340f, 260f), GUI.skin.box);
        GUILayout.Label("Playtest Overlay");
        GUILayout.Label("F1: toggle  F5: reload");

        if (GUILayout.Button("Reload Scene"))
        {
            ReloadScene();
        }

        if (GUILayout.Button("Reset Player"))
        {
            ResetPlayer();
        }

        if (GUILayout.Button(GetCursorButtonLabel()))
        {
            ToggleCursorLock();
        }

        if (GUILayout.Button("Slow Motion"))
        {
            Time.timeScale = 0.5f;
        }

        if (GUILayout.Button("Normal Speed"))
        {
            Time.timeScale = 1f;
        }

        GUILayout.Space(8f);
        GUILayout.Label(GetStatusText());
        GUILayout.EndArea();
    }

    private void RefreshPlayerReference()
    {
        if (playerMovement == null)
        {
            playerMovement = Object.FindFirstObjectByType<PlayerMovement>();
        }

        if (playerMovement != null && !hasSpawnPosition)
        {
            spawnPosition = playerMovement.transform.position;
            hasSpawnPosition = true;
        }
    }

    private void ReloadScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ResetPlayer()
    {
        if (playerMovement == null)
        {
            return;
        }

        if (!hasSpawnPosition)
        {
            spawnPosition = playerMovement.transform.position;
            hasSpawnPosition = true;
        }

        Time.timeScale = 1f;
        playerMovement.ResetForPlaytest(spawnPosition);
    }

    private void ToggleCursorLock()
    {
        bool shouldLock = Cursor.lockState != CursorLockMode.Locked;
        Cursor.lockState = shouldLock ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !shouldLock;
    }

    private string GetCursorButtonLabel()
    {
        return Cursor.lockState == CursorLockMode.Locked ? "Unlock Cursor" : "Lock Cursor";
    }

    private string GetStatusText()
    {
        if (playerMovement == null)
        {
            return "Player not found.";
        }

        Vector3 velocity = playerMovement.Velocity;
        return $"Grounded: {playerMovement.isGrounded}\nVelocity: {velocity}\nAir jumps: {playerMovement.AvailableAirJumps}/{playerMovement.MaxAirJumps}";
    }
}
#endif
