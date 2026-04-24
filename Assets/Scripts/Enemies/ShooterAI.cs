using UnityEngine;
using UnityEngine.AI;

public class ShooterAI : EnemyAI
{
    [Header("Range")]
    public float stopDistance = 50f; // Distance to stop and shoot (RANGE)

    [Header("Combat")]
    public GameObject projectilePrefab;
    public Transform firePoint; // A child object where bullets spawn
    public float fireRate = 2f;
    private float _nextFireTime;

    protected override void UpdateEnemyLogic()
    {
        float distance = Vector3.Distance(transform.position, _player.position);

        // AIMING LOGIC
        Vector3 targetCenter = _player.position + (Vector3.up * 0.2f); // (0, 1, 0) * 0.2f = (0, 0.2, 0)
        Vector3 aimDirection = (targetCenter - firePoint.position).normalized;

        // Rotate the firePoint (or the whole enemy) to face the target in 3D
        firePoint.forward = aimDirection;

        // MOVEMENT LOGIC
        if (distance > stopDistance)
        {
            _agent.SetDestination(_player.position);
        }
        else
        {
            _agent.ResetPath();
            if (Time.time >= _nextFireTime)
            {
                Shoot();
                _nextFireTime = Time.time + fireRate;
            }
        }
    }

    void Shoot()
    {
        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
    }
}