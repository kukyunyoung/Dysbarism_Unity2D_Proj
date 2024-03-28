using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField, Range(21,21)] int initalCapacity = 21;
    [SerializeField, Range(21,21)] int maxCapacity = 21;
    [SerializeField] InventoryManager inventoryManager;
    [SerializeField] Item[] items;
    [SerializeField] PlayerStatus playerStatus;
    [SerializeField] Sprite whatImg;

    readonly HashSet<int> indexSetForUpdate = new HashSet<int>();
    readonly static Dictionary<Type, int> _sortWeightDict = new Dictionary<Type, int>
        {
            { typeof(PortionItemData), 10000 },
            { typeof(WeaponItemData),  20000 },
            { typeof(AccessaryItemData),   30000 },
        };

    private class ItemComparer : IComparer<Item>
    {
        public int Compare(Item a, Item b)
        {
            return (a.Data.ID + _sortWeightDict[a.Data.GetType()])
                 - (b.Data.ID + _sortWeightDict[b.Data.GetType()]);
        }
    }
    private static readonly ItemComparer itemComparer = new ItemComparer();

    public int Capacity { get; private set; }

    private void Awake()
    {
        items = new Item[maxCapacity];
        Capacity = initalCapacity;
        inventoryManager.SetInventoryReference(this);
    }

    void Start()
    {
        UpdateAccessibleStatesAll();
    }

    bool IsValidIndex(int index)
    {
        return index >= 0 && index < Capacity;
    }

    /// <summary> 해당 슬롯이 셀 수 있는 아이템인지 여부 </summary>
    public bool IsCountableItem(int index)
    {
        return HasItem(index) && items[index] is CountableItem;
    }

    public int GetCurrentAmount(int index)
    {
        if (!IsValidIndex(index)) return -1;
        if (items[index] == null) return 0;

        CountableItem ci = items[index] as CountableItem;
        if (ci == null)
            return 1;

        return ci.Amount;
    }

    public void SeparateAmount(int indexA, int indexB, int amount)
    {
        // amount : 나눌 목표 수량

        if (!IsValidIndex(indexA)) return;
        if (!IsValidIndex(indexB)) return;

        Item itemA = items[indexA];
        Item itemB = items[indexB];

        CountableItem ciA = itemA as CountableItem;

        // 조건 : A 슬롯 - 셀 수 있는 아이템 / B 슬롯 - Null
        // 조건에 맞는 경우, 복제하여 슬롯 B에 추가
        if (ciA != null && itemB == null)
        {
            items[indexB] = ciA.SeperateAndClone(amount);

            UpdateSlot(indexA, indexB);
        }
    }

    /// <summary> 해당하는 인덱스의 슬롯 상태 및 UI 갱신 </summary>
    private void UpdateSlot(int index)
    {
        if (!IsValidIndex(index)) return;

        Item item = items[index];

        // 1. 아이템이 슬롯에 존재하는 경우
        if (item != null)
        {
            // 아이콘 등록
            inventoryManager.SetItemIcon(index, item.Data.IconSprite);

            // 1-1. 셀 수 있는 아이템
            if (item is CountableItem ci)
            {
                // 1-1-1. 수량이 0인 경우, 아이템 제거
                if (ci.IsEmpty)
                {
                    items[index] = null;
                    RemoveIcon();
                    return;
                }
                // 1-1-2. 수량 텍스트 표시
                else
                {
                    inventoryManager.SetItemAmountText(index, ci.Amount);
                }
            }
            // 1-2. 셀 수 없는 아이템인 경우 수량 텍스트 제거
            else
            {
                inventoryManager.HideItemAmountText(index);
            }

            // 슬롯 필터 상태 업데이트
            inventoryManager.UpdateSlotFilterState(index, item.Data);
        }
        // 2. 빈 슬롯인 경우 : 아이콘 제거
        else
        {
            RemoveIcon();
        }

        // 로컬 : 아이콘 제거하기
        void RemoveIcon()
        {
            inventoryManager.RemoveItem(index);
            inventoryManager.HideItemAmountText(index); // 수량 텍스트 숨기기
        }
    }

    public void PutItemList(List<ItemData> items)
    {
        for(int i=0;i<items.Count; i++)
        {
            this.items[i] = null;
            if(items[i] == null) continue;
            this.items[i] = items[i].CreateItem();
        }
        UpdateAllSlot();

        if(inventoryManager.weaponGo1 != null) Destroy(inventoryManager.weaponGo1);
        if(inventoryManager.weaponGo2 != null) Destroy(inventoryManager.weaponGo2);

        if (inventoryManager.slotUIList[15].hasItem) inventoryManager.InstWeaponGo1(inventoryManager.slotUIList[15]);
        if (inventoryManager.slotUIList[16].hasItem) inventoryManager.InstWeaponGo2(inventoryManager.slotUIList[16]);

        if(inventoryManager.weaponGo1 != null) 
        {
            inventoryManager.weaponGo1.SetActive(true); 
            if(inventoryManager.weaponGo2 != null) inventoryManager.weaponGo2.SetActive(false);
        }
        else 
        {
            if (inventoryManager.weaponGo2 != null) inventoryManager.weaponGo2.SetActive(true);
        }
        inventoryManager.SetWeaponGoQuickSlot();

        UIManager.instance.statusPanel.SetActive(!UIManager.instance.statusPanel.activeSelf);
        UIManager.instance.statusPanel.SetActive(!UIManager.instance.statusPanel.activeSelf);
    }

    /// <summary> 해당하는 인덱스의 슬롯들의 상태 및 UI 갱신 </summary>
    private void UpdateSlot(params int[] indices)
    {
        foreach (var i in indices)
        {
            UpdateSlot(i);
        }
    }

    private void UpdateAllSlot()
    {
        for (int i = 0; i < Capacity; i++)
        {
            UpdateSlot(i);
        }
    }

    public void TrimAll()
    {
        // 가장 빠른 배열 빈공간 채우기 알고리즘

        // i 커서와 j 커서
        // i 커서 : 가장 앞에 있는 빈칸을 찾는 커서
        // j 커서 : i 커서 위치에서부터 뒤로 이동하며 기존재 아이템을 찾는 커서

        // i커서가 빈칸을 찾으면 j 커서는 i+1 위치부터 탐색
        // j커서가 아이템을 찾으면 아이템을 옮기고, i 커서는 i+1 위치로 이동
        // j커서가 Capacity에 도달하면 루프 즉시 종료

        indexSetForUpdate.Clear();
        int i = -1;
        while (items[++i] != null)
        {
            if (i >= 14) break;
        }
        int j = i;

        while (true)
        {
            while (++j < 15 && items[j] == null) ;

            if (j == 15)
                break;

            indexSetForUpdate.Add(i);
            indexSetForUpdate.Add(j);

            items[i] = items[j];
            items[j] = null;
            i++;
        }

        foreach (var index in indexSetForUpdate)
        {
            UpdateSlot(index);
        }
        inventoryManager.UpdateAllSlotFilters();
    }

    public void SortAll()
    {
        // 1. Trim
        int i = -1;
        while (items[++i] != null) 
        {
            if(i>=14) break;
        }
        int j = i;

        while (true)
        {
            while (++j < 15 && items[j] == null) ;

            if (j == 15)
                break;

            items[i] = items[j];
            items[j] = null;
            i++;
        }

        // 2. Sort
        Array.Sort(items, 0, i, itemComparer);

        // 3. Update
        UpdateAllSlot();
        inventoryManager.UpdateAllSlotFilters(); // 필터 상태 업데이트
    }

    public ItemData GetItemData(int index)
    {
        if (!IsValidIndex(index)) return null;
        if (items[index] == null) return null;

        return items[index].Data;
    }

    public Item GetItem(int index)
    {
        if (!IsValidIndex(index)) return null;
        if (items[index] == null) return null;

        return items[index];
    }

    /// <summary> 해당 슬롯의 아이템 이름 리턴 </summary>
    public string GetItemName(int index)
    {
        if (!IsValidIndex(index)) return "";
        if (items[index] == null) return "";

        return items[index].Data.Name;
    }

    // 앞에서부터 비어있는 슬롯 인덱스 탐색
    public int FindEmptySlotIndex(int startIndex = 0)
    {
        for (int i = startIndex; i < Capacity; i++)
        {
            if (items[i] == null) return i;
        }
        return -1;
    }

    // 모든 슬롯 UI에 접근가능 여부 업데이트
    public void UpdateAccessibleStatesAll() 
    {
        inventoryManager.SetAccessibleSlotRange(Capacity);
    }


    public bool HasItem(int index)
    {
        return IsValidIndex(index) && items[index] != null;
    }

    public int GetCurrentItem(int index)
    {
        if(!IsValidIndex(index)) return -1;
        if (items[index] == null) return 0;

        Item item = items[index] as Item;
        if (item == null) return 1;

        return 0;
    }

    /// <summary> 앞에서부터 개수 여유가 있는 Countable 아이템의 슬롯 인덱스 탐색 </summary>
    private int FindCountableItemSlotIndex(CountableItemData target, int startIndex = 0)
    {
        for (int i = startIndex; i < Capacity; i++)
        {
            var current = items[i];
            if (current == null)
                continue;

            // 아이템 종류 일치, 개수 여유 확인
            if (current.Data == target && current is CountableItem ci)
            {
                if (!ci.IsMax)
                    return i;
            }
        }
        return -1;
    }

    public void Swap(int indexA, int indexB)
    {
        if(!IsValidIndex(indexA)) return;
        if(!IsValidIndex(indexB)) return;

        Item itemA = items[indexA];
        Item itemB = items[indexB];

        if(itemA != null && itemB != null && itemA.Data == itemB.Data &&
           itemA is CountableItem ciA && itemB is CountableItem ciB)
        {
            int maxAmount = ciB.MaxAmount;
            int sum = ciA.Amount + ciB.Amount;

            if(sum <= maxAmount)
            {
                ciA.SetAmount(0);
                ciB.SetAmount(sum);
            }
            else
            {
                ciA.SetAmount(sum - maxAmount);
                ciB.SetAmount(maxAmount);
            }
        }
        else
        {
            items[indexA] = itemB;
            items[indexB] = itemA;
        }

        UpdateSlot(indexA);
        UpdateSlot(indexB);
    }

    public void ConnectUI(InventoryManager inventoryUI)
    {
        inventoryManager = inventoryUI;
        inventoryManager.SetInventoryReference(this);
    }

    public int Add(ItemData itemData, int amount =1)
    {
        int index;

        // 수량이 있는 아이템
        if(itemData is CountableItemData ciData)
        {
            bool findNextCountable = true;
            index = -1;

            while( amount > 0)
            {
                // 이미 해당 아이템이 인벤에 존재하고, 개수 여유가 있는지
                if(findNextCountable)
                {
                    index = FindCountableItemSlotIndex(ciData, index +1);

                    // 개수 여유가 있는 기존재 슬롯이 더이상 없으면 빈슬롯 탐색
                    if(index == -1) findNextCountable = false;
                    // 여유있는 기존재 슬롯을 찾으면 양 증가시키고 초과량 존재시 amount에 초기화
                    else
                    {
                        CountableItem ci = items[index] as CountableItem;
                        amount = ci.AddAmountAndGetExcess(amount);

                        UpdateSlot(index);
                    }
                }
                // 빈슬롯 탐색
                else
                {
                    index = FindEmptySlotIndex(index + 1);

                    // 빈 슬롯 조차 없는경우 종료
                    if(index == -1 || index >= 15) { UIManager.instance.SetSystemMessage("인벤토리에 빈자리가 없습니다!"); break; }
                    else
                    {
                        CountableItem ci = ciData.CreateItem() as CountableItem;
                        ci.SetAmount(amount);

                        items[index] = ci;

                        amount = (amount > ciData.MaxAmount) ? (amount - ciData.MaxAmount) : 0;

                        UpdateSlot(index);
                    }
                }
            }
        }
        // 수량이 없는 아이템
        else
        {
            if(amount == 1)
            {
                index = FindEmptySlotIndex();
                if(index != -1 && index < 15)
                {
                    items[index] = itemData.CreateItem();
                    amount = 0;

                    UpdateSlot(index);
                }
                else UIManager.instance.SetSystemMessage("인벤토리에 빈자리가 없습니다!");
            }

            // 2개 이상의 수량없는 아이템을 동시에 추가하는 경우
            index = -1;
            for(; amount > 0; amount--)
            {
                // 아이템 넣은 인덱스의 다음 인덱스부터 슬롯 탐색
                index = FindEmptySlotIndex(index +1);

                if(index == -1 || index >= 15) { UIManager.instance.SetSystemMessage("인벤토리에 빈자리가 없습니다!"); break; }

                items[index] = itemData.CreateItem();
                
                UpdateSlot(index);
            }
        }

        return amount;
    }

    public void Remove(int index)
    {
        if(!IsValidIndex(index)) return;

        items[index] = null;
        UpdateSlot(index);
    }

    public void RemoveAll()
    {
        for(int i=0; i<Capacity; i++)
        {
            if(!IsValidIndex(i)) continue;
            items[i] = null;
        }
        UpdateAllSlot();
    }

    // 해당 슬롯의 아이템 사용
    public void Use(int index, ItemSlotUI from = null)
    {
        if(!IsValidIndex(index)) return;
        if(items[index] == null) return;

        int emptyIndex = 0;

        if(items[index] is IUseableItem useableItem) // 인벤토리의 포션아이템 우클릭
        {
            bool succeeded = useableItem.Use();

            if(succeeded) UpdateSlot(index);
        }
        else if (items[index] is IWeaponItem weaponItem) // 장비아이템 우클릭
        {
            WeaponItem itemData = weaponItem.Equip();
            

            if(index < 15) // 인벤토리슬롯을 클릭했을때
            {
                //if(items[15] == null) Swap(index, 15);
                //else if(items[16] == null) Swap(index, 16);
                //else Swap(index, 15);

                if(items[15] == null)
                {
                    inventoryManager.InstWeaponGo1(from);
                    inventoryManager.weaponGo1.SetActive(true);
                    if(inventoryManager.weaponGo2 != null) inventoryManager.weaponGo2.SetActive(false);
                    inventoryManager.SetWeaponGoQuickSlot();
                    //swap or move icon
                    Swap(index,15);
                }
                else if(items[16] == null) 
                {
                    inventoryManager.InstWeaponGo2(from);
                    inventoryManager.weaponGo2.SetActive(false);
                    inventoryManager.SetWeaponGoQuickSlot();
                    Swap(index, 16);
                }
                else
                {
                    Destroy(inventoryManager.weaponGo1);
                    inventoryManager.weaponGo2.SetActive(false);
                    inventoryManager.InstWeaponGo1(from);
                    inventoryManager.SetWeaponGoQuickSlot();
                    Swap(index,15);
                }
            }
            else // 장비슬롯을 클릭했을때
            {
                emptyIndex = FindEmptySlotIndex(0);

                if(emptyIndex != -1 && emptyIndex<15)
                {
                    if(index == 15)
                    { 
                        Destroy(inventoryManager.weaponGo1);
                        inventoryManager.weaponGo1 = null;
                        if(inventoryManager.weaponGo2 != null) inventoryManager.weaponGo2.SetActive(true);
                        inventoryManager.SetWeaponGoQuickSlot();
                    }
                    else if(index == 16) 
                    {
                        Destroy(inventoryManager.weaponGo2);
                        inventoryManager.weaponGo2 = null;
                        if (inventoryManager.weaponGo1 != null) inventoryManager.weaponGo1.SetActive(true);
                    }
                    Swap(index,emptyIndex);
                }
                else
                {
                    UIManager.instance.SetSystemMessage("빈공간 없음 !");
                    return;
                }
            }
            if (itemData != null) UpdateSlot(index);
        }
        else if (items[index] is IAccItem accItem) // 악세사리 아이템 클릭
        {
            AccessaryItem accData = accItem.Equip();

            if (index < 15) // 인벤토리슬롯을 클릭했을때
            {
                emptyIndex = FindEmptySlotIndex(17);

                if(emptyIndex == -1) Swap(index, 17); // 악세슬롯 다 차있으면
                else Swap(index, emptyIndex);
            }
            else // 장비슬롯을 클릭했을때
            {
                emptyIndex = FindEmptySlotIndex(0);

                if (emptyIndex != -1 && emptyIndex < 15)
                {
                    Swap(index, emptyIndex);
                }
                else
                {
                    UIManager.instance.SetSystemMessage("빈공간 없음 !");
                    return;
                }
            }
            if (accData != null) UpdateSlot(index);
        }

        UpdatePlayerStatus();
        for (int i = 0; i < 6; i++)
        {
            if(GetItemData(i+15) != null) UIManager.instance.equipments[i].sprite = GetItemData(i + 15).IconSprite;
            else UIManager.instance.equipments[i].sprite = whatImg;
        }
    }

    public void UpdatePlayerStatus()
    {
        int accInven = 17; // 17~20까지 악세슬롯

        playerStatus.InitStatus();
        for(int i=accInven; i<accInven+4; i++)
        {
            if(items[i] == null) continue; 
            AccessaryItemData accData = (AccessaryItemData)GetItemData(i);

            playerStatus.playerStatus[0] += accData.Power;
            playerStatus.playerStatus[1] += accData.Armory;
            playerStatus.playerStatus[2] += accData.Critical;
            playerStatus.playerStatus[3] += accData.Avoidance;
            playerStatus.playerStatus[4] *= accData.SwSpeed;
            playerStatus.playerStatus[5] *= accData.ReloadSpeed;
            playerStatus.playerStatus[6] *= accData.ChargeSpeed;
        }
        playerStatus.playerStatus[0] += playerStatus.paca[0];
        playerStatus.playerStatus[1] += playerStatus.paca[1];
        playerStatus.playerStatus[2] += playerStatus.paca[2];
        playerStatus.playerStatus[3] += playerStatus.paca[3];
        playerStatus.playerStatus[4] *= playerStatus.src[0];
        playerStatus.playerStatus[5] *= playerStatus.src[1];
        playerStatus.playerStatus[6] *= playerStatus.src[2];

        UIManager.instance.statusPanel.SetActive(!UIManager.instance.statusPanel.activeSelf);
        UIManager.instance.statusPanel.SetActive(!UIManager.instance.statusPanel.activeSelf);
    }
}