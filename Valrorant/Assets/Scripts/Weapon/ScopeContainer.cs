using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScopeContainer : MonoBehaviour
{
    [SerializeField] GameObject _scope;

    public GameObject ReturnScope(){ return _scope; }
}
