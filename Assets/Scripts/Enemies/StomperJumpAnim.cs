using UnityEngine;
using UnityEngine.AI;

public class NavMeshAnimationTrigger : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent parentAgent;
    private bool hasTriggered = false;

    void Start()
    {
        animator = GetComponent<Animator>();

        // Look at the parent for the NavMeshAgent
        parentAgent = GetComponentInParent<NavMeshAgent>();

        if (parentAgent == null)
        {
            Debug.LogError("No NavMeshAgent found on parent of " + gameObject.name);
        }
    }

    void Update()
    {
        // Check if the agent exists and has been disabled (enabled == false)
        if (parentAgent != null && !parentAgent.enabled && !hasTriggered)
        {
            TriggerAnimation();
        }

        // Optional: Reset if the agent is re-enabled later
        if (parentAgent != null && parentAgent.enabled)
        {
            hasTriggered = false;
        }
    }

    void TriggerAnimation()
    {
        if (animator != null)
        {
            // Replace "StartAction" with your actual trigger name
            animator.SetTrigger("StartAction");
            hasTriggered = true;
            Debug.Log("Parent NavMeshAgent disabled - Triggering Animation");
        }
    }
}