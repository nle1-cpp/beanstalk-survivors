using UnityEngine;

public class DeathZone : MonoBehaviour
{
    public Health player;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Triggered Death Zone");
            player.Kill();
        }

        if (other.CompareTag("Enemy") || other.CompareTag("Stomper"))
        {
            Debug.Log("Enemy Triggered Death Zone");

            // Trigger Death for enemies that fall
            IDamageable enemy = other.GetComponentInParent<IDamageable>();
            if (enemy != null)
            {
                enemy.Kill();
            }
        }
    }
}