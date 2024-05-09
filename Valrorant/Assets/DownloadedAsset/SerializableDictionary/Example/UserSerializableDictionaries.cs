using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DamageUtility;

[Serializable]
public class RecoilDataDictionary : SerializableDictionary<BaseWeapon.EventType, TextAsset> { }

[Serializable]
public class WeaponDataDictionary : SerializableDictionary<BaseWeapon.Name, WeaponFactoryData> { }



//[Serializable]
//public class Dictionary<HitArea, DistanceAreaData[]> : SerializableDictionary<HitArea, DistanceAreaData[]> { }

//[Serializable]
//public class DistanceAreaStorage : SerializableDictionary.Storage<DistanceAreaData[]> { }

//[Serializable]
//public class Dictionary<HitArea, DistanceAreaData[]> : SerializableDictionary<HitArea, DistanceAreaData[]> { }


[Serializable]
public class StringStringDictionary : SerializableDictionary<string, string> {}

[Serializable]
public class ObjectColorDictionary : SerializableDictionary<UnityEngine.Object, Color> {}

[Serializable]
public class ColorArrayStorage : SerializableDictionary.Storage<Color[]> {}

[Serializable]
public class StringColorArrayDictionary : SerializableDictionary<string, Color[], ColorArrayStorage> {}

[Serializable]
public class MyClass
{
    public int i;
    public string str;
}

[Serializable]
public class QuaternionMyClassDictionary : SerializableDictionary<Quaternion, MyClass> {}