using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInit : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(InitManager());
    }

    IEnumerator InitManager()
    {
        yield return new WaitUntil(() => DataManager.Instance != null);
        DataManager.Instance.Init();
        yield return null;
        UnlockManager.Instance.Init();
        yield return null;
        //UIManager.Instance.Init();
        //yield return null;
        //SceneManager.Instance.Init();
        //yield return null;
        TechLevelManager.Instance.Init();
        yield return null;
        InventoryManager.Instance.Init();
        yield return null;
        ScoreDetector.Instance.Init();

        BuffApplicationManager.Instance.ApplyAllUnlockedBuffs();
    }
}
