using System.Collections;
using System.Collections.Generic; // List
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI; // Image component

public class WaveManager : MonoBehaviour
{
    public GameObject[] powerupPrefabs; // Array of powerups
    public Transform[] powerupSpawnPoints; // Array of powerup spawnpoints
    public float powerupSpawnChance = 0.15f;

    public List<WaveData> waves;
    public Transform[] enemySpawnPoints; // Array of enemy spawnpoints

    private int _currentWaveIndex;
    private int _enemiesRemaining;

    [Header("Wave Clear Settings")]

    [Header("Flash Settings")]
    public Image flashImage;
    public float flashDuration = 1f;


    [Header("Boss Intro Settings")]
    public GameObject[] bossPrefabs;
    public int bossSpawnRound = 5;
    public float bossSpawnBottomY = -20f; // Starting point (below arena)
    public float bossArenaFlightY = 15f;  // Final hovering height
    public float ascensionDuration = 8f;

    [Header("Flash Color")]
    public Color winFlashColor = new Color(0, 1, 0, 0.4f); // Green at 40% alpha
    public Color bossFlashColor = new Color(1, 1, 1, 0.4f); // White at 40% alpha


    void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterWaveManager(this);
        }
        else
        {
            Debug.LogError("WaveManager: GameManager.Instance is still null in Start");
        }
        _currentWaveIndex = 0;
        StartNextWave();
    }

    IEnumerator WaveClearRoutine()
    {
        // PLAY WAVE CLEAR SOUND
        SoundManager.PlaySound(SoundType.BGM_Clear);

        // Flash Green
        if (flashImage != null)
        {
            flashImage.color = winFlashColor;

            // Fade out the green flash
            float elapsed = 0f;
            while (elapsed < flashDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(winFlashColor.a, 0, elapsed / flashDuration);
                flashImage.color = new Color(winFlashColor.r, winFlashColor.g, winFlashColor.b, alpha);
                yield return null;
            }
        }

        // Spawn specific powerup
        Transform spawnPoint = powerupSpawnPoints[4]; // midair spawnpoint
        Instantiate(powerupPrefabs[0], spawnPoint.position, Quaternion.identity);
        Debug.Log("Powerup spawned");

        // Pause
        yield return new WaitForSeconds(5.0f);
        

        // Start the next wave
        StartNextWave();
    }

    public void StartNextWave()
    {
        Debug.Log("Starting Wave " + (_currentWaveIndex + 1));
        if (_currentWaveIndex < waves.Count)
            {
                if (_currentWaveIndex == (bossSpawnRound-1))
            {
                StartCoroutine(BossAscensionIntro());

            }
            StartCoroutine(SpawnRoutine(waves[_currentWaveIndex]));
        }
    }

    IEnumerator SpawnRoutine(WaveData data)
    {
        _enemiesRemaining = data.totalToSpawn;


        for (int i = 0; i < data.totalToSpawn; i++)
        {
            // Pick random enemy + random spawn point
            int rand = UnityEngine.Random.Range(0, data.enemyPrefabs.Length);
            GameObject prefab = data.enemyPrefabs[rand];

            Transform spot;
            // Stompers should not be able to spawn on cloud platforms (unreactable)
            if (rand == 3) // If it is a stomper ->
            {
                spot = enemySpawnPoints[UnityEngine.Random.Range(0, 9)]; // Leaf spawns (omit cloud spawns)
            }
            else // All other enemies
            {
                spot = enemySpawnPoints[UnityEngine.Random.Range(0, enemySpawnPoints.Length)]; // All spawns
            }

            Instantiate(prefab, spot.position, spot.rotation);

            if (UnityEngine.Random.value < powerupSpawnChance) // powerup spawn chance (random)
            {
                SpawnMidRoundPowerup();
            }

            yield return new WaitForSeconds(data.timeBetweenSpawns);
        }
    }

    IEnumerator BossAscensionIntro()
    {
        // Spawn Position (Below the center of the arena)
        Vector3 spawnPos = new Vector3(0, bossSpawnBottomY, 0);
        GameObject boss = Instantiate(bossPrefabs[0], spawnPos, Quaternion.identity);

        // Grab boss controller (ensure canAct is false initially)
        BossController controller = boss.GetComponent<BossController>();

        // Ascension
        float elapsed = 0;
        Vector3 endPos = new Vector3(0, bossArenaFlightY, 0);

        while (elapsed < ascensionDuration)
        {
            boss.transform.position = Vector3.Lerp(spawnPos, endPos, elapsed / ascensionDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        boss.transform.position = endPos;

        yield return new WaitForSeconds(5);
        SoundManager.PlaySound(SoundType.Cat_Start, 0.4f);

        // Boss canAct = true
        controller.BeginBossFight();

        if (flashImage != null)
        {
            flashImage.color = bossFlashColor;

            // Fade out white flash
            elapsed = 0f;
            while (elapsed < flashDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(bossFlashColor.a, 0, elapsed / flashDuration);
                flashImage.color = new Color(bossFlashColor.r, bossFlashColor.g, bossFlashColor.b, alpha);
                yield return null;
            }
        }

    }

    private void SpawnMidRoundPowerup()
    {
        // Ensure array is assigned and not empty
        if (powerupPrefabs.Length == 0 || powerupSpawnPoints.Length == 0) return;

        // Pick random powerup from array
        GameObject pPrefab = powerupPrefabs[UnityEngine.Random.Range(0, powerupPrefabs.Length)];

        // Pick a random spawn point from array
        int randomIndex = UnityEngine.Random.Range(0, powerupSpawnPoints.Length);
        Transform spawnPoint = powerupSpawnPoints[randomIndex];

        Instantiate(pPrefab, spawnPoint.position, Quaternion.identity);
        Debug.Log($"Mid-round powerup spawned at point {randomIndex}");
    }

    // Called by EnemyHealth script when an enemy dies
    public void OnEnemyDefeated()
    {
        _enemiesRemaining--;
        Debug.Log(_enemiesRemaining + " enemies remaining");

        // When the wave is cleared -> move to next index and start next wave routine
        if (_enemiesRemaining <= 0)
        {
            _currentWaveIndex++;
            StartCoroutine(WaveClearRoutine());
        }
    }

    // Whenever ANY EnemyHealth fires OnDeath -> run local method
    private void OnEnable()
    { 
        EnemyHealth.OnDeath += HandleEnemyDeath;
    }

    // Crucial for memory management
    private void OnDisable()
    {
        EnemyHealth.OnDeath -= HandleEnemyDeath;
    }

    // Event -> Local method
    private void HandleEnemyDeath(EnemyHealth enemy)
    {
        OnEnemyDefeated();
    }

    // UI
    public int EnemiesRemaining => _enemiesRemaining;
    public int CurrentWave => _currentWaveIndex;
}
