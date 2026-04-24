using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class EnemyAI : MonoBehaviour
{
    protected NavMeshAgent _agent;
    protected Transform _player;

    [Header("Base Settings")]
    public float pathUpdateDelay = 0.2f;

    // Children should have their own Start logic
    protected virtual void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _player = GameObject.FindGameObjectWithTag("Player")?.transform;

        StartCoroutine(UpdatePathLoop());
    }

    IEnumerator UpdatePathLoop()
    {
        while (true)
        {
            if (CanUpdatePath())
            {
                UpdateEnemyLogic();
            }
            yield return new WaitForSeconds(pathUpdateDelay);
        }
    }

    // A helper to keep the "if" checks clean
    protected bool CanUpdatePath()
    {
        return _player != null && _agent.isOnNavMesh && _agent.isActiveAndEnabled;
    }

    // Every enemy has their own implementation
    protected abstract void UpdateEnemyLogic();
}