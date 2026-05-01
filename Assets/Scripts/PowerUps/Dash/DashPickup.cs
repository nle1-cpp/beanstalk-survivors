using UnityEngine;

public class DashPickup : MonoBehaviour
{

    [SerializeField] private int Winds = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Transform playerRoot = other.transform.root;

            // Add jar
            DashAbility Dash = playerRoot.GetComponentInChildren<DashAbility>();
            if (Dash != null)
            {
                Dash.GrantDash(Winds);

                // PLAY PICKUP SOUND
                SoundManager.PlaySound(SoundType.Weapon_Pickup);

                Destroy(gameObject);
            }
        }
    }
}