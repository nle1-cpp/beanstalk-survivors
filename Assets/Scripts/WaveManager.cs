using System; // Action
using System.Collections.Generic; // List
using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{

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
            Debug.LogError("WaveManager: GameManager.Instance is still null in Start!");
        }

        StartNextWave();
    }

    public void StartNextWave()
    {
        Debug.Log("Starting Wave");
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
            // Pick random enemy from the SO and random spawn point
            GameObject prefab = data.enemyPrefabs[UnityEngine.Random.Range(0, data.enemyPrefabs.Length)];
            Transform spot = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];

            Instantiate(prefab, spot.position, spot.rotation);

            yield return new WaitForSeconds(data.timeBetweenSpawns);
        }
    }

    // Called by EnemyHealth script when an enemy dies
    public void OnEnemyDefeated()
    {
        _enemiesRemaining--;
        if (_enemiesRemaining <= 0)
        {
            _currentWaveIndex++;
        }
    }
}