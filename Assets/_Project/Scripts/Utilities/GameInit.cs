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
        DataManager.Instance.Init();
        Debug.Log("111110");
        yield return null;
        //UIManager.Instance.Init();
        //yield return null;
        //SceneManager.Instance.Init();
        //yield return null;
    }
}
