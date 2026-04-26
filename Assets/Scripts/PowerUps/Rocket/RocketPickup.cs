using UnityEngine;

public class RocketPickup : MonoBehaviour
{
    [SerializeField] private int ammoGrant = 5;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Transform playerRoot = other.transform.root;

            // Switcher enables rocket
            WeaponSwitcher switcher = playerRoot.GetComponentInChildren<WeaponSwitcher>();
            if (switcher != null) switcher.SwitchToRocket();

            // Add ammo
            RocketLauncher rl = playerRoot.GetComponentInChildren<RocketLauncher>();
            if (rl != null)
            {
                rl.GrantWeapon(ammoGrant);
                Destroy(gameObject);
            }
        }
    }
}