using UnityEngine;

public class CrusherAI : EnemyAI
{
    protected override void UpdateEnemyLogic()
    {
        // Chaser AI nav
        _agent.SetDestination(_player.position);
    }

    private void OnCollisionEnter(Collision collision)
    {
        IDamageable health = collision.gameObject.GetComponentInChildren<IDamageable>();

        if (health == null)
        {
            health = collision.gameObject.GetComponentInParent<IDamageable>();
        }

        if (health != null)
        {
            health.TakeDamage(20);
        }
    }
}