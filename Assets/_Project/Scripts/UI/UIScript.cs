using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AutoFill : MonoBehaviour
{
    [Range(0.1f, 5f)]
    [SerializeField]public float fillSpeed = 0.2f; // 控制填充速度
    
    private Image _image;
    private float _currentFill;

    void Start()
    {
        _image = GetComponent<Image>();
        _image.fillAmount = 0;           // 初始状态为空
        
        //这里是用来调用显示的
        StartCoroutine(FillAnimation());
    }

    System.Collections.IEnumerator FillAnimation()
    {
        while (_image.fillAmount < 1)
        {
            _image.fillAmount += Time.deltaTime * fillSpeed;
            yield return null; // 每帧递增
        }

        _image.fillAmount = 1; // 确保最终填满
    }
}
