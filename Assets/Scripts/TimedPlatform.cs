using System.Collections;
using UnityEngine;

public class TimedPlatform : MonoBehaviour
{
    private float timeRequired;
    private float timeStayed;
    private bool playerInside;

    private void Start()
    {
        // Choose the random limit once at the start or when the player enters
        // Using 8 so that 7 is a possible outcome (exclusive max)
        timeRequired = Random.Range(3, 8);
    }

    private void Update()
    {
        if (playerInside)
        {
            // Accumulate time based on seconds passed since last frame
            timeStayed += Time.deltaTime;

            // Check if the player has been inside long enough
            if (timeStayed >= timeRequired)
            {
                DeactivatePlatform();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            timeStayed = 0f; // Reset the timer if they leave
        }
    }

    private IEnumerator DeactivatePlatform()
    {
        gameObject.SetActive(false);
        yield return new WaitForSeconds(3); // Reactivate after 3 seconds
        gameObject.SetActive(true);
    }
}