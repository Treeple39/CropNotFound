using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : MonoBehaviour
{
    public void DashDash()
    {
        InputManager.Instance.DashPressed = true;
    }
}
