using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SimpleArchiveUI : MonoBehaviour
{
    public Transform slotRoot;
    public Transform activeContainer;
    public Transform inactiveContainer;
    public GameObject slotPrefab;

    public Material silhouetteMaterial;

    private Dictionary<int, Transform> idToSlotMap;

    void Start()
    {
        BuildSlotMap();
        RefreshAllSlots();
    }

    void BuildSlotMap()
    {
        idToSlotMap = new Dictionary<int, Transform>();
        foreach (Transform t in slotRoot.GetComponentsInChildren<Transform>())
        {
            if (t.name.StartsWith("Slot_"))
            {
                if (int.TryParse(t.name.Substring(5), out int id))
                {
                    idToSlotMap[id] = t;
                }
            }
        }
    }

public void RefreshAllSlots()
{
    foreach (Transform t in activeContainer) Destroy(t.gameObject);
    foreach (Transform t in inactiveContainer) Destroy(t.gameObject);

    var items = ArchiveManager.Instance.GetAllItems();
    var itemDict = new Dictionary<int, ArchiveManager.ArchiveItem>();
    foreach (var item in items)
        itemDict[item.ID] = item;

    foreach (Transform slot in slotRoot)
    {
        if (!slot.name.StartsWith("Slot_")) continue;

        if (!int.TryParse(slot.name.Substring(5), out int id)) continue;

        if (!itemDict.TryGetValue(id, out var item)) continue;

        var parent = item.isActivated ? activeContainer : inactiveContainer;
        var newSlot = Instantiate(slotPrefab, parent);

        // ✅ 保证遮挡关系按 slot 顺序
        newSlot.transform.SetSiblingIndex(parent.childCount - 1);

        newSlot.transform.localPosition = slot.localPosition;
        newSlot.transform.localRotation = slot.localRotation;
        newSlot.transform.localScale = slot.localScale;

        var image = newSlot.GetComponent<Image>();

        Sprite sprite = Resources.Load<Sprite>("Characters/" + item.imagePath);
        if (sprite != null)
        {
            image.sprite = sprite;
            image.preserveAspect = true;
            image.SetNativeSize();
        }

        image.color = item.isActivated ? Color.white : Color.clear;

        if (!item.isActivated)
        {
            image.material = silhouetteMaterial;
        }
    }
}


}