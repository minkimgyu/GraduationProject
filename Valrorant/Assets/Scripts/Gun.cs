using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public enum PenetrateTarget
//{
//    Wood,
//    Rock,
//    Iron,
//    Agent // �÷��̾ ���뼦�� �����ϹǷ� ���⿡ �ִ´�.
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
        return Vector3.Distance(_entryPoint, _exitPoint) * _target.ReturnDurability(); // �Ÿ��� ������ ������
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

        System.Array.Sort(hitObjects, (x, y) => x.distance.CompareTo(y.distance)); // ���� ��ü���� �Ÿ� ������� �������ش�.

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
        Debug.DrawRay(_camTransform.position, _camTransform.forward * range, Color.green, 10); // ����� ���̸� ī�޶� �ٶ󺸰� �ִ� �������� �߻��Ѵ�.

        RaycastHit[] entryHits;
        entryHits = Physics.RaycastAll(_camTransform.position, _camTransform.forward, range);

        if (entryHits.Length == 0) return; // ���� �ƹ��� ���� �ʾҴٸ� ����

        System.Array.Sort(entryHits, (x, y) => x.distance.CompareTo(y.distance)); // ���� ��ü���� �Ÿ� ������� �������ش�.

        Vector3 endPoint = _camTransform.position + _camTransform.forward * range; // ����ĳ��Ʈ�� �ݴ� ���� ������ ��ġ

        RaycastHit[] exitHits;
        exitHits = Physics.RaycastAll(endPoint, -_camTransform.position, range); // �ٽ� �ݴ�� ����

        System.Array.Sort(exitHits, (x, y) => y.distance.CompareTo(x.distance)); // ���� ��ü���� �Ÿ� ������� �������ش�.
        //System.Array.Reverse(exitHits); // �ش� ����Ʈ�� �������ش�.


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
                // �� ���, ����, ���� ����Ʈ ��ο� ���� �̹��� �߰�
            }
            else
            {
                // �� ���, ������ ������ �� ����Ʈ���� �Ѿ� ���� �׸���
                // �� ����Ʈ�� �Ѿ��� ���� �̹��� �߰�
            }

            // �߰���, ������ �Ǵ��� ���� ����Ʈ ���� �������� �Ÿ��� �ִٸ� ������ �����ֱ�
            // �� ������ �Ÿ� * (������ �� �� * N) ������ �����Ѵ�.

            // ���� ���� ��ġ�� ����ؼ� �Ѿ� ������ ���� �׷�����
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
