using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 10f;
    public float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime); // Cleanup if misses
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponentInParent<Health>();

            playerHealth.TakeDamage(damage);
            
            // PLAY PLAYER HURT SOUND
            SoundManager.PlaySound(SoundType.Player_Hurt_Projectile);


            Destroy(gameObject);
        }
    }
}