using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public static event Action<EnemyHealth> OnDeath;

    [SerializeField] private float _health = 100;
    private bool _isDead = false;

    public void TakeDamage(float damage)
    {
        Debug.Log("Dealt -" + damage + " dmg to " + gameObject.name);
        _health -= damage;
        if (_health <= 0) Die();
    }

    private void Die()
    {
        if (_isDead) return;

        _isDead = true;
        // ?.Invoke -> Notify the WaveManager
        OnDeath?.Invoke(this);
        Destroy(gameObject);
    }

    public void Kill()
    {
        if (_isDead) return; // Exit if dead already (For the death zone so it trigger doesn't trigger twice; Stompers have 2 colliders)
        Die();
    }
}