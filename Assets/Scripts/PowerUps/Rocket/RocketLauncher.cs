using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class RocketLauncher : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject rocketPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private WeaponSwitcher switcher;

    [Header("Rocket Jump Settings")]
    [SerializeField] private float recoilForce = 10f;
    [SerializeField] private float fireCooldown = 2.5f;

    [Header("Ammo")]
    public int currentAmmo = 0;
    public int maxAmmo = 10;

    [Header("Debug")]
    private bool isAttacking = false;

    public void OnAttack(InputValue value)
    {
        if (value.isPressed && !isAttacking && currentAmmo > 0)
        {
            StartCoroutine(FireRoutine());
        }
    }

    private IEnumerator FireRoutine()
    {
        isAttacking = true;
        currentAmmo--;

        // Recoil (Rocket Jump); Reset vertical velocity for consistent jump height
        playerRigidbody.linearVelocity = new Vector3(playerRigidbody.linearVelocity.x, 0, playerRigidbody.linearVelocity.z);

        // Push the player in opposite direction of firePoint
        Vector3 recoilVector = -firePoint.forward * recoilForce;
        playerRigidbody.AddForce(recoilVector, ForceMode.Impulse);

        // Spawn Projectile
        GameObject rocketObj = Instantiate(rocketPrefab, firePoint.position, firePoint.rotation);

        RocketProjectile rocket = rocketObj.GetComponent<RocketProjectile>();
        if (rocket != null)
        {
            rocket.Initialize(firePoint.forward); // Projectile only handles its own flight
        }

        yield return new WaitForSeconds(fireCooldown);
        isAttacking = false;
    }

    public void GrantWeapon(int ammoAmount)
    {
        if (currentAmmo <= (maxAmmo - ammoAmount))
        {
            currentAmmo += ammoAmount;
        }
        else
        {
            currentAmmo = maxAmmo;
        }

    }

    private void OnDisable()
    {
        isAttacking = false;
    }
}