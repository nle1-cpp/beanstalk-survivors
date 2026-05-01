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
        // Wait a bit after throwing
        yield return new WaitForSeconds(0.75f);

        // Spawn the platform at jar's current position
        if (platformPrefab != null)
        {
            Instantiate(platformPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}