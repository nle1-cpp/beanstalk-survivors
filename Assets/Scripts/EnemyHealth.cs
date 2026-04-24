using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    // The Event: Any script can listen, but only this script can fire it.
    // We pass 'this' so the listener knows WHICH enemy died.
    public static event Action<EnemyHealth> OnDeath;

    [SerializeField] private float _health = 100;

    public void TakeDamage(float damage)
    {
        _health -= damage;
        if (_health <= 0) Die();
    }

    private void Die()
    {
        // ?.Invoke safely checks if anyone is actually listening before firing
        OnDeath?.Invoke(this);

        Destroy(gameObject);
    }
}