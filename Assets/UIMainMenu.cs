using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] private string sceneName = "Animation";

    public void ContinueGame()
    {
        SceneManager.LoadScene(sceneName);
    }

    public void NewGame() {
        //SceneManager.instance.DeleteSaveData();
        //GameManager.Instance.StartStory();
    }

    public void ExitGame() { 
        
        
    }
}
