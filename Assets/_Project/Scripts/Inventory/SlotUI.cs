using Inventory;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Inventory
{
    public class SlotUI : MonoBehaviour, IPointerClickHandler
    {
        [Header("组件获取")]

        [SerializeField] private Image slotImage;
        [SerializeField] private Text amountText;
        public Image slotHighlight;
        [SerializeField] private Button button;

        [Header("Slot Category")]
        public SlotType slotType;
        public bool isSelected;
        public int slotIndex;

        [Header("Item Info")]
        public ItemDetails itemDetails;
        public int itemAmount;
        private InventoryUI inventoryUI => GetComponentInParent<InventoryUI>();

        private void Start()
        {
            isSelected = false;

            if (itemDetails.itemID == 0)
            {
                UpdateEmptySlot();
            }
        }

        public void UpdateSlot(ItemDetails item, int amount)
        {
            Sprite icon = ResourceManager.LoadSprite(item.itemIcon);

            itemDetails = item;
            slotImage.sprite = icon;
            itemAmount = amount;
            amountText.text = amount.ToString();
            slotImage.enabled = true;
            button.interactable = true;

            button.onClick.RemoveAllListeners();

            //TODO:冷却

            if (itemDetails.canUseSpell)
            {
                button.onClick.AddListener(delegate
                {
                    ItemType type = (ItemType)System.Enum.Parse(typeof(ItemType), itemDetails.itemType);
                    EventHandler.CallItemSpellUse(type, itemDetails);
                });
            }
        }

        public void UpdateEmptySlot()
        {
            if (isSelected)
            {
                isSelected = false;
            }

            amountText.text = string.Empty;
            slotImage.enabled = false;
            button.interactable = false;
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            if (itemAmount == 0) return;
            isSelected = true;

            inventoryUI.UpdateSlotHighlight(slotIndex);

            //鼠标黏着
        }
    }
}