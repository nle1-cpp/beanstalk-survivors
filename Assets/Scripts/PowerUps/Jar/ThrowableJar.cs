using UnityEngine;
using System.Collections;

public class ThrowableJar : MonoBehaviour
{
    public GameObject platformPrefab;

    private void Start()
    {
        StartCoroutine(LifeCycle());
    }

    private IEnumerator LifeCycle()
    {
        // Wait 1 second after spawning/throwing
        yield return new WaitForSeconds(1f);

        // Spawn the platform at jar's current position
        if (platformPrefab != null)
        {
            Instantiate(platformPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}