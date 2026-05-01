using UnityEngine;

public class SceneOptions : MonoBehaviour
{
    public void TriggerRetry()
    {
        // Always find the 'Survivor' GameManager that didn't get deleted
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Retry();
        }
    }
    public void TriggerTitle()
    {
        // Always find the 'Survivor' GameManager that didn't get deleted
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetToTitle();
        }
    }

    public void TriggerArena()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartRun();
        }
    }
}