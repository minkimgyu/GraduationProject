using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class ResultStrategy
{
    public abstract void Attack();
    public virtual void UnEquip() { }

    protected virtual void SpawnHitEffect(Vector3 hitPosition, Vector3 hitNormal) { }
    protected virtual void SpawnHitEffect(Vector3 hitPosition, Vector3 hitNormal, bool isBlocked) { }
}

public class NoAttack : ResultStrategy
{
    public override void Attack() { }
}

public class Zoom : ResultStrategy
{
    // ��ũ���ͺ� ������Ʈ�� �����ϱ�

    AimEvent _aimEvent;
    GameObject _scope;

    bool nowZoom = false;

    public Zoom(AimEvent aimEvent, GameObject scope)
    {
        _aimEvent = aimEvent;
        _scope = scope;
    }

    public override void Attack()
    {
        nowZoom = !nowZoom;
        _aimEvent.OnAimRequested(nowZoom);
        _scope.SetActive(nowZoom);
    }

    public override void UnEquip()
    {
        _aimEvent.OnAimRequested(false);
        _scope.SetActive(false);
    }
}


abstract public class ApplyAttack : ResultStrategy
{
    protected Transform _camTransform;

    protected float _range;
    protected string _hitEffect;
    protected int _targetLayer;

    public ApplyAttack(Transform camTransform, float range, string hitEffect, int targetLayer)
    {
        _camTransform = camTransform;
        _range = range;
        _hitEffect = hitEffect;
        _targetLayer = targetLayer;
    }

    protected void DrawDebugLine(Vector3 offset = default(Vector3))
    {
        Debug.DrawRay(_camTransform.position, (_camTransform.forward + offset) * _range, Color.green, 10); // ����� ���̸� ī�޶� �ٶ󺸰� �ִ� �������� �߻��Ѵ�.
    }

    protected void DrawDebugLine1(Vector3 offset = default(Vector3))
    {
        Debug.DrawRay(_camTransform.position, -(_camTransform.forward + offset) * _range, Color.green, 10); // ����� ���̸� ī�޶� �ٶ󺸰� �ִ� �������� �߻��Ѵ�.
    }
}

abstract public class BaseGunAttack : ApplyAttack
{
    protected Transform _muzzle;

    protected Transform _weaponHolder;

    protected float _penetratePower = 15;
    protected float _trajectoryLineOffset = 1.3f;

    protected string _nonPenetrateHitEffect;
    protected string _trajectoryLineEffect;

    public BaseGunAttack(Transform camTransform, float range, string hitEffect, int targetLayer, Transform weaponHolder, Transform muzzle, float penetratePower,
        string nonPenetrateHitEffect, string trajectoryLineEffect) : base(camTransform, range, hitEffect, targetLayer)
    {
        _weaponHolder = weaponHolder;
        _muzzle = muzzle;
        _penetratePower = penetratePower;

        _nonPenetrateHitEffect = nonPenetrateHitEffect;
        _trajectoryLineEffect = trajectoryLineEffect;
    }

