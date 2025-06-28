using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AutoFill : MonoBehaviour
{
    [Range(0.1f, 5f)]
    [SerializeField]public float fillSpeed = 0.2f; // ��������ٶ�
    
    private Image _image;
    private float _currentFill;

    void Start()
    {
        _image = GetComponent<Image>();
        _image.fillAmount = 0;           // ��ʼ״̬Ϊ��
        
        //����������������ʾ��
        StartCoroutine(FillAnimation());
    }

    System.Collections.IEnumerator FillAnimation()
    {
        while (_image.fillAmount < 1)
        {
            _image.fillAmount += Time.deltaTime * fillSpeed;
            yield return null; // ÿ֡����
        }

        _image.fillAmount = 1; // ȷ����������
    }
}
