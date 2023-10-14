using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageUtility
{
    [System.Serializable]
    public struct DistanceAreaData
    {
        public DistanceAreaData(float minDistance, float maxDistance, float damage)
        {
            _minDistance = minDistance;
            _maxDistance = maxDistance;
            _damage = damage;
        }

        public enum HitArea
        {
            Head,
            Body,
            Leg,
        }

        float _minDistance;

        float _maxDistance;

        float _damage;

        public float Damage { get { return _damage; } }

        public bool IsInRange(float distance)
        {
            if (_minDistance <= distance && distance <= _maxDistance)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class BaseDamageConverter
    {
        public virtual float ReturnDamage(DistanceAreaData.HitArea hitArea, float distance) { return 0; }
        public virtual float ReturnDamage(Vector3 playerFoward, Vector3 targetFoward) { return 0; }
    }

    public class DistanceAreaBasedDamageConverter : BaseDamageConverter
    {
        Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> _damageDictionary;

        public DistanceAreaBasedDamageConverter(Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> damageDictionary)
        {
            _damageDictionary = damageDictionary;
        }

        public override float ReturnDamage(DistanceAreaData.HitArea hitArea, float distance)
        {
            if (_damageDictionary.ContainsKey(hitArea) == false) return 0;

            for (int i = 0; i < _damageDictionary[hitArea].Length; i++)
            {
                if (_damageDictionary[hitArea][i].IsInRange(distance) == true)
                {
                    return _damageDictionary[hitArea][i].Damage;
                }
            }

            // 만약 해당하는 범위에 존재하지 않는다면 --> 범위가 넘어간다면
            int length = _damageDictionary[hitArea].Length - 1;

            return _damageDictionary[hitArea][length].Damage; // 마지막 데미지 값을 넣어줌
        }
    }

    [System.Serializable]
    public struct DirectionData
    {
        [SerializeField]
        float _frontAttackDamage;
        public float FrontAttackDamage { get { return _frontAttackDamage; } }

        [SerializeField]
        float _backAttackDamage;
        public float BackAttackDamage { get { return _backAttackDamage; } }
    }

    public class DirectionBasedDamageConverter : BaseDamageConverter
    {
        DirectionData _directionData;

        public DirectionBasedDamageConverter(DirectionData directionData)
        {
            _directionData = directionData;
        }

        public override float ReturnDamage(Vector3 playerFoward, Vector3 targetFoward)
        {
            float angle = Vector3.Angle(playerFoward, targetFoward);

            if (Mathf.Abs(angle) < 90)
            {
                return _directionData.FrontAttackDamage;
            }
            else
            {
                return _directionData.BackAttackDamage;
            }
        }
    }
}