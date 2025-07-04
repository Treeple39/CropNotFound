using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory;

    public class ClockUsageStrategy : IItemSpellUsage
{
        public bool Execute(ItemDetails itemDetail)
        {
            if (GlobalTimeManager.Instance == null) return false;

            // �޸�ȫ��ʱ�䣨ʾ��ֵ������ʵ�����������
            GlobalTimeManager.Instance.ModifyTime(0.5f);
            //��ɫ����
            PlayerMovement.Instance.UpdateMaxVelocity(2.0f);
            Debug.Log($"ʹ��ʱ�ӵ��ߣ�ʱ�����ټ���");
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
