using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    public float maxHealth = 100f;
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log("-" + amount + "HP" + "(" + currentHealth + "HP)");
        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        Debug.Log("Trigger Death");
        //if (CompareTag("Player")) GameManager.Instance.ResetToTitle();
        //else Destroy(gameObject);
    }

    public void Kill()
    {
        Debug.Log("Killed Player");
    }
}