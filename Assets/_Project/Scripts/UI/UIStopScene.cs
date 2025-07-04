using UnityEngine;

public class GlobalTimeManager : Singleton<GlobalTimeManager>
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

    public void ModifyTime(float timeScale)
    {
        Time.timeScale = timeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }
}