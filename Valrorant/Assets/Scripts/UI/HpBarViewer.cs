using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HpBarViewer : MonoBehaviour
{
    [SerializeField] Image _background;
    [SerializeField] Image _content;
    [SerializeField] Color _startColor;
    [SerializeField] Color _endColor;

    float _aliveContentAlphaValue = 255f/255f;
    float _aliveBackgroundAlphaValue = 50f/255f;

    float _dieContentAlphaValue = 50f/255f;
    float _dieBackgroundAlphaValue = 10f/255f;

    public void OnReviveRequested()
    {
        _content.color = new Color(_content.color.r, _content.color.g, _content.color.b, _aliveContentAlphaValue);
        _background.color = new Color(_background.color.r, _background.color.g, _background.color.b, _aliveBackgroundAlphaValue);
    }

    public void OnDieRequested()
    {
        _content.color = new Color(_content.color.r, _content.color.g, _content.color.b, _dieContentAlphaValue);
        _background.color = new Color(_background.color.r, _background.color.g, _background.color.b, _dieBackgroundAlphaValue);
    }

    public void OnChangeHpViewer(float ratio)
    {
        Color mixColor = Color.Lerp(_endColor, _startColor, ratio);
        _content.DOFillAmount(ratio, 0.5f);
        _content.DOBlendableColor(mixColor, 0.5f);
    }
}
