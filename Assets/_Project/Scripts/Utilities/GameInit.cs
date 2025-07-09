using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInit : MonoBehaviour
{
    void Awake()
    {
        StartCoroutine(InitManager());
    }

    IEnumerator InitManager()
    {
        yield return new WaitUntil(() => DataManager.Instance != null);
        DataManager.Instance.Init();
        yield return null;
        //UIManager.Instance.Init();
        //yield return null;
        //SceneManager.Instance.Init();
        //yield return null;
    }
}
