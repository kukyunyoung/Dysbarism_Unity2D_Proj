using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccessaryItem : Item, IAccItem
{
    public AccessaryItem(AccessaryItemData data) : base(data) { }

    public AccessaryItem Equip()
    {
        Debug.Log("AccItem Message : Equip");

        return this;
    }
}
