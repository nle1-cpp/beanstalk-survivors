using UnityEngine;

public class StomperAI : EnemyAI
{

    protected override void UpdateEnemyLogic()
    {
        // Chaser AI Nav
        _agent.SetDestination(_player.position);
    }

    // DAMAGE LOGIC (Solid vs Solid)
    private void OnCollisionEnter(Collision collision)
    {
        IDamageable health = collision.gameObject.GetComponentInChildren<IDamageable>();

        if (health == null)
        {
            health = collision.gameObject.GetComponentInParent<IDamageable>();
        }

        if (health != null)
        {
            health.TakeDamage(10);

            // PLAY PLAYER HURT SOUND
            SoundManager.PlaySound(SoundType.Player_Hurt);

        }
    }
}