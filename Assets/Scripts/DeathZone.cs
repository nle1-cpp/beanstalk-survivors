using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Triggered Death Zone");
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