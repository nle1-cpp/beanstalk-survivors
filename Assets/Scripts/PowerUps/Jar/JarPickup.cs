using UnityEngine;

public class JarPickup : MonoBehaviour
{

    [SerializeField] private int jarGrant = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Transform playerRoot = other.transform.root;

            // Switcher enables jar
            WeaponSwitcher switcher = playerRoot.GetComponentInChildren<WeaponSwitcher>();
            if (switcher != null) switcher.SwitchToJar();

            // Add jar
            JarAbility ja = playerRoot.GetComponentInChildren<JarAbility>();
            if (ja != null)
            {
                ja.GrantJar(jarGrant);

                // PLAY PICKUP SOUND
                SoundManager.PlaySound(SoundType.Weapon_Pickup);

                Destroy(gameObject);
            }
        }
    }
}