using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyJumpHandler : MonoBehaviour
{
    private NavMeshAgent agent;
    public float jumpHeight = 2.0f; // Adjust for a higher or flatter arch

    public float jumpDuration = 1.2f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        // Disable automatic link traversal to handle it manually
        agent.autoTraverseOffMeshLink = false;
        StartCoroutine(CheckForLinks());
    }

    IEnumerator CheckForLinks()
    {
        while (true)
        {
            // Check if agent is valid and on a link
            if (agent != null && agent.isOnOffMeshLink)
            {
                // Perform the jump and wait for it to finish
                yield return StartCoroutine(ParabolicJump(agent, jumpHeight, jumpDuration));

                // FINAL SAFETY: Only complete the link if the agent is still active 
                // and hasn't fallen into the DeathZone during the jump
                if (agent != null && agent.isOnOffMeshLink)
                {
                    agent.CompleteOffMeshLink();
                }
            }
            yield return null;
        }
    }

    IEnumerator ParabolicJump(NavMeshAgent agent, float height, float duration)
    {
        // DATA VALIDATION: Grab data once at the start
        OffMeshLinkData data = agent.currentOffMeshLinkData;

        // If the link is gone before we even start, abort
        if (!data.valid) yield break;

        Vector3 startPos = agent.transform.position;
        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
        float normalizedTime = 0.0f;

        while (normalizedTime < 1.0f)
        {
            // MID-AIR VALIDATION: If the platform is deactivated while in the air
            // agent.currentOffMeshLinkData.valid will become false
            if (agent == null || !agent.currentOffMeshLinkData.valid)
            {
                // The platform vanished -> Enable gravity so they fall into the void
                agent.enabled = false;
                yield break; // Kill this coroutine
            }

            float yOffset = height * 4.0f * (normalizedTime - normalizedTime * normalizedTime);
            agent.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;

            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
    }
}