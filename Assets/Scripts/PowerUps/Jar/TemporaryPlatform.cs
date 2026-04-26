using UnityEngine;

public class TemporaryPlatform : MonoBehaviour
{
    [SerializeField] private float platformTimer = 5f;
    private void Start()
    {
        // Automatically destroy platform after X seconds
        Destroy(gameObject, platformTimer);
    }
}