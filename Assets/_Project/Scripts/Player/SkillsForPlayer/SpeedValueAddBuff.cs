using UnityEngine;

public class SpeedValueAddBuff : BaseBuff
{
    // ���Buff���ӵ�������ֵ��������Inspector������
    [SerializeField] private float speedIncreaseAmount = 2.0f;

    private void Awake()
    {
        buffName = "��������";
    }

    public override void ApplyBuff(GameObject target)
    {
        // �������������һ����Ϊ PlayerMovement �Ľű������ƶ�
        PlayerMovement movementController = target.GetComponent<PlayerMovement>();

        if (movementController != null)
        {
            // ֱ���޸ĵײ�����
            movementController.maxSpeed += speedIncreaseAmount;
            Debug.Log($"Ӧ��Buff: '{buffName}'�������������� {speedIncreaseAmount}��");
        }
        else
        {
            Debug.LogWarning($"��Ŀ��'{target.name}'��û���ҵ� PlayerMovement �ű����޷�Ӧ���ٶ�Buff��");
        }
    }
}