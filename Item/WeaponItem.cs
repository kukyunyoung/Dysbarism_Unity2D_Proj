using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItem : Item, IWeaponItem
{
    public WeaponItem(WeaponItemData data) : base(data) { }

    public WeaponItem Equip()
    {
        Debug.Log("WeaponItem Message : Equip");

        return this;
    }
}
