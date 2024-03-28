using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item_Accessary_", menuName = "Inventory System/Item Data/Accessary", order = 1)]

public class AccessaryItemData : EquipmentItemData
{
    public int Power => power;
    public int Armory => armory;
    public int Critical => critical;
    public int Avoidance => avoidance;
    public float SwSpeed => swSpeed;
    public float ReloadSpeed => reloadSpeed;
    public float ChargeSpeed => chargeSpeed;

    [SerializeField] int power=0;
    [SerializeField] int armory = 0;
    [SerializeField] int critical = 0;
    [SerializeField] int avoidance = 0;
    [SerializeField] float swSpeed = 0;
    [SerializeField] float reloadSpeed = 0;
    [SerializeField] float chargeSpeed = 0;

    public override Item CreateItem()
    {
        return new AccessaryItem(this);
    }
}
