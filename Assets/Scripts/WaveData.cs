using UnityEngine;

[CreateAssetMenu(fileName = "NewWave", menuName = "Arena/Wave Data")]
public class WaveData : ScriptableObject
{
    public GameObject[] enemyPrefabs; // Which enemies
    public int totalToSpawn;         // Total per wave
    public float timeBetweenSpawns;  // Spawnrate speed
}