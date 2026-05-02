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
            if (agent.isOnOffMeshLink)
            {
                yield return StartCoroutine(ParabolicJump(agent, jumpHeight, jumpDuration));
                agent.CompleteOffMeshLink();
            }
            yield return null;
        }
    }

    IEnumerator ParabolicJump(NavMeshAgent agent, float height, float duration)
    {
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 startPos = agent.transform.position;
        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
        float normalizedTime = 0.0f;

        while (normalizedTime < 1.0f)
        {
            float yOffset = height * 4.0f * (normalizedTime - normalizedTime * normalizedTime);
            agent.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
    }
}