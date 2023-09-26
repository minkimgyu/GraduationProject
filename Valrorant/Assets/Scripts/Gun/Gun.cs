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
        return ReturnDistance() * _target.ReturnDurability(); // �Ÿ��� ������ ������
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


        // ���⿡�� UI�� �̹�Ʈ�� �����Ű�� ���
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
        Debug.DrawRay(_camTransform.position, _camTransform.forward * range, Color.green, 10); // ����� ���̸� ī�޶� �ٶ󺸰� �ִ� �������� �߻��Ѵ�.

        RaycastHit[] entryHits;
        entryHits = Physics.RaycastAll(_camTransform.position, _camTransform.forward, range, targetLayer);

        if (entryHits.Length == 0) return; // ���� �ƹ��� ���� �ʾҴٸ� ����

        System.Array.Sort(entryHits, (x, y) => x.distance.CompareTo(y.distance)); // ���� ��ü���� �Ÿ� ������� �������ش�.

        Vector3 endPoint = _camTransform.position + _camTransform.forward * range; // ����ĳ��Ʈ�� �ݴ� ���� ������ ��ġ

        RaycastHit[] tmpHits;
        tmpHits = Physics.RaycastAll(endPoint, -_camTransform.forward, range, targetLayer); // �ٽ� �ݴ�� ����

        System.Array.Sort(tmpHits, (x, y) => y.distance.CompareTo(x.distance)); // ���� ��ü���� �Ÿ� ������� �������ش�.

        List<RaycastHit> exitHits = new List<RaycastHit>();

        for (int i = 0; i < tmpHits.Length; i++)
        {
            if(tmpHits[i].collider.transform == _weaponHolder) continue; // _weaponHolder �迭���� �����ϱ�
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
                // �� ���, ����, ���� ����Ʈ ��ο� ���� �̹��� �߰�

                Debug.Log("beforeTmpPenetratePower: " + tmpPenetratePower);

                tmpPenetratePower -= finalDurability;

                Debug.Log("afterTmpPenetratePower: " + tmpPenetratePower);

                if(i == penetrateDatas.Count - 1) // ���� ������ ������ �浹�� ���
                {
                    Vector3 offsetDir = (penetrateDatas[i].ExitPoint - muzzle.position).normalized * trajectoryLineOffset;
                    DrawTrajectoryLine(penetrateDatas[i].ExitPoint + offsetDir);
                }
            }
            else
            {
                SpawnBulletHole(penetrateDatas[i].EntryPoint, penetrateDatas[i].EntryNormal, true);
                DrawTrajectoryLine(penetrateDatas[i].EntryPoint);
                break; // ���⼭ ������
                // �� ���, ������ ������ �� ����Ʈ���� �Ѿ� ���� �׸���
                // �� ����Ʈ�� �Ѿ��� ���� �̹��� �߰�
            }


            if (penetrateDatas.Count <= i + 1) continue; // �ڿ� ���� ������ ���ٸ� �������� ����
            Debug.Log("DistanceBetweenTwoPoints");

            Debug.Log("beforeTmpPenetratePower: " + tmpPenetratePower);

            float distanceBetweenExitAndEntryPoint = Vector3.Distance(penetrateDatas[i].ExitNormal, penetrateDatas[i + 1].EntryNormal);
            float finalDistanceBetweenExitAndEntryPoint = distanceBetweenExitAndEntryPoint * penetrateDatas[i].AirDurability;

            Debug.Log("distanceBetweenExitAndEntryPoint : " + distanceBetweenExitAndEntryPoint);
            Debug.Log("finalDistanceBetweenExitAndEntryPoint : " + finalDistanceBetweenExitAndEntryPoint);

            tmpPenetratePower -= finalDistanceBetweenExitAndEntryPoint;
            Debug.Log("afterTmpPenetratePower: " + tmpPenetratePower);

            // �߰���, ������ �Ǵ��� ���� ����Ʈ ���� �������� �Ÿ��� �ִٸ� ������ �����ֱ�
            // �� ������ �Ÿ� * (������ �� �� * N) ������ �����Ѵ�.

            // ���� ���� ��ġ�� ����ؼ� �Ѿ� ������ ���� �׷�����
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
