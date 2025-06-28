using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParallaxCtrl2 : MonoBehaviour
{
    [Tooltip("材质随镜头偏移量（小数百分数）,0相对世界坐标静止，1相对镜头坐标静止")]   //移动贴图坐标，赋予循环背景图视差效果，常用于循环的背景山脉、峡谷、高楼等等
    public Vector2 followRate;
    [Tooltip("材质初始位置修改（运行才生效）")]                                        //修改贴图的初始坐标，特定场景中循环图构图不好可以用这个调整
    public Vector2 startRate;
    [Tooltip("材质随时间平移速度")]                                                    //贴图内容随时间滚动循环，适用于漂浮的云雾、车流人流、滚动电子屏、乘坐火车等载具时的背景
    public Vector2 moveSpeed;
    [Tooltip("对象随镜头移动量（小数百分数）")]                                        //移动对象物理坐标，赋予非循环素材视差效果，一些横向循环素材上下没有留空，但又想做Y轴视差可以使用
    public Vector2 transformRate = new Vector2(1, 1);

    [HideInInspector] public Image image;
    public Transform Cam;

    private float startPointX;
    private float startPointY;

    private void Awake()
    {
        image = GetComponent<Image>();                                 //获取贴图组件
        Cam = GameObject.Find("Main Camera").GetComponent<Transform>();      //寻找名为Main Camera的对象，获取主摄像机变换坐标
        startPointX = transform.position.x;                                  //记录摄像机初始X坐标位置
        startPointY = transform.position.y;                                  //记录摄像机初始Y坐标位置
    }

    protected virtual void LateUpdate()
    {
        float moveX = Mathf.Repeat(Time.time * moveSpeed.x, 1);
        float moveY = Mathf.Repeat(Time.time * moveSpeed.y, 1);

        float rateX = Cam.position.x * transformRate.x * 0.02f;
        float rateY = Cam.position.y * transformRate.y * 0.02f;

        image.material.mainTextureOffset = new Vector2(moveX + (rateX * followRate.x) + startRate.x, moveY + (rateY * followRate.y) + startRate.y);
    }
}
