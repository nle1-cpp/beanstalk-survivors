using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform player;
    public float stoppingDistance = 8f;

    [Header("Movement Settings")]
    public float flySpeed = 5f;
    public float rotationSpeed = 50f;

    [Header("Dash Settings")]
    public float minDashSpeed = 25f;
    public float maxDashSpeed = 50f;
    public float chargeTime = 1.5f;   // Time spent "winding up"
    public float dashDuration = 0.5f; // Short, snappy burst
    public float attackCooldown = 4f;

    private Rigidbody rb;
    private bool isDashing = false;
    private float lastAttackTime;

    private bool canAct = false; // Bool guard to wait for animation before acting

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        if (player == null)
        {
            GameObject foundPlayer = GameObject.Find("PlayerObject");
            if (foundPlayer != null)
            {
                player = foundPlayer.transform;
            }
            else
            {
                Debug.LogError("BossController: Could not find a GameObject named 'PlayerObject' in the scene!");
            }
        }
    }

        void FixedUpdate()
    {
        // Perpetual Rotation
        Quaternion deltaRotation = Quaternion.Euler(new Vector3(0, rotationSpeed * Time.fixedDeltaTime, 0));
        rb.MoveRotation(rb.rotation * deltaRotation);

        if (!canAct || isDashing || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Attack Trigger Logic (Only attack if close enough AND cooldown is ready)
        if (Time.time >= lastAttackTime + attackCooldown && distance <= stoppingDistance)
        {
            StartCoroutine(PerformDash());
            return;
        }

        // Movement Logic
        if (distance > stoppingDistance)
        {
            Vector3 dir = (player.position - transform.position).normalized;
            rb.MovePosition(transform.position + dir * flySpeed * Time.fixedDeltaTime);
        }
        else
        {
            // Passive Brake: Stops the boss from drifting past the player
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, Time.fixedDeltaTime * 4f);
        }
    }

    IEnumerator PerformDash()
    {
        isDashing = true;
        lastAttackTime = Time.time;

        SoundManager.PlaySound(SoundType.Cat_Windup, 0.4f);

        Vector3 awayDir = (transform.position - player.position).normalized;
        float anticipationSpeed = 2f;
        float timer = 0;

        while (timer < chargeTime)
        {
            // Use velocity instead of AddForce for a controlled "glide" back
            rb.linearVelocity = awayDir * anticipationSpeed;
            timer += Time.deltaTime;
            yield return null; // Wait for next frame
        }

        // LOCK-IN & DASH
        // Zero out the backward drift so the dash is clean
        rb.linearVelocity = Vector3.zero;

        // Recalculate target vector at LAST possible moment
        Vector3 finalDashDir = (player.position - transform.position).normalized;
        float currentDashSpeed = Random.Range(minDashSpeed, maxDashSpeed);

        rb.AddForce(finalDashDir * currentDashSpeed, ForceMode.Impulse);

        SoundManager.PlaySound(SoundType.Cat_Charge, 0.4f);

        yield return new WaitForSeconds(dashDuration);

        // RECOVERY
        rb.linearVelocity = Vector3.zero;
        yield return new WaitForSeconds(attackCooldown);
        isDashing = false;
    }
    public void BeginBossFight()
    {
        canAct = true;
    }
}