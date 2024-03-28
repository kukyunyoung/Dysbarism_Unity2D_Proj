using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlotUI : MonoBehaviour
{
    public ItemData WillBuyObj => willBuyObj;
    public int Price => price;
    public Button Btn => btn;

    [SerializeField] Image itemImage;
    [SerializeField] Text itemName;
    [SerializeField] Text itemSummary;
    [SerializeField] Text itemPrice;
    [SerializeField] ItemData willBuyObj;
    public GameObject soldOut;

    Button btn;
    SmithShop smithShop;
    WitchShop witchShop;
    int price;

    public void Init(ItemData item)
    {
        itemImage.sprite = item.IconSprite;
        itemName.text = item.Name;
        itemSummary.text = item.Tooltip;
        itemPrice.text = item.Price.ToString() + " G";
        price = item.Price;
        willBuyObj = item;
    }

    void Start()
    {
        smithShop = GameObject.FindWithTag("NpcCanvas").transform.GetChild(0).GetComponent<SmithShop>();
        witchShop = GameObject.FindWithTag("NpcCanvas").transform.GetChild(1).GetComponent<WitchShop>();
        btn = GetComponent<Button>();
    }

    public void Buy()
    {
        if(UIManager.coin * 100 < price) { UIManager.instance.SetSystemMessage("골드가 부족합니다!!"); return; }
        if(smithShop.buyPanel.activeSelf || witchShop.buyPanel.activeSelf) return;

        if(willBuyObj is EquipmentItemData)
        {
            smithShop.nowSlot = this;
            smithShop.buyPanel.SetActive(true);
        }
        else if(willBuyObj is PortionItemData)
        {
            witchShop.nowSlot = this;
            witchShop.buyPanel.SetActive(true);
        }
    }
}
