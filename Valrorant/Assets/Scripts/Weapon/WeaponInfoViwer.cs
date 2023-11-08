using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponInfoViwer : MonoBehaviour
{
    [SerializeField] GameObject _viewr;

    [SerializeField] TMP_Text _objectName;

    [SerializeField] float _offsetYPos = 0.5f;

    Transform _cameraTransform;

    private void Start()
    {
        _cameraTransform = Camera.main.transform;
    }

    public void OnViewEventReceived(bool nowActivate, string name, Vector3 position)
    {
        _viewr.SetActive(nowActivate);
        _objectName.text = name;
        transform.position = position + Vector3.up * _offsetYPos;
    }

    private void Update()
    {
        transform.LookAt(_viewr.transform.position + _cameraTransform.forward);
    }
}
