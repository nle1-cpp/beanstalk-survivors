using UnityEngine;
using UnityEngine.UI; // Required for Image component
using System.Collections; // Required for Coroutines
public class Health : MonoBehaviour, IDamageable
{
    public float maxHealth = 100f;
    private float currentHealth;
    public GameManager gameManager;
    private bool isDead;

    [Header("UI References")]
    public Image damageFlashImage;
    public float flashDuration = 0.2f;
    public Color flashColor = new Color(1, 0, 0, 0.4f); // Red with some transparency

    void Start()
    {
        currentHealth = maxHealth;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterPlayer(this);
        }

        if (damageFlashImage != null)
            damageFlashImage.color = new Color(flashColor.r, flashColor.g, flashColor.b, 0);
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (damageFlashImage != null)
        {
            StopAllCoroutines(); // Reset if hit again quickly
            StartCoroutine(FlashRoutine());
        }

        Debug.Log("-" + amount + "HP" + "(" + currentHealth + "HP)");
        if (currentHealth <= 0 && !isDead) Die();
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        Debug.Log("+" + amount + "HP" + "(" + currentHealth + "HP)");
    }    

    private IEnumerator FlashRoutine()
    {
        // Red with 50% transparency
        damageFlashImage.color = flashColor;

        float duration = 0.2f;
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0.5f, 0, elapsed / duration);
            damageFlashImage.color = new Color(1, 0, 0, alpha);
            yield return null;
        }
    }

    public void Die()
    {
        isDead = true;
        GameManager.Instance.GameOver();
        Time.timeScale = 0f;
        Debug.Log("Trigger Death");

    }

    public void Kill()
    {
        currentHealth = 0;
        Debug.Log("Killed Player");
        Die();
    }

    public void Reset()
    {
        currentHealth = maxHealth;
        Debug.Log($"Initiated player with {currentHealth} health");

        isDead = false;
        Debug.Log($"Player is alive");

        Vector3 startPosition = new Vector3(20f, 3f, -20f);
        GameObject playerRoot = transform.root.gameObject;
        playerRoot.transform.position = startPosition;
        Debug.Log($"Player teleported to {startPosition}");
    }

    public int CurrentHealth => (int)currentHealth; // UI
}