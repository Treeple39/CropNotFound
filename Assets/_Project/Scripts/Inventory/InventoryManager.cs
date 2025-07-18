using Inventory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Inventory
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        //������Դ��DataManager
        public InventoryBag_SO _runtimeInventory;

        protected override void Awake()
        {

            base.Awake();
            DontDestroyOnLoad(gameObject);
        }

        public void Init()
        {
            _runtimeInventory = DataManager.Instance.playerBag;
            //����һ����ұ����ݴ�SO��UI
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, _runtimeInventory.itemList);
        }

        //����DataManager���ߣ��õ���Ʒ��Ϣ
        public ItemDetails GetItemDetail(int ID)
        {
            return DataManager.Instance.GetItemDetail(ID);
            //��������������������
        }

        public void AddItem(int itemID)
        {
            var index = GetItemIndexInBag(itemID);
            AddItemAtIndex(itemID, index, 1); //��ʱдһ����֮��������ӹ���

            //ˢ��UI
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, _runtimeInventory.itemList);
        }


        public void RemoveItem(int itemID, int removeAmount)
        {
            int index = GetItemIndexInBag(itemID);
            if (index == -1) return; //������

            InventoryItem item = _runtimeInventory.itemList[index];
            if (item.itemAmount < removeAmount) return; //��������

            // �����������Ƴ�
            if (item.itemAmount == removeAmount)
            {
                _runtimeInventory.itemList[index] = new InventoryItem { itemID = 0, itemAmount = 0 }; // ��ո���
            }
            else
            {
                _runtimeInventory.itemList[index] = new InventoryItem
                {
                    itemID = itemID,
                    itemAmount = item.itemAmount - removeAmount
                };
            }

            // ˢ��UI
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, _runtimeInventory.itemList);
        }

        // ���Ƴ�ǰ����Ƿ���������
        public bool CanRemoveItem(int itemID, int amount)
        {
            int totalAmount = 0;
            foreach (var item in _runtimeInventory.itemList)
            {
                if (item.itemID == itemID)
                    totalAmount += item.itemAmount;
            }
            return totalAmount >= amount;
        }

        public bool CheckBagCapacity()
        {
            for (int i = 0; i < _runtimeInventory.itemList.Count; i++)
            {
                if (_runtimeInventory.itemList[i].itemID == 0)
                {
                    return true;
                }
            }
            return false;
        }

        private int GetItemIndexInBag(int ID)
        {
            for (int i = 0; i < _runtimeInventory.itemList.Count; i++)
            {
                if (_runtimeInventory.itemList[i].itemID == ID)
                {
                    return i;
                }
            }
            return -1;
        }

        private void AddItemAtIndex(int ID, int index, int amount)
        {
            if (index == -1 && CheckBagCapacity())
            {
                InventoryItem item = new InventoryItem { itemID = ID, itemAmount = amount };
                for (int i = 0; i < _runtimeInventory.itemList.Count; i++)
                {
                    if (_runtimeInventory.itemList[i].itemID == 0)
                    {
                        _runtimeInventory.itemList[i] = item;
                        break;
                    }
                }
            }
            else
            {
                if (index == -1)
                {
                    //�޷�ʰȡ����Ʒ��������
                    return;
                }

                int currentAmount = _runtimeInventory.itemList[index].itemAmount + amount;

                InventoryItem item = new InventoryItem { itemID = ID, itemAmount = currentAmount };
                _runtimeInventory.itemList[index] = item;
            }
        }
    }
}