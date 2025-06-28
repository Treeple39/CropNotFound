using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static bool IsPaused { get; private set; }

    public void TogglePause()
    {
        IsPaused = !IsPaused;

        Time.timeScale = IsPaused ? 0 : 1;
        AudioListener.pause = IsPaused;

        Debug.Log($"”Œœ∑“—{(IsPaused ? "‘›Õ£" : "ª÷∏¥")}");
    }
}