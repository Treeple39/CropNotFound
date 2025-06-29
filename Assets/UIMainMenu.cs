using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] private string sceneName = "Animation";
    private bool skipAnim = false;

    public void ContinueGame()
    {
        SceneManager.LoadScene(sceneName);
    }

    public void NewGame() {
        //SceneManager.instance.DeleteSaveData();
        if (!skipAnim) {
            GameManager.Instance.StartStory(); 
            skipAnim= true;
        }
        else
        {
            GameManager.Instance.StartLevel();
        }

    }

    public void ExitGame() { 
        
        
    }
}
