using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Database : MonoBehaviour
{
    [SerializeField] SpriteDictionary _iconDictionary;
    [SerializeField] SpriteDictionary _previewDictionary;
    [SerializeField] ProfileDictionary _profileDictionary;

    [Serializable]
    public enum IconName
    {
        ArmorIcon,
        AidKitIcon,
        AmmoIcon,

        PistolIcon,
        SMGIcon,

        AKIcon,
        ARIcon,
        SniperIcon,

        AutoShotgunIcon,
        DMRIcon,
        KnifeIcon,
        LMGIcon,
        ReviveIcon
    }

    [Serializable]
    public enum PersonName
    {
        Warden,
        Rook,
        Oryx,
        Player
    }

    static Database _instance = null;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else Destroy(gameObject);
    }

    public static Sprite ReturnIcon(IconName name)
    {
        return _instance._iconDictionary[name];
    }

    public static Sprite ReturnPreview(IconName name)
    {
        return _instance._previewDictionary[name];
    }

    public static Sprite ReturnProfile(PersonName name)
    {
        return _instance._profileDictionary[name];
    }
}
