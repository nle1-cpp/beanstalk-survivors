using UnityEngine;

public class MeleeHitboxSensor : MonoBehaviour
{
    public MeleeWeapon weaponLogic;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Hitbox touched: {other.name}");

        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Hit object with Enemy tag");
            weaponLogic.ProcessHit(other);
        }
    }
}