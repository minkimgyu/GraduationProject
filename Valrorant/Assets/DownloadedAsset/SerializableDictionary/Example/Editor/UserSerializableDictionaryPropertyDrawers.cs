using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(RecoilDataDictionary))]
[CustomPropertyDrawer(typeof(WeaponDataDictionary))]
//[CustomPropertyDrawer(typeof(Dictionary<HitArea, DistanceAreaData[]>))]
//[CustomPropertyDrawer(typeof(Dictionary<HitArea, DistanceAreaData[]>1))]


[CustomPropertyDrawer(typeof(StringStringDictionary))]
[CustomPropertyDrawer(typeof(ObjectColorDictionary))]
[CustomPropertyDrawer(typeof(StringColorArrayDictionary))]
public class AnySerializableDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer {}

//[CustomPropertyDrawer(typeof(DistanceAreaStorage))]
[CustomPropertyDrawer(typeof(ColorArrayStorage))]
public class AnySerializableDictionaryStoragePropertyDrawer: SerializableDictionaryStoragePropertyDrawer {}
