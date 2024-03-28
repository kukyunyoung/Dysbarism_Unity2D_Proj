using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUpgrade : MonoBehaviour
{
    [SerializeField] ItemSlotUI itemSlotUI;
    [SerializeField] Button upgradeBtn;
    [SerializeField] Text summaryText;
    [SerializeField] Text price;
    [SerializeField] Image fadePanel;
    [SerializeField] Inventory inventory;
    [SerializeField] GameObject successPanel;
    [SerializeField] Npc UpgradeNpc;

    public WeaponItemData itemData;
    public int slotNum;
    WeaponItemData.Grade nextGrade;
    Color fade = new Color(0,0,0,0.01f);
    bool isDoing;

    void Start()
    {
        upgradeBtn.onClick.AddListener(() => UpgradeBtn());
        inventory = GameObject.FindWithTag("Inventory").GetComponent<Inventory>();
        isDoing = false;
    }

    private void OnEnable() 
    {
        inventory.GetComponent<InventoryManager>().weaponUpgrade = this;
    }

    private void OnDisable() 
    {
        Init();
    }

    public void SetIcon()
    {
        itemSlotUI.SetItem(itemData.IconSprite);
        if(itemData.grade == WeaponItemData.Grade.Normal) nextGrade = WeaponItemData.Grade.Rare;
        else if(itemData.grade == WeaponItemData.Grade.Rare) nextGrade = WeaponItemData.Grade.Unique;
        summaryText.text = 
            "현재 등급 : " + itemData.grade + "\n" +
            "다음 등급 : " + nextGrade;
        price.text = (itemData.nextItemData.Price * 0.5f).ToString() + " G";
    }

    public void UpgradeBtn()
    {
        // 최종적으로 확인버튼 누르면
        if(UIManager.coin > itemData.nextItemData.Price * 0.005f)
        {
            UIManager.coin -= (int)(itemData.nextItemData.Price * 0.005f);
            StartCoroutine(DisplayFlash());
        }
        else
            UIManager.instance.SetSystemMessage("소지한 골드가 부족합니다!");
    }

    IEnumerator DisplayFlash()
    {
        for(int i=0; i<100; i++)
        {
            fadePanel.color += fade;
            yield return new WaitForSeconds(0.01f);
        }

        UpgradeItem();

        yield return new WaitForSeconds(1);

        successPanel.gameObject.transform.GetChild(0).GetComponent<Image>().sprite = itemData.nextItemData.IconSprite;
        successPanel.gameObject.transform.GetChild(1).GetComponent<Text>().text = itemData.nextItemData.Name;
        successPanel.gameObject.SetActive(true);

        Init();
        UpgradeNpc.npcOn = false;
        UIManager.instance.stopTime--;
        UIManager.instance.EscapeInvenUI();
        itemSlotUI.SetItem(null);
        isDoing = false;
        gameObject.SetActive(false);
        fadePanel.color = new Color(1,1,1,0);
    }

    void UpgradeItem()
    {
        if (isDoing) return;
        isDoing = true;
        print(slotNum);

        // 클릭했던 슬롯의 아이템을 삭제하고
        inventory.Remove(slotNum);
        // 그 슬롯에 강화된 아이템을 추가
        inventory.Add(itemData.nextItemData);
    }

    public void Init()
    {
        itemSlotUI.RemoveItem();
        itemData = null;
        summaryText.text = "";
        price.text = "";
    }
}
