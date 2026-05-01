using UnityEngine;

public class DashHitboxSensor : MonoBehaviour
{
    public DashAbility dashLogic;

    private void OnTriggerEnter(Collider other)
    {
        // Ensure the dash is actually active and we hit an enemy
        if (other.CompareTag("Enemy") || other.CompareTag("Stomper"))
        {
            dashLogic.ProcessHit(other);
        }
    }
}