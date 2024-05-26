using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRecoilReceiver
{
    Vector2 FireViewRotation { get; } // 총 발사 시작 지점 회전 값 프로퍼티

    Vector3 ReturnRaycastPos();
    Vector3 ReturnRaycastDir();

    void OnRecoilRequested(Vector2 recoilForce);

    void ApplyRecoilToCamera();
}

namespace AI.Component
{
    public class ViewComponent : MonoBehaviour
    {
        float _speed;
        //[SerializeField] Transform _sightPoint;

        public void Initialize(float speed)
        {
            _speed = speed;
        }

        public void View(Vector3 dir)
        {
            //dir = new Vector3(0, dir.y, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * _speed);
        }
    }
}