    protected List<PenetrateData> ReturnPenetrateData(Vector3 offset = default(Vector3))
    {
        RaycastHit[] entryHits;
        entryHits = Physics.RaycastAll(_camTransform.position, _camTransform.forward + offset, _range, _targetLayer);

        if (entryHits.Length == 0) return null; // ���� �ƹ��� ���� �ʾҴٸ� ����

        System.Array.Sort(entryHits, (x, y) => x.distance.CompareTo(y.distance)); // ���� ��ü���� �Ÿ� ������� �������ش�.

        Vector3 endPoint = _camTransform.position + (_camTransform.forward + offset) * _range; // ����ĳ��Ʈ�� �ݴ� ���� ������ ��ġ

        RaycastHit[] tmpHits;
        tmpHits = Physics.RaycastAll(endPoint, -(_camTransform.forward + offset), _range, _targetLayer); // �ٽ� �ݴ�� ����

        DrawDebugLine1(offset);

        System.Array.Sort(tmpHits, (x, y) => y.distance.CompareTo(x.distance)); // ���� ��ü���� �Ÿ� ������� �������ش�.

        List<RaycastHit> exitHits = new List<RaycastHit>();

        for (int i = 0; i < tmpHits.Length; i++)
        {
            if (tmpHits[i].collider.transform == _weaponHolder)
            {
                continue; // _weaponHolder �迭���� �����ϱ�
            }
            exitHits.Add(tmpHits[i]);
        }

        List<PenetrateData> penetrateDatas = new List<PenetrateData>();

        Debug.Log(entryHits.Length);
        Debug.Log(exitHits.Count);

        if (entryHits.Length != exitHits.Count) return null;

        for (int i = 0; i < entryHits.Length; i++)
        {
            if (entryHits[i].collider != exitHits[i].collider) continue;

            IPenetrateTarget target = entryHits[i].collider.GetComponent<IPenetrateTarget>();
            if (target == null) continue;

            penetrateDatas.Add(new PenetrateData(entryHits[i].point, exitHits[i].point, entryHits[i].normal, exitHits[i].normal, target));
        }

        return penetrateDatas;
    }

    protected void CalculatePenetratePower(List<PenetrateData> penetrateDatas)
    {
        float tmpPenetratePower = _penetratePower;

        for (int i = 0; i < penetrateDatas.Count; i++)
        {
            float finalDurability = penetrateDatas[i].ReturnFinalDurability();

            if (tmpPenetratePower - finalDurability >= 0)
            {
                SpawnHitEffect(penetrateDatas[i].EntryPoint, penetrateDatas[i].EntryNormal, false);
                SpawnHitEffect(penetrateDatas[i].ExitPoint, penetrateDatas[i].ExitNormal, false);
                // �� ���, ����, ���� ����Ʈ ��ο� ���� �̹��� �߰�

                tmpPenetratePower -= finalDurability;

                if (i == penetrateDatas.Count - 1) // ���� ������ ������ �浹�� ���
                {
                    Vector3 offsetDir = (penetrateDatas[i].ExitPoint - _muzzle.position).normalized * _trajectoryLineOffset;
                    DrawTrajectoryLine(penetrateDatas[i].ExitPoint + offsetDir);
                }
            }
            else
            {
                SpawnHitEffect(penetrateDatas[i].EntryPoint, penetrateDatas[i].EntryNormal, true);
                DrawTrajectoryLine(penetrateDatas[i].EntryPoint);
                break; // ���⼭ ������
                // �� ���, ������ ������ �� ����Ʈ���� �Ѿ� ���� �׸���
                // �� ����Ʈ�� �Ѿ��� ���� �̹��� �߰�
            }

            if (penetrateDatas.Count <= i + 1) continue; // �ڿ� ���� ������ ���ٸ� �������� ����

            float distanceBetweenExitAndEntryPoint = Vector3.Distance(penetrateDatas[i].ExitNormal, penetrateDatas[i + 1].EntryNormal);
            float finalDistanceBetweenExitAndEntryPoint = distanceBetweenExitAndEntryPoint * penetrateDatas[i].AirDurability;

            tmpPenetratePower -= finalDistanceBetweenExitAndEntryPoint;
        }
    }

    protected override void SpawnHitEffect(Vector3 hitPosition, Vector3 hitNormal, bool isBlocked)
    {
        BulletHoleEffect bulletHoleEffect;

        if (isBlocked) bulletHoleEffect = ObjectPooler.SpawnFromPool<BulletHoleEffect>(_nonPenetrateHitEffect);
        else bulletHoleEffect = ObjectPooler.SpawnFromPool<BulletHoleEffect>(_hitEffect);

        bulletHoleEffect.Initialize(hitPosition, hitNormal, Quaternion.LookRotation(-hitNormal));
        bulletHoleEffect.PlayEffect();
    }

