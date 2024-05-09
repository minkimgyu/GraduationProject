using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace DamageUtility
{
    [System.Serializable]
    public enum HitArea
    {
        Head,
        Body,
        Leg,
    }

    [System.Serializable]
    public struct DistanceAreaData
    {
        public DistanceAreaData(float minDistance, float maxDistance, float damage)
        {
            _minDistance = minDistance;
            _maxDistance = maxDistance;
            _damage = damage;
        }

        public float _minDistance;

        public float _maxDistance;

        public float _damage;

        [JsonIgnore] public float Damage { get { return _damage; } }

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

    [System.Serializable]
    public struct DirectionData
    {
        public float _frontAttackDamage;
        [JsonIgnore] public float FrontAttackDamage { get { return _frontAttackDamage; } }

        public float _backAttackDamage;
        [JsonIgnore] public float BackAttackDamage { get { return _backAttackDamage; } }
    }

    public class BaseDamageConverter
    {
        public virtual float ReturnDamage(HitArea hitArea, float distance) { return 0; }
        public virtual float ReturnDamage(Vector3 playerFoward, Vector3 targetFoward) { return 0; }
    }

    public class DistanceAreaBasedDamageConverter : BaseDamageConverter
    {
        Dictionary<HitArea, DistanceAreaData[]> _damageDictionary;

        public DistanceAreaBasedDamageConverter(Dictionary<HitArea, DistanceAreaData[]> damageDictionary)
        {
            _damageDictionary = damageDictionary;
        }

        public override float ReturnDamage(HitArea hitArea, float distance)
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

    public class DirectionBasedDamageConverter : BaseDamageConverter
    {
        DirectionData _directionData;
        float _backAngle = 60;

        public DirectionBasedDamageConverter(DirectionData directionData)
        {
            _directionData = directionData;
        }

        public override float ReturnDamage(Vector3 playerFoward, Vector3 targetFoward)
        {
            float angle = Vector3.Angle(playerFoward, targetFoward);

            if (Mathf.Abs(angle) < _backAngle) // 뒷 각
            {
                return _directionData.BackAttackDamage;
            }
            else
            {
                return _directionData.FrontAttackDamage;
            }
        }
    }
}