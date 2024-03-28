using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CountableItem : Item
{
    public CountableItemData CountableData { get; protected set; }
    public int Amount { get; protected set; } 
    public int MaxAmount => CountableData.MaxAmount; 
    public bool IsMax => Amount >= CountableData.MaxAmount; 
    public bool IsEmpty => Amount <= 0;

    public CountableItem(CountableItemData data, int amount=1) : base(data)
    {
        CountableData = data;
        SetAmount(amount);
    }

    public void SetAmount(int amount)
    {
        Amount = Mathf.Clamp(amount, 0, MaxAmount);
    }

    public int AddAmountAndGetExcess(int amount)
    {
        int nextAmount = Amount + amount;
        SetAmount(nextAmount);

        return (nextAmount>MaxAmount) ? (nextAmount - MaxAmount) : 0;
    }

    public CountableItem SeperateAndClone(int amount)
    {
        if (amount <= 1) return null;

        if(amount > Amount -1) amount = Amount -1;

        Amount = amount;
        return Clone(amount);
    }

    protected abstract CountableItem Clone(int amount);
}
