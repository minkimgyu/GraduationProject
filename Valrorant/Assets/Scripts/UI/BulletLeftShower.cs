using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ObserverPattern;

public class BulletLeftShower : MonoBehaviour, IObserver<int, int>
{
    [SerializeField]
    GameObject container;

    [SerializeField]
    TMP_Text bulletCountInMagazine;

    [SerializeField]
    TMP_Text bulletCountInPossession;

    public void Notify(int inMagazine, int inPossession)
    {
        if(inMagazine == 0 && inPossession == 0)
        {
            container.SetActive(false);
        }
        else
        {
            container.SetActive(true);
            bulletCountInMagazine.text = inMagazine.ToString();
            bulletCountInPossession.text = inPossession.ToString();
        }
    }
}
