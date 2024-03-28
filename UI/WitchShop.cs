using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WitchShop : MonoBehaviour
{
    [SerializeField] ShopSlotUI slotPrefab;
    [SerializeField] ItemData[] itemData;
    [SerializeField] GameObject parent;
    public int price;
    public GameObject buyPanel;
    public ItemData willBuyItem;
    public ShopSlotUI nowSlot;

    public List<ShopSlotUI> slotList = new List<ShopSlotUI>();
    int random;
    Inventory inventory;

    private void Start()
    {
        inventory = GameObject.FindWithTag("Inventory").GetComponent<Inventory>();
        int slotRange = Random.Range(2, 5);
        for (int i = 0; i < slotRange; i++)
        {
            random = Random.Range(0, itemData.Length);
            ShopSlotUI go = Instantiate(slotPrefab);
            go.transform.parent = parent.transform;
            go.Init(itemData[random]);
            slotList.Add(go);
        }
    }

    public void BuyPanel(bool choice)
    {
        price = nowSlot.Price;
        willBuyItem = nowSlot.WillBuyObj;

        if (!choice) buyPanel.SetActive(false);
        else
        {
            UIManager.coin -= price / 100;
            inventory.Add(willBuyItem);
            nowSlot.soldOut.SetActive(true);
            nowSlot.Btn.enabled = false;
            buyPanel.SetActive(false);
        }
    }
}
