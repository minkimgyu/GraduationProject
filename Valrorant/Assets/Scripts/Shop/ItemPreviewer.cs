using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemPreviewer : MonoBehaviour
{
    [SerializeField] Image _model;
    [SerializeField] TMP_Text _title;
    [SerializeField] TMP_Text _info;

    public void TurnOnPreview(Sprite sprite, string title, string info)
    {
        if(gameObject.activeSelf == false) gameObject.SetActive(true);

        _model.sprite = sprite;
        _title.text = title;
        _info.text = info;
    }

    public void TurnOffPreview()
    {
        gameObject.SetActive(false);
    }
}
