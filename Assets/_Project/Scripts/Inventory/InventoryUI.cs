using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Inventory 
{
    public class InventoryUI : MonoBehaviour
    {
        [Header("µÀ¾ß±³°ü")]
        [SerializeField] private GameObject bagUIPanel;
        private Animator bagUIAnim;
        public bool bagOpened;

        [SerializeField] private SlotUI[] bagSlots;

        private void OnEnable()
        {
            EventHandler.UpdateInventoryUI += OnUpdateInventoryUI;
        }

        private void OnDisable()
        {
            EventHandler.UpdateInventoryUI -= OnUpdateInventoryUI;
        }

        private void Awake()
        {
            bagUIAnim = bagUIPanel.GetComponent<Animator>();
        }

        private void Start()
        {
            for (int i = 0; i < bagSlots.Length; i++)
            {
                bagSlots[i].slotIndex = i;
            }
            bagOpened = bagUIPanel.activeInHierarchy;
        }

        private void OnUpdateInventoryUI(InventoryLocation location, List<InventoryItem> list)
        {
            switch (location)
            {
                case InventoryLocation.Player:
                    for (int i = 0; i < bagSlots.Length; i++)
                    {
                        if (list[i].itemAmount > 0)
                        {
                            Debug.Log(list[i].itemID);
                            var item = InventoryManager.Instance.GetItemDetail(list[i].itemID);
                            bagSlots[i].UpdateSlot(item, list[i].itemAmount);
                        }
                        else
                        {
                            bagSlots[i].UpdateEmptySlot();
                        }
                    }
                    break;

                default:

                    break;
            }
        }

        public void OpenBagUI()
        {
            bagOpened = !bagOpened;
            bagUIAnim.SetBool("opened", bagOpened);
        }

        public void UpdateSlotHighlight(int index)
        {
            foreach (var slot in bagSlots)
            {
                if (slot.isSelected && slot.slotIndex == index)
                {
                    slot.slotHighlight.gameObject.SetActive(true);
                }
                else
                {
                    slot.isSelected = false;
                    slot.slotHighlight.gameObject.SetActive(false);
                }
            }
        }
    }
}
