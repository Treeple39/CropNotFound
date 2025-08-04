using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class ChangeInputMode : MonoBehaviour
{

    public void ChangeInputModeOnce(Slider slider)
    {
        if (slider.value < 0.5f)
        {
            EventHandler.CallInputModeChanged(InputMode.VirtualJoystick);
            slider.value = 1;
        }
        else
        {
            EventHandler.CallInputModeChanged(InputMode.FullScreenTouch);
            slider.value = 0;
        }
    }
}
