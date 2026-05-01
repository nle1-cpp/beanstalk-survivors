using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class DashAbility : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashForce = 25f;      // Intensity of the burst
    public float dashHitboxDuration = 1f; // How long the kill-zone stays active
    public float attackCooldown = 5f;
    public int damageAmount = 50;

    [Header("References")]
    public GameObject hitbox;
    public PlayerMovement movementScript;
    public Transform playerCamera; // Drag your Main Camera here
    public GameObject playerCapsule;

    private bool isDashing = false;
    public int numDashes = 0;

    private void Awake()
    {
        numDashes = 0;
    }

    public void OnSprint(InputValue value)
    {
        if (value.isPressed && !isDashing && numDashes > 0)
        {
            StartCoroutine(PerformDash());
            numDashes--;
        }
    }

    IEnumerator PerformDash()
    {
        isDashing = true;
        hitbox.SetActive(true);

        // Capture where the camera is looking at the moment of the press
        Vector3 dashDirection = playerCamera.forward;

        // PlayerMovement launches player
        movementScript.ApplyDashForce(dashDirection, dashForce);

        // Keep the killing hitbox active while the player is launched forward
        yield return new WaitForSeconds(dashHitboxDuration);

        hitbox.SetActive(false);

        // Wait for cooldown
        yield return new WaitForSeconds(attackCooldown);
        isDashing = false;
    }

    public void ProcessHit(Collider other)
    {
        IDamageable damageable = other.GetComponentInParent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damageAmount);
            // We don't disable the hitbox so we can "multikill" enemies in one line
        }
    }

    public void GrantDash(int dashAmount)
    {
        numDashes += dashAmount;
    }
}