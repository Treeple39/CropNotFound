using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DashButtonController : MonoBehaviour
{
    Button dashButton;
    void Start()
    {
        dashButton = GetComponent<Button>();
        if (dashButton != null)
        {
            InputManager.Instance.TryBindDashButton(dashButton);
        }
        else
        {
            Debug.LogWarning("[DashButtonController] Button component not found on this GameObject.");
        }
    }
    public void LogClick()
    {
        Debug.Log("Clicked!");
    }
}