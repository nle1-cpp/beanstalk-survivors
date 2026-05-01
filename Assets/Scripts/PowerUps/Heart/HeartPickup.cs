using UnityEngine;

public class HeartPickup : MonoBehaviour
{
    [SerializeField] private float healAmount = 20;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Transform playerRoot = other.transform.root;

            Health playerHealth = playerRoot.GetComponentInChildren<Health>();
            if (playerHealth != null)
            {
                playerHealth.Heal(healAmount);
            }
        }
        Destroy(gameObject);
    }
}