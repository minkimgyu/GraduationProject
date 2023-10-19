using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ObserverPattern;

public class LeftRoundShower : MonoBehaviour//, IObserver<int, int>
{
    [SerializeField]
    GameObject _container;

    [SerializeField]
    TMP_Text _bulletCountInMagazine;

    [SerializeField]
    TMP_Text _bulletCountInPossession;

    public void OnBulletCountChange(int inMagazine, int inPossession)
    {
        _bulletCountInMagazine.text = inMagazine.ToString();
        _bulletCountInPossession.text = inPossession.ToString();
    }

    public void OnActiveContainer(bool isActive)
    {
        _container.SetActive(isActive);
    }
}
