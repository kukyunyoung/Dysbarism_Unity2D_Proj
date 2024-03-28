using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CountableItemData : ItemData
{
    public int MaxAmount => maxAmount;
    [SerializeField] int maxAmount = 99;
}
