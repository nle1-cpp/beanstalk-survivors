using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Triggered Death Zone");
        }

        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Enemy Triggered Death Zone");
            Destroy(other);
        }
    }
}