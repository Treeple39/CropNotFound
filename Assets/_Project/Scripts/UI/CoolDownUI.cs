using Inventory;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CooldownUI : MonoBehaviour
{
    [SerializeField] private Image _coolDownImage;
    private SlotUI _slot; // 关联的槽位

    private void Awake()
    {
        _slot = GetComponentInParent<SlotUI>();
        _coolDownImage.fillAmount = 0;
    }

    private void OnEnable()
    {
        EventHandler.OnCooldownStart += HandleCooldownStart;
        EventHandler.OnCooldownEnd += HandleCooldownEnd;
    }

    private void OnDisable()
    {
        EventHandler.OnCooldownStart -= HandleCooldownStart;
        EventHandler.OnCooldownEnd -= HandleCooldownEnd;
    }

    private void HandleCooldownStart(SlotUI slot, float duration)
    {
        if (slot != _slot) return; // 只处理当前槽位的冷却
        _slot.GetComponent<Button>().interactable = false;

        StartCoroutine(RunCooldown(duration));
    }

    private IEnumerator RunCooldown(float duration)
    {
        float timer = duration;
        _coolDownImage.fillAmount = 1;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            _coolDownImage.fillAmount = timer / duration;
            yield return null;
        }

        _coolDownImage.fillAmount = 0;
        EventHandler.CallCooldownEnd(_slot); // 可选：通知冷却结束
    }

    private void HandleCooldownEnd(SlotUI slot)
    {
        if (slot == _slot)
            _coolDownImage.fillAmount = 0;
        _slot.GetComponent<Button>().interactable = true;
    }
}