    void DrawTrajectoryLine(Vector3 hitPosition)
    {
        TrajectoryLineEffect trajectoryLineEffect = ObjectPooler.SpawnFromPool<TrajectoryLineEffect>(_trajectoryLineEffect);
        trajectoryLineEffect.Initialize(hitPosition, _muzzle.position);
        trajectoryLineEffect.PlayEffect();
    }
}

public class SingleProjectileAttack : BaseGunAttack
{
    // NormalGun, ScatterGun ������ ���⿡ ���� �Լ� ����

    public SingleProjectileAttack(Transform camTransform, float range, string hitEffect, int targetLayer, Transform weaponHolder, Transform muzzle, float penetratePower,
        string nonPenetrateHitEffect, string trajectoryLineEffect) 
        : base(camTransform, range, hitEffect, targetLayer, weaponHolder, muzzle, penetratePower, nonPenetrateHitEffect, trajectoryLineEffect)
    {
    }

    public override void Attack()
    {
        //if (CanShoot() == false) return;

        DrawDebugLine();

        List<PenetrateData> penetrateDatas = ReturnPenetrateData();
        if (penetrateDatas == null) return;

        CalculatePenetratePower(penetrateDatas);
    }
}

public class ScatterProjectileGunAttack : BaseGunAttack
{
    int _projectileCounts;

    float _spreadOffset;

    public ScatterProjectileGunAttack(Transform camTransform, float range, string hitEffect, int targetLayer, Transform weaponHolder, Transform muzzle, float penetratePower,
       string nonPenetrateHitEffect, string trajectoryLineEffect, int projectileCounts, float spreadOffset)
       : base(camTransform, range, hitEffect, targetLayer, weaponHolder, muzzle, penetratePower, nonPenetrateHitEffect, trajectoryLineEffect)
    {
        _projectileCounts = projectileCounts;
        _spreadOffset = spreadOffset;
    }

    List<Vector3> ReturnOffsetDistance()
    {
        List<Vector3> offsetDistance = new List<Vector3>();

        for (int i = 0; i < _projectileCounts; i++)
        {
            float x = Random.Range(-_spreadOffset, _spreadOffset);
            float y = Random.Range(-_spreadOffset, _spreadOffset);
            offsetDistance.Add(new Vector3(0, y, x));
        }

        return offsetDistance;
    }

    public override void Attack()
    {
        List<Vector3> offsetDistances = ReturnOffsetDistance();

        for (int i = 0; i < offsetDistances.Count; i++)
        {
            DrawDebugLine(offsetDistances[i]);

            List<PenetrateData> penetrateDatas = ReturnPenetrateData(offsetDistances[i]);
            if (penetrateDatas == null) continue;

            CalculatePenetratePower(penetrateDatas);
        }
    }
}

public class KnifeAttack : ApplyAttack
{
    // Į ������ ���⼭ ����
    public KnifeAttack(Transform camTransform, float range, string hitEffect, int targetLayer) : base(camTransform, range, hitEffect, targetLayer)
    {
    }

    void CheckRaycastHit()
    {
        RaycastHit hit;
        Physics.Raycast(_camTransform.position, _camTransform.forward, out hit, _range, _targetLayer);

        if (hit.collider == null) return; 

        SpawnHitEffect(hit.point, hit.normal);
    }

    public override void Attack()
    {
        DrawDebugLine();
        CheckRaycastHit();
    }

    protected override void SpawnHitEffect(Vector3 hitPosition, Vector3 hitNormal)
    {
        BaseEffect bulletHoleEffect;

        bulletHoleEffect = ObjectPooler.SpawnFromPool<BaseEffect>(_hitEffect);

        bulletHoleEffect.Initialize(hitPosition, hitNormal, Quaternion.LookRotation(-hitNormal));
        bulletHoleEffect.PlayEffect();
    }
}
