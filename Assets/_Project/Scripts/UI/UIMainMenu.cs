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
        if (!Score.skipAnim) {
            Score.AnimSkip();
            GameManager.Instance.StartStory(); 
        }
        else
        {
            GameManager.Instance.StartLevel();
        }

    }

    public void DrawCard() {

        GameManager.Instance.GoToCardScene();
    }
}
