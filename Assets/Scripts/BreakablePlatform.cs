using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class BreakablePlatform : MonoBehaviour
{
    [SerializeField] private GameObject _cloudMesh; // Disable the cloud mesh (not the cube mesh)

    [SerializeField] private float _respawnTime = 5.0f;

    private List<GameObject> _occupants = new List<GameObject>();
    private bool _isBroken = false;

    private MeshRenderer _renderer;
    private Collider _collider;
    private NavMeshObstacle _obstacle;

    void Awake()
    {
        _collider = GetComponent<Collider>();
        _obstacle = GetComponent<NavMeshObstacle>(); // Used to carve a hole in NavSurface Mesh

        if (_obstacle != null) _obstacle.carving = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isBroken) return;

        // Check tags
        bool isStomper = other.CompareTag("Stomper");
        bool isEnemy = other.CompareTag("Enemy");

        // Add relevant entities to the fall list first
        if (isStomper || isEnemy)
        {
            if (!_occupants.Contains(other.gameObject))
            {
                _occupants.Add(other.gameObject);
            }

            // ONLY trigger the break sequence if it's the Stomper
            if (isStomper)
            {
                Debug.Log("Stomper detected: Breaking platform!");
                StartCoroutine(BreakSequence());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_occupants.Contains(other.gameObject))
            _occupants.Remove(other.gameObject);
    }

    private IEnumerator BreakSequence()
    {
        // Please keep this in
        SoundManager.PlaySound(SoundType.Stomper_Cloud, 0.3f);
        _isBroken = true;

        // Disable cloud (visual)
        if (_cloudMesh != null) _cloudMesh.SetActive(false);

        // Disable floor
        _collider.enabled = false;

        // Hole in the NavMesh
        if (_obstacle != null) _obstacle.carving = true;

        // Force every entity CURRENTLY on the platform to fall
        foreach (GameObject occupant in _occupants)
        {
            if (occupant == null) continue;

            // Turn off enemy nav
            var agent = occupant.GetComponentInParent<NavMeshAgent>();
            var rb = occupant.GetComponentInParent<Rigidbody>();

            if (agent != null) agent.enabled = false;
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }
        }

        yield return new WaitForSeconds(_respawnTime);

        // Reset Platform
        if (_cloudMesh != null) _cloudMesh.SetActive(true);
        _collider.enabled = true;
        if (_obstacle != null) _obstacle.carving = false;
        _occupants.Clear();

        _isBroken = false;
    }
}