using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PenetrateData
{
    const float _airDurability = 3;
    public float AirDurability { get { return _airDurability; } }

    Vector3 _entryPoint;
    public Vector3 EntryPoint { get { return _entryPoint; } }

    Vector3 _entryNormal;
    public Vector3 EntryNormal { get { return _entryNormal; } }


    Vector3 _exitPoint;
    public Vector3 ExitPoint { get { return _exitPoint; } }

    Vector3 _exitNormal;
    public Vector3 ExitNormal { get { return _exitNormal; } }


    IPenetrateTarget _target;

    public float ReturnFinalDurability()
    {
        return ReturnDistance() * _target.ReturnDurability(); // 거리와 내구성 곱연산
    }

    public float ReturnDistance()
    {
        return Vector3.Distance(_entryPoint, _exitPoint);
    }

    public PenetrateData(Vector3 entryPoint, Vector3 exitPoint, Vector3 entryNormal, Vector3 exitNormal, IPenetrateTarget target)
    {
        _entryPoint = entryPoint;
        _exitPoint = exitPoint;

        _entryNormal = entryNormal;
        _exitNormal = exitNormal;

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
    protected float trajectoryLineOffset = 1.3f;
    

    [SerializeField]
    Transform muzzle;

    protected int bulletShootInOneAction = 1;

    protected int loadedBullet = 30;
    protected int possessingBullet = 60;

    private void Start()
    {
        _muzzleFlash = GetComponentInChildren<ParticleSystem>();
    }

    public override void Initialize(Transform holder, Transform cam)
    {
        _weaponHolder = holder;
        _camTransform = cam;


        // 여기에서 UI에 이밴트로 연결시키는 방식
    }

    public override void Action()
    {
        if (loadedBullet <= 0) return;
        loadedBullet -= bulletShootInOneAction;

        _muzzleFlash.Play();
        ShootRaycast();
    }

    void ShootRaycast()
    {
        Debug.DrawRay(_camTransform.position, _camTransform.forward * range, Color.green, 10); // 디버그 레이를 카메라가 바라보고 있는 방향으로 발사한다.

        RaycastHit[] entryHits;
        entryHits = Physics.RaycastAll(_camTransform.position, _camTransform.forward, range, targetLayer);

        if (entryHits.Length == 0) return; // 만약 아무도 맞지 않았다면 리턴

        System.Array.Sort(entryHits, (x, y) => x.distance.CompareTo(y.distance)); // 맞은 객체들을 거리 기반으로 정렬해준다.

        Vector3 endPoint = _camTransform.position + _camTransform.forward * range; // 레이캐스트가 닫는 가장 마지막 위치

        RaycastHit[] tmpHits;
        tmpHits = Physics.RaycastAll(endPoint, -_camTransform.forward, range, targetLayer); // 다시 반대로 쏴줌

        System.Array.Sort(tmpHits, (x, y) => y.distance.CompareTo(x.distance)); // 맞은 객체들을 거리 기반으로 정렬해준다.

        List<RaycastHit> exitHits = new List<RaycastHit>();

        for (int i = 0; i < tmpHits.Length; i++)
        {
            if(tmpHits[i].collider.transform == _weaponHolder) continue; // _weaponHolder 배열에서 제외하기
            exitHits.Add(tmpHits[i]);
        }

        List<PenetrateData> penetrateDatas = new List<PenetrateData>();

        if (entryHits.Length != exitHits.Count) return;

        for (int i = 0; i < entryHits.Length; i++)
        {
            if (entryHits[i].collider != exitHits[i].collider) continue;

            IPenetrateTarget target = entryHits[i].collider.GetComponent<IPenetrateTarget>();
            if (target == null) continue;

            penetrateDatas.Add(new PenetrateData(entryHits[i].point, exitHits[i].point, entryHits[i].normal, exitHits[i].normal, target));
        }

        float tmpPenetratePower = penetratePower;

        for (int i = 0; i < penetrateDatas.Count; i++)
        {
            float finalDurability = penetrateDatas[i].ReturnFinalDurability();

            Debug.Log("Distance: " + penetrateDatas[i].ReturnDistance());
            Debug.Log("finalDurability: " + finalDurability);

            if (tmpPenetratePower - finalDurability >= 0)
            {
                SpawnBulletHole(penetrateDatas[i].EntryPoint, penetrateDatas[i].EntryNormal, false);
                SpawnBulletHole(penetrateDatas[i].ExitPoint, penetrateDatas[i].ExitNormal, false);
                // 이 경우, 진입, 퇴장 포인트 모두에 관통 이미지 추가

                Debug.Log("beforeTmpPenetratePower: " + tmpPenetratePower);

                tmpPenetratePower -= finalDurability;

                Debug.Log("afterTmpPenetratePower: " + tmpPenetratePower);

                if(i == penetrateDatas.Count - 1) // 만약 관통이 마지막 충돌일 경우
                {
                    Vector3 offsetDir = (penetrateDatas[i].ExitPoint - muzzle.position).normalized * trajectoryLineOffset;
                    DrawTrajectoryLine(penetrateDatas[i].ExitPoint + offsetDir);
                }
            }
            else
            {
                SpawnBulletHole(penetrateDatas[i].EntryPoint, penetrateDatas[i].EntryNormal, true);
                DrawTrajectoryLine(penetrateDatas[i].EntryPoint);
                break; // 여기서 끝내줌
                // 이 경우, 막혔기 때문에 이 포인트까지 총알 궤적 그리기
                // 이 포인트에 총알이 막힌 이미지 추가
            }


            if (penetrateDatas.Count <= i + 1) continue; // 뒤에 관통 정보가 없다면 진행하지 않음
            Debug.Log("DistanceBetweenTwoPoints");

            Debug.Log("beforeTmpPenetratePower: " + tmpPenetratePower);

            float distanceBetweenExitAndEntryPoint = Vector3.Distance(penetrateDatas[i].ExitNormal, penetrateDatas[i + 1].EntryNormal);
            float finalDistanceBetweenExitAndEntryPoint = distanceBetweenExitAndEntryPoint * penetrateDatas[i].AirDurability;

            Debug.Log("distanceBetweenExitAndEntryPoint : " + distanceBetweenExitAndEntryPoint);
            Debug.Log("finalDistanceBetweenExitAndEntryPoint : " + finalDistanceBetweenExitAndEntryPoint);

            tmpPenetratePower -= finalDistanceBetweenExitAndEntryPoint;
            Debug.Log("afterTmpPenetratePower: " + tmpPenetratePower);

            // 추가로, 관통이 되더라도 다음 포인트 진입 시점까지 거리가 멀다면 적절히 끊어주기
            // 이 연산은 거리 * (관통한 벽 수 * N) 값으로 진행한다.

            // 남은 관통 수치에 비례해서 총알 궤적을 마저 그려주자
        }
    }

    void DrawTrajectoryLine(Vector3 hitPosition)
    {
        TrajectoryLineEffect trajectoryLineEffect = ObjectPooler.SpawnFromPool<TrajectoryLineEffect>("TrajectoryLineEffect");
        trajectoryLineEffect.Initialize(hitPosition, muzzle.position);
        trajectoryLineEffect.PlayEffect();
    }

    void SpawnBulletHole(Vector3 hitPosition, Vector3 hitNormal, bool isBlocked)
    {
        BulletHoleEffect bulletHoleEffect;
        
        if (isBlocked) bulletHoleEffect = ObjectPooler.SpawnFromPool<BulletHoleEffect>("NonPenetrateBulletHoleEffect");
        else bulletHoleEffect = ObjectPooler.SpawnFromPool<BulletHoleEffect>("BulletHoleEffect");

        bulletHoleEffect.Initialize(hitPosition, hitNormal, Quaternion.LookRotation(-hitNormal));
        bulletHoleEffect.PlayEffect();
    }
}
