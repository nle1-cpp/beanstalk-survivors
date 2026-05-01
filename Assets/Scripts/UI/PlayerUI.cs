using UnityEngine;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [Header("UI Text Elements")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI jarCountText;
    [SerializeField] private TextMeshProUGUI dashCountText;
    [SerializeField] private TextMeshProUGUI enemiesText;
    [SerializeField] private TextMeshProUGUI waveText;

    [Header("Game References")]
    [SerializeField] private Health playerHealth;
    [SerializeField] private RocketLauncher rocketLauncher;
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private JarAbility jarAbility;
    [SerializeField] private DashAbility dashAbility;

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

        // Rocket Ammo
        if (rocketLauncher != null) ammoText.text = $"{rocketLauncher.currentAmmo}";

        // Jar Count
        if (jarAbility != null) jarCountText.text = $"{jarAbility.numJars}";

        // Dash Count
        if (dashAbility != null) dashCountText.text = $"{dashAbility.numDashes}";
    }
}