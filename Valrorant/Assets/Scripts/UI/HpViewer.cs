using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HpViewer : MonoBehaviour
{
    [SerializeField]
    TMP_Text _hp;

    [SerializeField]
    TMP_Text _armor;

    public void OnHpChange(int hp, int armor)
    {
        _hp.text = hp.ToString();
        _armor.text = armor.ToString();
    }
}
