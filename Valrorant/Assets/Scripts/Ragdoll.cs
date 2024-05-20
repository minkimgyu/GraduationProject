using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    [SerializeField] Transform _rig;

    public Transform Rig { get { return _rig; } }

    private void OnEnable()
    {
        Invoke("DisableGo", 5f);
    }

    void DisableGo()
    {
        Destroy(gameObject);

        //gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        //CancelInvoke();
        //ObjectPooler.ReturnToPool(gameObject);
    }
}
