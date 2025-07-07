using UnityEngine;
using UnityEngine.UI;


public class SimpleArchiveUI : MonoBehaviour
{
    [Header("基本设置")]
    public Transform gridParent;  // 放所有格子的父物体
    public Transform[] slotPositions; // 改为手动指定位置（在Inspector中拖入）
    public GameObject slotPrefab; // 格子预制件

    void Start()
    {
        // 确保有位置数据
        if (slotPositions == null || slotPositions.Length == 0)
        {
            slotPositions = gridParent.GetComponentsInChildren<Transform>();
            Debug.LogWarning("自动获取到" + (slotPositions.Length - 1) + "个位置点");
        }

        RefreshAllSlots();
    }

    public void RefreshAllSlots()
    {
        // 清空现有实例（防止重复创建）
        foreach (Transform pos in slotPositions)
        {
            if (pos.childCount > 0)
            {
                Destroy(pos.GetChild(0).gameObject);
            }
        }

        // 为每个图鉴项创建格子
        var items = ArchiveManager.Instance.GetAllItems();
        for (int i = 0; i < Mathf.Min(slotPositions.Length, items.Count); i++)
        {
            CreateSlot(items[i], slotPositions[i]);
        }
    }

    private void CreateSlot(ArchiveManager.ArchiveItem item, Transform parent)
    {
        GameObject newSlot = Instantiate(slotPrefab, parent);
        Image icon = newSlot.GetComponent<Image>();

        Sprite sprite = Resources.Load<Sprite>("Characters/" + item.imagePath);
        if (sprite != null)
        {
            icon.sprite = sprite;
            icon.preserveAspect = true;
            icon.color = item.isActivated ? Color.white : new Color(0.3f, 0.3f, 0.3f, 0.6f);
        }
        if (item.isActivated == false)
        {
            icon.material = new Material(Shader.Find("UI/BlackWithAlpha"));
        }
    }
}