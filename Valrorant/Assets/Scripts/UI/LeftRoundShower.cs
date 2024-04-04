using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LeftRoundShower : MonoBehaviour
{
    [SerializeField]
    GameObject _container;

    [SerializeField]
    TMP_Text _bulletCountInMagazine;

    [SerializeField]
    TMP_Text _bulletCountInPossession;

    public void OnRoundCountChange(bool isActive, int inMagazine, int inPossession)
    {
        _container.SetActive(isActive);
        _bulletCountInMagazine.text = inMagazine.ToString();
        _bulletCountInPossession.text = inPossession.ToString();
    }
}
