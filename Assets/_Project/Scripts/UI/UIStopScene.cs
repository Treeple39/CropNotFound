using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static bool IsPaused { get; private set; }
    public GameObject pauseIcon;
    public GameObject resumeIcon;

    public void TogglePause()
    {
        IsPaused = !IsPaused;

        Time.timeScale = IsPaused ? 0 : 1;
        pauseIcon.SetActive(!IsPaused);
        resumeIcon.SetActive(IsPaused);

        AudioListener.pause = IsPaused;

        Debug.Log($"”Œœ∑“—{(IsPaused ? "‘›Õ£" : "ª÷∏¥")}");
    }
}