using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    public float maxHealth = 100f;
    private float currentHealth;
    public GameManager gameManager;
    private bool isDead;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log("-" + amount + "HP" + "(" + currentHealth + "HP)");
        if (currentHealth <= 0 && !isDead) Die();
    }

    public void Die()
    {
        isDead = true;
        gameManager.GameOver();
        Time.timeScale = 0f;
        Debug.Log("Trigger Death");
        //if (CompareTag("Player")) GameManager.Instance.ResetToTitle();
        //else Destroy(gameObject);
    }

    public void Kill()
    {
        Debug.Log("Killed Player");
    }

    public int CurrentHealth => (int)currentHealth; // UI
}