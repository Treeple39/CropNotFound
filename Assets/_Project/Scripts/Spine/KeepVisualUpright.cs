using UnityEngine;

[DisallowMultipleComponent]
public class KeepVisualUpright : MonoBehaviour
{
    void LateUpdate()
    {
        // 强制世界坐标系下的旋转为 identity
        transform.rotation = Quaternion.identity;
    }
}