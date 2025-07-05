using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_PreventAccidentalTouch : MonoBehaviour
{
    [Tooltip("需要防误触的UI对象列表")]
    public List<GameObject> uiObjects = new List<GameObject>();
    
    private bool touchStartedOnUI = false;
    private bool currentTouchOnUI = false;
    private bool touchActive = false;
    
    // 用于在外部检查是否应该阻止角色移动的公共属性
    public bool ShouldBlockCharacterMovement { get; private set; }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }
    
    private void HandleInput()
    {
        // 处理鼠标输入
        if (Input.GetMouseButtonDown(0))
        {
            touchActive = true;
            currentTouchOnUI = IsPointerOverUI();
            touchStartedOnUI = currentTouchOnUI;
            UpdateBlockState();
        }
        else if (Input.GetMouseButton(0) && touchActive)
        {
            // 检查触摸是否在UI上
            currentTouchOnUI = IsPointerOverUI();
            UpdateBlockState();
        }
        else if (Input.GetMouseButtonUp(0) && touchActive)
        {
            // 触摸结束
            currentTouchOnUI = IsPointerOverUI();
            UpdateBlockState();
            
            // 重置状态
            touchActive = false;
            touchStartedOnUI = false;
            currentTouchOnUI = false;
            ShouldBlockCharacterMovement = false;
        }
        
        // 处理触摸输入
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            if (touch.phase == TouchPhase.Began)
            {
                touchActive = true;
                currentTouchOnUI = IsPointerOverUI();
                touchStartedOnUI = currentTouchOnUI;
                UpdateBlockState();
            }
            else if (touch.phase == TouchPhase.Moved && touchActive)
            {
                // 检查触摸是否在UI上
                currentTouchOnUI = IsPointerOverUI();
                UpdateBlockState();
            }
            else if ((touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) && touchActive)
            {
                // 触摸结束
                currentTouchOnUI = IsPointerOverUI();
                UpdateBlockState();
                
                // 重置状态
                touchActive = false;
                touchStartedOnUI = false;
                currentTouchOnUI = false;
                ShouldBlockCharacterMovement = false;
            }
        }
    }
    
    private void UpdateBlockState()
    {
        // 场景1：开始在UI上，当前也在UI上，应该阻止角色移动
        // 场景2：开始在UI上，但滑出了UI，不应该阻止角色移动
        // 场景3：开始不在UI上，但滑入了UI，不应该阻止角色移动
        ShouldBlockCharacterMovement = touchStartedOnUI && currentTouchOnUI;
    }
    
    private bool IsPointerOverUI()
    {
        // 检查指针是否在UI上
        if (EventSystem.current == null)
            return false;
            
        // 首先使用标准方法检查是否在任何UI上
        bool onAnyUI = false;
        if (Input.touchCount > 0)
        {
            onAnyUI = EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        }
        else
        {
            onAnyUI = EventSystem.current.IsPointerOverGameObject();
        }
        
        // 如果不在任何UI上，直接返回false
        if (!onAnyUI)
            return false;
            
        // 如果UI对象列表为空，则任何UI都会触发防误触
        if (uiObjects.Count == 0)
            return true;
            
        // 检查是否在指定的UI对象上
        Vector2 pointerPosition = Input.touchCount > 0 ? 
            (Vector2)Input.GetTouch(0).position : 
            (Vector2)Input.mousePosition;
            
        foreach (GameObject uiObject in uiObjects)
        {
            if (uiObject != null && IsPointerOverUIObject(uiObject, pointerPosition))
                return true;
        }
        
        // 不在指定的UI对象上
        return false;
    }
    
    private bool IsPointerOverUIObject(GameObject uiObject, Vector2 pointerPosition)
    {
        // 获取UI对象的RectTransform
        RectTransform rectTransform = uiObject.GetComponent<RectTransform>();
        if (rectTransform == null)
            return false;
            
        // 检查对象是否激活
        if (!uiObject.activeInHierarchy)
            return false;
            
        // 检查点击位置是否在UI矩形区域内
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform, 
            pointerPosition, 
            null, // 对于屏幕空间覆盖的Canvas，使用null
            out Vector2 localPoint);
            
        // 检查本地坐标是否在矩形内
        Vector2 sizeDelta = rectTransform.rect.size;
        Vector2 pivot = rectTransform.pivot;
        Vector2 normalizedPosition = localPoint + new Vector2(
            sizeDelta.x * pivot.x,
            sizeDelta.y * pivot.y
        );
        
        // 检查点是否在矩形内
        bool isInRect = (normalizedPosition.x >= 0 && normalizedPosition.x <= sizeDelta.x &&
                        normalizedPosition.y >= 0 && normalizedPosition.y <= sizeDelta.y);
        
        // 如果不在当前对象上，还需要检查其子对象
        if (!isInRect)
        {
            for (int i = 0; i < uiObject.transform.childCount; i++)
            {
                GameObject child = uiObject.transform.GetChild(i).gameObject;
                if (IsPointerOverUIObject(child, pointerPosition))
                    return true;
            }
        }
        
        return isInRect;
    }
}
