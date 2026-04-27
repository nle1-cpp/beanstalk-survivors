using UnityEngine;
using System.Collections;
using UnityEngine.AI; // Required for Coroutines

public class StomperAI : EnemyAI
{
    private NavMeshAgent agentNav;
    private Rigidbody rb;
    public GameObject cloud1;
    public GameObject cloud2;
    public GameObject cloud3;
    public GameObject cloud4;

    protected override void Awake()
    {
        // Parent Start logic
        base.Awake();

        cloud1 = GameObject.Find("Cloud1");
        cloud2 = GameObject.Find("Cloud2");
        cloud3 = GameObject.Find("Cloud3");
        cloud4 = GameObject.Find("Cloud4");

        // For falling through platforms (disable agent nav and rigidbody)
        agentNav = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
    }

    protected override void UpdateEnemyLogic()
    {
        // Chaser AI Nav
        _agent.SetDestination(_player.position);
    }

    // DAMAGE LOGIC (Solid vs Solid)
    private void OnCollisionEnter(Collision collision)
    {
        IDamageable health = collision.gameObject.GetComponentInChildren<IDamageable>();

        if (health == null)
        {
            health = collision.gameObject.GetComponentInParent<IDamageable>();
        }

        if (health != null)
        {
            health.TakeDamage(10);

            // PLAY PLAYER HURT SOUND
            SoundManager.PlaySound(SoundType.Player_Hurt);

        }
    }

    // Cloud Deactivation
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cloud"))
        {
            // Trigger sensor allows the Kinematic AI to "see" the platform
            StartCoroutine(CloudVanish(other.gameObject));
        }
    }

    IEnumerator CloudVanish(GameObject cloud)
    {
        cloud.SetActive(false);
        agentNav.enabled = false;
        rb.isKinematic = false;
        yield return new WaitForSeconds(2f);
        cloud.SetActive(true);
    }
}