using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class WanderingFSM : MonoBehaviour
//{
//    [SerializeField] float _radius = 2;
//    [SerializeField] float _angle = 30;
//    [SerializeField] float _distrance = 4f;

//    Vector3 dir;

//    private void Update()
//    {
//        float offset;

//        int random = Random.Range(0, 2);
//        if (random == 0) offset = 3 * Mathf.Deg2Rad;
//        else offset = -3 * Mathf.Deg2Rad;
//        _angle += offset;

//        Vector3 pos = ReturnPositionInCircle();
//        Debug.DrawLine(transform.position, pos);

//        Vector3 newDir = (pos - transform.position).normalized;
//        dir = Vector3.Lerp(dir, newDir, Time.deltaTime);

//        transform.forward = dir;
//        transform.position += dir * Time.deltaTime;
//    }

//    Vector3 ReturnPositionInCircle()
//    {
//        return new Vector3(Mathf.Cos(_angle) * _radius, 0f, Mathf.Sin(_angle) * _radius) + transform.position + new Vector3(_distrance, 0, 0); // 각도에 따른 위치 계산
//    }
//}
