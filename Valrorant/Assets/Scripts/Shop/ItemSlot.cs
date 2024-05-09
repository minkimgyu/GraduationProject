using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

namespace Shop
{
    public struct ItemData
    {
        public enum Type
        {
            Equipment,
            Side,
            Main
        }

        Type _type;
        int _number;
        int _cost;

        string _name;

        public int Number { get { return _number; } }
        public string Name { get { return _name; } }
        public int Cost { get { return _cost; } }

        public ItemData(Type type, int number, string name, int cost)
        {
            _type = type;
            _number = number;
            _name = name;
            _cost = cost;
        }
    }

    public class ItemSlot : MonoBehaviour
    {
        [SerializeField] TMP_Text _numberTxt;
        [SerializeField] TMP_Text _costTxt;

        [SerializeField] TMP_Text _nameTxt;

        [SerializeField] Button _button;

        Func<int> OnAssetRequested;

        ItemData _data;

        public void Initialize(ItemData data, Func<int> OnAssetRequested)
        {
            _button.onClick.AddListener(Buy);

            _data = data;
            ResetTxt();
        }

        protected virtual void Buy()
        {
            int asset = OnAssetRequested();
            if (asset < _data.Cost) return;


        }

        void ResetTxt()
        {
            _numberTxt.text = _data.Number.ToString();
            _costTxt.text = _data.Cost.ToString();

            _nameTxt.text = _data.Name;
        }
    }


    // 이벤트를 넣어서 클릭 시 작동하는 함수를 다르게 하기
    public class WeaponSlot : ItemSlot
    {
        protected override void Buy()
        {

        }
    }

    public class EquipmentSlot : ItemSlot
    {
        protected override void Buy()
        {

        }
    }
}