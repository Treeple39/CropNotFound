using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieAnim : MonoBehaviour
{
    public GameObject FlashX;
    public void StarExplode(Vector3 position)
    {
        GameObject fx = Instantiate(FlashX, position, Quaternion.identity);
    }
}
