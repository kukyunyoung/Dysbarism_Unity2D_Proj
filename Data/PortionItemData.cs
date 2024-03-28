using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item_Portion", menuName = "Inventory System/Item Data/Portion", order = 3)]

public class PortionItemData : CountableItemData
{
    [SerializeField] float value;
    public float Value => value;

    public int Power => power;
    public int Armory => armory;
    public int Critical => critical;
    public int Avoidance => avoidance;
    public float SwSpeed => swSpeed;
    public float ReloadSpeed => reloadSpeed;
    public float ChargeSpeed => chargeSpeed;

    [SerializeField] int power = 0;
    [SerializeField] int armory = 0;
    [SerializeField] int critical = 0;
    [SerializeField] int avoidance = 0;
    [SerializeField] float swSpeed = 0;
    [SerializeField] float reloadSpeed = 0;
    [SerializeField] float chargeSpeed = 0;

    public override Item CreateItem()
    {
        return new PortionItem(this);
    }
}
