using UnityEngine;
public class CatDamage : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        IDamageable health = other.gameObject.GetComponentInChildren<IDamageable>();

        if (health == null)
        {
            health = other.gameObject.GetComponentInParent<IDamageable>();
        }

        if (health != null)
        {
            health.TakeDamage(25);

            // PLAY PLAYER HURT SOUND
            SoundManager.PlaySound(SoundType.Player_Hurt);
        }
    }
}
