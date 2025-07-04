using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;

public class ClockUsageStrategy : IItemSpellUsage
{
    public bool Execute(ItemDetails itemDetail)
    {
        Debug.Log("HAHAHA");
        if (GlobalTimeManager.Instance == null) return false;


        // 修改全局时间
        GlobalTimeManager.Instance.ModifyTime(0.5f);
        //角色加速
        PlayerMovement.Instance.UpdateMaxVelocity(2.0f);
        Debug.Log($"使用时钟道具，时间流速减速");
        Camera.main.transform.GetChild(0).GetComponent<PostProcessVolume>().weight = 1.0f;
        GlobalTimeManager.Instance.TimeRecover();

        return true;
    }
}

    public class PlacePrefabStrategy : IItemSpellUsage
{
        public bool Execute(ItemDetails itemDetail)
        {
            if (itemDetail.prefabToSpawnPath == null) return false;

            Vector3 spawnPos = PlayerMovement.Instance.GetSpawnPosition();
            GameObject obj = ResourceManager.LoadPrefab(itemDetail.prefabToSpawnPath);
            GameObject.Instantiate(obj, spawnPos, Quaternion.identity);
            return true;
        }
    }
