using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    public class Item : MonoBehaviour
    {
        public int itemID;
        public SpriteRenderer spriteRenderer;
        public BoxCollider2D coll;
        [HideInInspector] public ItemDetails itemDetails;
        private ItemType itemType;

        private void Start()
        {
            if (itemID != 0)
                Init(itemID);
        }

        public void Init(int ID)
        {
            itemID = ID;
            itemDetails = InventoryManager.Instance.GetItemDetail(itemID);

            Sprite sprite = ResourceManager.LoadSprite(itemDetails.itemSpriteOnWorld);
            Sprite icon = ResourceManager.LoadSprite(itemDetails.IconPath);

            //��ײ������Ӧ
            if (itemDetails != null)
            {
                
                spriteRenderer.sprite = sprite != null 
                    ? sprite
                    : icon;

                Vector2 newSize = new Vector2(
                    spriteRenderer.sprite.bounds.size.x,
                    spriteRenderer.sprite.bounds.size.y
                    );
                coll.size = newSize;
                coll.offset = new Vector2(0, spriteRenderer.sprite.bounds.center.y);

                coll.isTrigger = itemDetails.canPickedup;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!itemDetails.canPickedup) return;

            if (collision.CompareTag("Player"))
            {
                //ʰȡ

                //������Ʒ�򲥷Ŷ���
            }
        }
    }
}