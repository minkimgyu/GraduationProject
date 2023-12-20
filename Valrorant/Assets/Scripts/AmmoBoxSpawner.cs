using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AmmoBoxSpawner : MonoBehaviour
{
    [Serializable]
    public struct SpawnData
    {
        [SerializeField] Transform _spawnPoints;
        [SerializeField] GameObject _spawnedAmmoBox;

        public Transform SpawnPoints { get { return _spawnPoints; } }
        public GameObject SpawnedAmmoBox { get { return _spawnedAmmoBox; } set { _spawnedAmmoBox = value; } }
    }

    [SerializeField] SpawnData[] _spawnDatas;
    [SerializeField] GameObject ammoPrefab;
    [SerializeField] float _spawnDelayTime = 5;
    [SerializeField] Vector3 _spawnOffset = new Vector3(0, 0.5f, 0);

    Timer _spawnTimer;

    private void Start()
    {
        _spawnTimer = new Timer();
        SpawnAmmoBox();
    }

    float ReturnDistanceBetweenTwoPoints(Vector3 pos1, Vector3 pos2)
    {
        return Vector3.Distance(pos1, pos2);
    }

    public Vector3 ReturnClosestPoint(Vector3 objectPos)
    {
        float distance = 0;
        int closestIndex = 0;

        for (int i = 0; i < _spawnDatas.Length; i++)
        {
            float nowIndexDistance = ReturnDistanceBetweenTwoPoints(_spawnDatas[i].SpawnPoints.position, objectPos);

            if (i == 0)
            {
                distance = nowIndexDistance;
            }
            else
            {
                if (distance <= nowIndexDistance || _spawnDatas[i].SpawnedAmmoBox == null) continue;

                distance = nowIndexDistance;
                closestIndex = i;
            }
        }

        return _spawnDatas[closestIndex].SpawnPoints.position;
    }

    void SpawnAmmoBox()
    {
        for (int i = 0; i < _spawnDatas.Length; i++)
        {
            if (_spawnDatas[i].SpawnedAmmoBox != null) continue;
            _spawnDatas[i].SpawnedAmmoBox = Instantiate(ammoPrefab, _spawnDatas[i].SpawnPoints.position + _spawnOffset, Quaternion.identity);
        }
    }

    private void Update()
    {
        _spawnTimer.Update();

        if(_spawnTimer.IsFinish)
        {
            // 비어있는지 확인하고 다시 스폰시켜주기
            SpawnAmmoBox();
            _spawnTimer.Reset();
            _spawnTimer.Start(_spawnDelayTime);
        }
    }
}
