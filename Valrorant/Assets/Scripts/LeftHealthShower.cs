using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LeftHealthShower : MonoBehaviour
{
    [SerializeField] TMP_Text _hpTxt;

    public void OnHealthUpdateRequested(float hp)
    {
        _hpTxt.text = hp.ToString();
    }
}
