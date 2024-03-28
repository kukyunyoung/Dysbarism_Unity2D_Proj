using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item_Weapon_", menuName = "Inventory System/Item Data/Weapon", order = 1)]

public class WeaponItemData : EquipmentItemData
{
    /// <summary> 공격력 </summary>
    public int Damage => _damage;

    [SerializeField] private int _damage = 1;
    public enum Grade {Normal, Rare, Unique}
    public Grade grade;
    public WeaponItemData nextItemData;

    public override Item CreateItem()
    {
        return new WeaponItem(this);
    }
}
