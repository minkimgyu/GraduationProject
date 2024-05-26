using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(AudioClipDictionary))]
[CustomPropertyDrawer(typeof(RecoilDataDictionary))]
[CustomPropertyDrawer(typeof(WeaponDataDictionary))]
[CustomPropertyDrawer(typeof(CharacterDataDictionary))]
[CustomPropertyDrawer(typeof(SlotDataDictionary))]

[CustomPropertyDrawer(typeof(SpriteDictionary))]
[CustomPropertyDrawer(typeof(ProfileDictionary))]
[CustomPropertyDrawer(typeof(ItemSlotDictionary))]
//[CustomPropertyDrawer(typeof(Dictionary<HitArea, DistanceAreaData[]>))]
//[CustomPropertyDrawer(typeof(Dictionary<HitArea, DistanceAreaData[]>1))]


[CustomPropertyDrawer(typeof(StringStringDictionary))]
[CustomPropertyDrawer(typeof(ObjectColorDictionary))]
[CustomPropertyDrawer(typeof(StringColorArrayDictionary))]
public class AnySerializableDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer {}

//[CustomPropertyDrawer(typeof(DistanceAreaStorage))]
[CustomPropertyDrawer(typeof(ColorArrayStorage))]
public class AnySerializableDictionaryStoragePropertyDrawer: SerializableDictionaryStoragePropertyDrawer {}
