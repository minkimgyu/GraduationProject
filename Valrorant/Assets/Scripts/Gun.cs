using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public enum PenetrateTarget
//{
//    Wood,
//    Rock,
//    Iron,
//    Agent // 플레이어도 관통샷이 가능하므로 여기에 넣는다.
//}

public struct PenetrateData
{
    Vector3 _entryPoint;
    public Vector3 EntryPoint { get { return _entryPoint; } }

    Vector3 _exitPoint;
    public Vector3 ExitPoint { get { return _exitPoint; } }

    IPenetrateTarget _target;

    public float ReturnFinalDurability()
    {
        return Vector3.Distance(_entryPoint, _exitPoint) * _target.ReturnDurability(); // 거리와 내구성 곱연산
    }

    public PenetrateData(Vector3 entryPoint, Vector3 exitPoint, IPenetrateTarget target)
    {
        _entryPoint = entryPoint;
        _exitPoint = exitPoint;
        _target = target;
    }
}

public class Gun : BaseWeapon
{
    Transform _weaponHolder;
    Transform _camTransform;
    public float range = 100f;

    ParticleSystem _muzzleFlash;

    protected float penetratePower = 15;
    float tmpPenetratePower;

    [SerializeField]
    Transform muzzle;

    [SerializeField]
    TrajectoryLineEffect lineEffectPrefab;

    [SerializeField]
    BulletHoleEffect bulletHoleEffectPrefab;

    private void Start()
    {
        _muzzleFlash = GetComponentInChildren<ParticleSystem>();
    }


    public override void Initialize(Transform holder, Transform cam)
    {
        _weaponHolder = holder;
        _camTransform = cam;
    }

    public override void Action()
    {
        _muzzleFlash.Play();

        RaycastHit[] hitObjects;
        hitObjects = Physics.RaycastAll(_camTransform.position, _camTransform.forward, range);
        Debug.DrawRay(_camTransform.position, _camTransform.forward * range, Color.green, 10);

        System.Array.Sort(hitObjects, (x, y) => x.distance.CompareTo(y.distance)); // 맞은 객체들을 거리 기반으로 정렬해준다.

        for (int i = 0; i < hitObjects.Length; i++)
        {
            if (hitObjects[i].transform == _weaponHolder) continue;

            DrawTrajectoryLine(hitObjects[i].point);
            SpawnBulletHole(hitObjects[i].point, hitObjects[i].normal);

            break;
        }
    }

    void ShootRaycast()
    {
        Debug.DrawRay(_camTransform.position, _camTransform.forward * range, Color.green, 10); // 디버그 레이를 카메라가 바라보고 있는 방향으로 발사한다.

        RaycastHit[] entryHits;
        entryHits = Physics.RaycastAll(_camTransform.position, _camTransform.forward, range);

        if (entryHits.Length == 0) return; // 만약 아무도 맞지 않았다면 리턴

        System.Array.Sort(entryHits, (x, y) => x.distance.CompareTo(y.distance)); // 맞은 객체들을 거리 기반으로 정렬해준다.

        Vector3 endPoint = _camTransform.position + _camTransform.forward * range; // 레이캐스트가 닫는 가장 마지막 위치

        RaycastHit[] exitHits;
        exitHits = Physics.RaycastAll(endPoint, -_camTransform.position, range); // 다시 반대로 쏴줌

        System.Array.Sort(exitHits, (x, y) => y.distance.CompareTo(x.distance)); // 맞은 객체들을 거리 기반으로 정렬해준다.
        //System.Array.Reverse(exitHits); // 해당 리스트를 뒤집어준다.


        List<PenetrateData> penetrateDatas = new List<PenetrateData>();

        if (entryHits.Length != exitHits.Length) return;

        for (int i = 0; i < entryHits.Length; i++)
        {
            if (entryHits[i].collider != exitHits[i].collider) continue;

            IPenetrateTarget target = entryHits[i].collider.GetComponent<IPenetrateTarget>();
            if (target == null) continue;

            penetrateDatas.Add(new PenetrateData(entryHits[i].point, exitHits[i].point, target));
        }

        tmpPenetratePower = penetratePower;

        for (int i = 0; i < penetrateDatas.Count; i++)
        {
            float finalDurability = penetrateDatas[i].ReturnFinalDurability();

            if (tmpPenetratePower - finalDurability >= 0)
            {
                // 이 경우, 진입, 퇴장 포인트 모두에 관통 이미지 추가
            }
            else
            {
                // 이 경우, 막혔기 때문에 이 포인트까지 총알 궤적 그리기
                // 이 포인트에 총알이 막힌 이미지 추가
            }

            // 추가로, 관통이 되더라도 다음 포인트 진입 시점까지 거리가 멀다면 적절히 끊어주기
            // 이 연산은 거리 * (관통한 벽 수 * N) 값으로 진행한다.

            // 남은 관통 수치에 비례해서 총알 궤적을 마저 그려주자
        }
    }

    void DrawTrajectoryLine(Vector3 hitPosition)
    {
        TrajectoryLineEffect trajectoryLineEffect = Instantiate(lineEffectPrefab);
        trajectoryLineEffect.Initialize(hitPosition, muzzle.position);
        trajectoryLineEffect.PlayEffect();
    }

    void SpawnBulletHole(Vector3 hitPosition, Vector3 hitNormal)
    {
        BulletHoleEffect bulletHoleEffect = Instantiate(bulletHoleEffectPrefab);

        bulletHoleEffect.Initialize(hitPosition, hitNormal, Quaternion.LookRotation(-hitNormal));
        bulletHoleEffect.PlayEffect();
    }
}
