using UnityEngine;

public class DeathZone : MonoBehaviour
{
    public Health player;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Triggered Death Zone");
            GameObject playerRoot = other.transform.root.gameObject; // Get parent of PlayerObject
            GameManager.Instance.HandlePlayerDeath(playerRoot);
        }

        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Enemy Triggered Death Zone");

            // Trigger Death for enemies that fall
            IDamageable health = other.GetComponentInParent<IDamageable>();

            if (health != null)
            {
                health.Kill();
            }
        }
    }
}