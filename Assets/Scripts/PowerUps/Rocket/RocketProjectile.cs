using JetBrains.Annotations;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RocketProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionDamage = 50f;
    [SerializeField] private LayerMask enemyLayer;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    public void Initialize(Vector3 direction)
    {
        rb.linearVelocity = direction * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    private void Explode()
    {
        // Only deal damage to enemies (Enemy layer)

        // AoE Damage
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, enemyLayer);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(explosionDamage);
            }
        }

        Destroy(gameObject);
    }
}