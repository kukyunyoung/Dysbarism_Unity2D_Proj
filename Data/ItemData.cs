using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    public int ID => id;
    public string Name => itemName;
    public string Tooltip => tooltip;
    public Sprite IconSprite => iconSprite;
    public int Price => price;

    [SerializeField] private int id;
    [SerializeField] private string itemName;    
    [Multiline]
    [SerializeField] private string tooltip; 
    [SerializeField] private Sprite iconSprite; 
    [SerializeField] private GameObject dropItemPrefab;
    [SerializeField] public GameObject instItemPrefab;
    [SerializeField] int price;

    public abstract Item CreateItem();
}
