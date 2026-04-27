using UnityEngine;

public class TemporaryPlatform : MonoBehaviour
{
    [SerializeField] private float platformTimer = 5f;
    private void Start()
    {
        // PLAY SPAWN SOUND
        SoundManager.PlaySound(SoundType.Platform_Spawn);

        // Automatically destroy platform after X seconds
        Destroy(gameObject, platformTimer);
    }
}