using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HpViewer : MonoBehaviour
{
    [SerializeField]
    HpBarViewer _barViewer;

    public void OnHpChange(float ratio)
    {
        _barViewer.OnChangeHpViewer(ratio);
    }
}
