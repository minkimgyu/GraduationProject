using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using System;
using Object = UnityEngine.Object;

public class DieState : State
{
    float _destoryDelay;
    Action<float> DestoryMe;
    Transform _myTransform;
    Transform _myRig;

    GameObject _model;
    CapsuleCollider _modelCol;
    Ragdoll _ragdoll;
    string _ragdoolName;

    private void CopyAnimCharacterTransformToRagdoll(Transform origin, Transform rag)
    {
        rag.position = origin.position;
        rag.rotation = origin.rotation;

        for (int i = 0; i < origin.transform.childCount; i++)
        {
            if (origin.childCount != rag.childCount) continue;
            if (origin.transform.childCount != 0)
            {
                CopyAnimCharacterTransformToRagdoll(origin.transform.GetChild(i), rag.transform.GetChild(i));
            }

            rag.transform.GetChild(i).localPosition = origin.transform.GetChild(i).localPosition;
            rag.transform.GetChild(i).localRotation = origin.transform.GetChild(i).localRotation;
        }
    }

    void OnDieRequested()
    {
        _modelCol = _myTransform.GetComponent<CapsuleCollider>();
        _modelCol.enabled = false;

        if(_ragdoll != null)
        {
            Ragdoll ragdoll = Object.Instantiate(_ragdoll);
            CopyAnimCharacterTransformToRagdoll(_myRig, ragdoll.Rig);
        }

        // 그냥 여기서 생성시켜버리자
        //Ragdoll ragObj = ObjectPooler.SpawnFromPool<Ragdoll>(_ragdoolName);
        //CopyAnimCharacterTransformToRagdoll(_myRig, ragObj.Rig);
        _model.SetActive(false);
        DestoryMe(_destoryDelay);
    }

    public DieState(Ragdoll ragdoll, string ragdoolName, Transform myTransform, GameObject model, Transform myRig, float destoryDelay, Action<float> DestoryMe)
    {
        _ragdoll = ragdoll;
        _ragdoolName = ragdoolName;
        _myTransform = myTransform;
        _model = model;
        _myRig = myRig;
        _destoryDelay = destoryDelay;
        this.DestoryMe = DestoryMe;
    }

    public override void OnStateEnter()
    {
        OnDieRequested();
    }
}