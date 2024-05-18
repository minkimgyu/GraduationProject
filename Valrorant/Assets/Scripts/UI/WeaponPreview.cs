using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponPreview : MonoBehaviour
{
    [SerializeField] Image _image;
    [SerializeField] TMP_Text _num;

    public void TurnOnPreview(Sprite sprite, int num)
    {
        if (gameObject.activeSelf == false) gameObject.SetActive(true);

        _image.sprite = sprite;
        _num.text = num.ToString();
    }

    public void TurnOffPreview()
    {
        gameObject.SetActive(false);
    }
}
