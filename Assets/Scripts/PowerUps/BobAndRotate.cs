using UnityEngine;

public class BobAndRotate : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private Vector3 rotationSpeed = new Vector3(0, 100, 0);

    [Header("Bobbing Settings")]
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private float floatAmplitude = 0.5f;

    private Vector3 startPosition;

    void Start()
    {
        // Capture the initial position (bob around the correct starting point)
        startPosition = transform.position;
    }

    void Update()
    {
        // Rotation
        transform.Rotate(rotationSpeed * Time.deltaTime);

        // Bobbing
        // Mathf.Sin returns a value between -1 and 1 (down <-> up)
        float newY = startPosition.y + (Mathf.Sin(Time.time * floatSpeed) * floatAmplitude);

        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}