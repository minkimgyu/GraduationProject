using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    [SerializeField] GameObject _crosshair;

    public void SwitchCrosshair(bool turnOn)
    {
        _crosshair.SetActive(turnOn);
    }
}
