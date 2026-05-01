using UnityEngine;

public class MeleeHitboxSensor : MonoBehaviour
{
    public MeleeWeapon weaponLogic;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Stomper"))
        {
            weaponLogic.ProcessHit(other);
        }
    }
}