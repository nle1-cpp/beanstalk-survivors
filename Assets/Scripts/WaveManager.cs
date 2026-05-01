using System.Collections;
using System.Collections.Generic; // List
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public GameObject[] powerupPrefabs; // Array of powerups
    public Transform powerupSpawnPoint1;
    public Transform powerupSpawnPoint2;
    public float powerupSpawnChance = 0.15f;


    public List<WaveData> waves;
    public Transform[] spawnPoints;

    private int _currentWaveIndex = 0;
    private int _enemiesRemaining;

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

        StartNextWave();
    }

    public void StartNextWave()
    {
        Debug.Log("Starting Wave " + (_currentWaveIndex + 1));
        if (_currentWaveIndex < waves.Count)
        {
            StartCoroutine(SpawnRoutine(waves[_currentWaveIndex]));
        }
    }

    IEnumerator SpawnRoutine(WaveData data)
    {
        _enemiesRemaining = data.totalToSpawn;


        for (int i = 0; i < data.totalToSpawn; i++)
        {
            // Pick random enemy + random spawn point
            GameObject prefab = data.enemyPrefabs[UnityEngine.Random.Range(0, data.enemyPrefabs.Length)];
            Transform spot = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];

            Instantiate(prefab, spot.position, spot.rotation);

            if (UnityEngine.Random.value < powerupSpawnChance)
            {
                SpawnMidRoundPowerup();
            }

            yield return new WaitForSeconds(data.timeBetweenSpawns);
        }
    }

    private void SpawnMidRoundPowerup()
    {
        // Pick a random powerup from array
        GameObject pPrefab = powerupPrefabs[UnityEngine.Random.Range(0, powerupPrefabs.Length)];

        // TEMP: Pick between 2 existing spawn points
        Transform spawnPoint = (UnityEngine.Random.value > 0.5f) ? powerupSpawnPoint1 : powerupSpawnPoint2;

        Instantiate(pPrefab, spawnPoint.position, Quaternion.identity);
        Debug.Log("Mid-round powerup spawned");
    }

    // Called by EnemyHealth script when an enemy dies
    public void OnEnemyDefeated()
    {
        _enemiesRemaining--;
        Debug.Log(_enemiesRemaining + " enemies remaining");

        // When the wave is cleared, move to the next index and start the next wave
        if (_enemiesRemaining <= 0)
        {
            _currentWaveIndex++;
            StartNextWave();
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