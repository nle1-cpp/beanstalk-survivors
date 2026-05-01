using UnityEngine;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [Header("UI Text Elements")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI jarCountText;
    [SerializeField] private TextMeshProUGUI enemiesText;
    [SerializeField] private TextMeshProUGUI waveText;

    [Header("Game References")]
    [SerializeField] private Health playerHealth;
    [SerializeField] private RocketLauncher rocketLauncher;
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private JarAbility jarAbility;

    void Update()
    {
        // Health
        if (playerHealth != null) healthText.text = $"{playerHealth.CurrentHealth}";

        // Enemies Remaining in wave + wave number
        if (waveManager != null)
        {
            enemiesText.text = $"Remaining: {waveManager.EnemiesRemaining}";
            waveText.text = $"{waveManager.CurrentWave + 1}";
        }

        // Rocket Ammo Logic
        if (rocketLauncher != null) ammoText.text = $"{rocketLauncher.currentAmmo}";

        // Jar Count Logic
        if (jarAbility != null) jarCountText.text = $"{jarAbility.numJars}";
    }
}