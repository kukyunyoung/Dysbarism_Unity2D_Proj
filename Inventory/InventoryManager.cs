using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryManager : GraphicRaycaster
{
    [Header("Options")]
    [Range(0, 10)]
    [SerializeField] private int horizontalSlotCount = 3;  // 슬롯 가로 개수
    [Range(0, 10)]
    [SerializeField] private int verticalSlotCount = 5;      // 슬롯 세로 개수
    [SerializeField] private float slotMargin = 25;          // 한 슬롯의 상하좌우 여백
    [SerializeField] private float contentAreaPadding = 20f; // 인벤토리 영역의 내부 여백
    [Range(110,110)]
    [SerializeField] private float slotSize = 110f;      // 각 슬롯의 크기

    [Space]
    [SerializeField] private bool showTooltip = true;
    [SerializeField] private bool showHighlight = true;
    [SerializeField] private bool showRemovingPopup = true;

    [Header("Connected Objects")]
    [SerializeField] private RectTransform contentAreaRT; // 슬롯들이 위치할 영역
    [SerializeField] private GameObject slotUiPrefab;     // 슬롯의 원본 프리팹

    [Header("Buttons")]
    [SerializeField] private Button trimButton;
    [SerializeField] private Button sortButton;

    [Header("Filter Toggles")]
    [SerializeField] private Toggle toggleFilterAll;
    [SerializeField] private Toggle toggleFilterEquipments;
    [SerializeField] private Toggle toggleFilterPortions;

    [SerializeField] TooltipUI itemTooltip;
    [SerializeField] InvenPopUpUI popup;
    [Space]
    [SerializeField] ItemSlotUI[] weaponSlot;
    [SerializeField] ItemSlotUI[] accSlot;
    [Space]
    [SerializeField] public WeaponUpgrade weaponUpgrade;

    Inventory inventory;

    GraphicRaycaster gr;
    PointerEventData ped;
    List<RaycastResult> rrList;

    ItemSlotUI beginDragSlot; // 현재 드래그를 시작한 슬롯
    ItemSlotUI pointerOverSlot;
    Transform beginDragIconTr;

    Vector3 beginDragIconPoint; // 드래그 시작시 슬롯의 위치
    Vector3 beginDragCursorPoint; // 드래그 시작시 커서의 위치
    int beginDragSlotSiblingIndex;

    public List<ItemSlotUI> slotUIList = new List<ItemSlotUI>();

    public GameObject weaponGo1;
    public GameObject weaponGo2;

    /// <summary> 인벤토리 UI 내 아이템 필터링 옵션 </summary>
    private enum FilterOption
    {
        All, Equipment, Portion
    }
    private FilterOption currentFilterOption = FilterOption.All;
    /***********************************************************************
    *                               Unity Events
    ***********************************************************************/
#pragma warning disable CS0114 // 멤버가 상속된 멤버를 숨깁니다. override 키워드가 없습니다.
    private void Awake()
#pragma warning restore CS0114 // 멤버가 상속된 멤버를 숨깁니다. override 키워드가 없습니다.
    {
        Init();
        InitSlots();
        InitButtonEvents();
        InitToggleEvents();
    }

    void Update()
    {
        ped.position = Input.mousePosition;

        OnPointerEnterAndExit();
        ShowOrHideItemTooltip();

        OnPointerDown();
        OnDrag();
        OnPointerUp();
    }

    /***********************************************************************
    *                               Init Methods
    ***********************************************************************/

    void Init()
    {
        TryGetComponent(out gr);
        if(gr == null) gr = gameObject.AddComponent<GraphicRaycaster>();

        ped = new PointerEventData(EventSystem.current);
        rrList = new List<RaycastResult>(10);

        if(itemTooltip == null)
        {
            itemTooltip = transform.parent.parent.parent.parent.GetComponentInChildren<TooltipUI>();
        }
    }

    private void InitSlots()
    {
        // 슬롯 프리팹 설정
        slotUiPrefab.TryGetComponent(out RectTransform slotRect);
        slotRect.sizeDelta = new Vector2(slotSize, slotSize);

        slotUiPrefab.TryGetComponent(out ItemSlotUI itemSlot);
        if (itemSlot == null)
            slotUiPrefab.AddComponent<ItemSlotUI>();

        slotUiPrefab.SetActive(false);

        // --
        Vector2 beginPos = new Vector2(contentAreaPadding, -contentAreaPadding);
        print(beginPos);
        Vector2 curPos = beginPos;

        slotUIList = new List<ItemSlotUI>(verticalSlotCount * horizontalSlotCount);

        // 슬롯들 동적 생성
        for (int j = 0; j < verticalSlotCount; j++)
        {
            for (int i = 0; i < horizontalSlotCount; i++)
            {
                int slotIndex = (horizontalSlotCount * j) + i;

                var slotRT = CloneSlot();
                slotRT.pivot = new Vector2(0f, 1f); // Left Top
                slotRT.anchoredPosition = curPos;
                slotRT.gameObject.SetActive(true);
                slotRT.gameObject.name = $"Item Slot [{slotIndex}]";

                var slotUI = slotRT.GetComponent<ItemSlotUI>();
                slotUI.SetSlotIndex(slotIndex);
                slotUI.slotState = ItemSlotUI.SlotState.inven;
                slotUIList.Add(slotUI);

                // Next X
                curPos.x += (slotMargin + slotSize);
            }

            // Next Line
            curPos.x = beginPos.x;
            curPos.y -= (slotMargin + slotSize);
        }

        // 슬롯 프리팹 - 프리팹이 아닌 경우 파괴
        if (slotUiPrefab.scene.rootCount != 0)
            Destroy(slotUiPrefab);

        weaponSlot[0].SetSlotIndex(15);
        weaponSlot[0].slotState = ItemSlotUI.SlotState.weapon;
        slotUIList.Add(weaponSlot[0]); // 15
        weaponSlot[1].SetSlotIndex(16);
        weaponSlot[1].slotState = ItemSlotUI.SlotState.weapon;
        slotUIList.Add(weaponSlot[1]); // 16

        accSlot[0].SetSlotIndex(17);
        accSlot[0].slotState = ItemSlotUI.SlotState.accessary;
        slotUIList.Add(accSlot[0]); // 17
        accSlot[1].SetSlotIndex(18);
        accSlot[1].slotState = ItemSlotUI.SlotState.accessary;
        slotUIList.Add(accSlot[1]); // 18
        accSlot[2].SetSlotIndex(19);
        accSlot[2].slotState = ItemSlotUI.SlotState.accessary;
        slotUIList.Add(accSlot[2]); // 19
        accSlot[3].SetSlotIndex(20);
        accSlot[3].slotState = ItemSlotUI.SlotState.accessary;
        slotUIList.Add(accSlot[3]); // 20

        // -- Local Method --
        RectTransform CloneSlot()
        {
            GameObject slotGo = Instantiate(slotUiPrefab);
            RectTransform rt = slotGo.GetComponent<RectTransform>();
            rt.SetParent(contentAreaRT);

            return rt;
        }
    }

    private void InitButtonEvents()
    {
        trimButton.onClick.AddListener(() => inventory.TrimAll());
        sortButton.onClick.AddListener(() => inventory.SortAll());
    }

    private void InitToggleEvents()
    {
        toggleFilterAll.onValueChanged.AddListener(flag => UpdateFilter(flag, FilterOption.All));
        toggleFilterEquipments.onValueChanged.AddListener(flag => UpdateFilter(flag, FilterOption.Equipment));
        toggleFilterPortions.onValueChanged.AddListener(flag => UpdateFilter(flag, FilterOption.Portion));

        // Local Method
        void UpdateFilter(bool flag, FilterOption option)
        {
            if (flag)
            {
                currentFilterOption = option;
                //UpdateAllSlotFilters();
            }
        }
    }

    /***********************************************************************
    *                               End
    ***********************************************************************/

    void OnPointerEnterAndExit()
    {
        var prevSlot = pointerOverSlot;

        var curSlot = pointerOverSlot = RaycastAndGetFirstComponent<ItemSlotUI>();

        if(prevSlot == null)
        {
            if(curSlot != null) OnCurrentEnter();
        }
        else
        {
            if(curSlot == null) OnPrevExit();
            else if(prevSlot != curSlot)
            {
                OnPrevExit();
                OnCurrentEnter();
            }
        }

        void OnCurrentEnter() {curSlot.Highlight(true);}
        void OnPrevExit() {prevSlot.Highlight(false);}
    }

    T RaycastAndGetFirstComponent<T>() where T : Component
    {
        rrList.Clear();
        gr.Raycast(ped, rrList);

        if (rrList.Count == 0) return null;

        return rrList[0].gameObject.GetComponent<T>();
    }

    public void OnPointerDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(weaponUpgrade!=null && weaponUpgrade.gameObject.activeSelf)
            {
                ItemSlotUI slot = RaycastAndGetFirstComponent<ItemSlotUI>();

                if (slot != null && slot.hasItem && slot.isAccessible)
                {
                    if (inventory.GetItemData(slot.index) is WeaponItemData) // 클릭한게 장비아이템이면
                    {
                        weaponUpgrade.itemData = (WeaponItemData)inventory.GetItemData(slot.index);
                        weaponUpgrade.slotNum = slot.index;
                    }
                    else return;
                }
                print(weaponUpgrade.itemData);
                weaponUpgrade.SetIcon();

                beginDragSlot = null;
                slot=null;
                return;
            }

            beginDragSlot = RaycastAndGetFirstComponent<ItemSlotUI>();

            // 아이템을 갖고 있는 슬롯만 해당
            if (beginDragSlot != null && beginDragSlot.hasItem && beginDragSlot.isAccessible)
            {
                // 위치 기억, 참조 등록
                beginDragIconTr = beginDragSlot.IconRect.transform;
                beginDragIconPoint = beginDragIconTr.position;
                beginDragCursorPoint = Input.mousePosition;

                // 맨위에 보이기
                beginDragSlotSiblingIndex = beginDragSlot.transform.GetSiblingIndex();
                beginDragSlot.transform.SetAsLastSibling();

                beginDragSlot.SetHighlightOnTop(false);
            }
            else beginDragSlot = null;
        }
        if(Input.GetMouseButtonDown(1))
        {
            ItemSlotUI slot = RaycastAndGetFirstComponent<ItemSlotUI>();
            
            if(slot != null && slot.hasItem && slot.isAccessible)
            {
                if(inventory.GetItemData(slot.index) is PortionItemData) // 클릭한게 포션 아이템이면
                    TryUseItem(slot.index);
                else // 클릭한게 장비나 악세면
                {
                    TryUseItem(slot.index, slot);
                }
            }
            
            beginDragSlot = null;
        }
    }

    public void OnDrag()
    {
        if (beginDragSlot == null) return;

        if (Input.GetMouseButton(0))
        {
            beginDragIconTr.position =
                beginDragIconPoint + (Input.mousePosition - beginDragCursorPoint);
        }
    }

    /// <summary> 접근 가능한 슬롯 범위 설정 </summary>
    public void SetAccessibleSlotRange(int accessibleSlotCount)
    {
        for (int i = 0; i < slotUIList.Count; i++)
        {
            slotUIList[i].SetSlotAccessibleState(i < accessibleSlotCount);
        }
    }

    // Ŭ���� �� ���
    public void OnPointerUp()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if(beginDragSlot != null)
            {
                beginDragIconTr.position = beginDragIconPoint;
                // UI ���� ����
                beginDragSlot.transform.SetSiblingIndex(beginDragSlotSiblingIndex);

                EndDrag();

                beginDragSlot = null;
                beginDragIconTr = null;
            }
        }
    }

    void EndDrag()
    {
        ItemSlotUI endDragSlot = RaycastAndGetFirstComponent<ItemSlotUI>();

        // 아이템 슬롯끼리 아이콘 교환 또는 이동
        if(endDragSlot != null && endDragSlot.isAccessible) 
        {
            // 수량 나누기 조건
            // 1) 마우스 클릭 떼는 순간 좌측 컨트롤 또는 쉬프트키 유지
            // 2) begin : 셀 수 있는 아이템 / end : 비어있는 슬롯
            // 3) begin 아이템의 수량 > 1
            bool isSeparatable =
                (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftShift)) &&
                (inventory.IsCountableItem(beginDragSlot.index) && !inventory.HasItem(endDragSlot.index));

            // true : 수량 나누기 , false : 교환 또는 이동
            bool isSeparation = false;
            int currentAmount = 0;

            if(isSeparatable)
            {
                currentAmount = inventory.GetCurrentAmount(beginDragSlot.index);
                if(currentAmount > 1) isSeparation = true;
            }

            if(isSeparation) TrySeparateAmount(beginDragSlot.index, endDragSlot.index, currentAmount);
            else TrySwapItems(beginDragSlot, endDragSlot);

            UpdateTooltipUI(endDragSlot);
            return;
        }

        if(!IsOverUI()) 
        {
            int index = beginDragSlot.index;
            string itemName = inventory.GetItemName(index);
            int amount = inventory.GetCurrentAmount(index);

            if(amount > 1) itemName += $"x{amount}";

            popup.OpenConfirmationPopup(() => TryRemoveItem(index), itemName);
        }
    }

    void TryRemoveItem(int index)
    {
        inventory.Remove(index);
    }

    void TrySwapItems(ItemSlotUI from, ItemSlotUI to)
    {
        if(from == to) return;
        if((from.slotState==ItemSlotUI.SlotState.weapon && to.slotState==ItemSlotUI.SlotState.accessary) ||
           (from.slotState==ItemSlotUI.SlotState.accessary && to.slotState==ItemSlotUI.SlotState.weapon)) return; // 장비 <-> 악세 슬롯 스왑 금지
        if(inventory.GetItemData(from.index) is WeaponItemData && to.slotState == ItemSlotUI.SlotState.accessary ||
          (inventory.GetItemData(from.index) is AccessaryItemData && to.slotState == ItemSlotUI.SlotState.weapon)) return;   

        if(to.slotState == ItemSlotUI.SlotState.weapon) // 장비 교체 했을때
        {
            if(!to.hasItem) // 슬롯에 아이템이 없을때 (등록)
            {
                if(to.index == 15)
                {
                    InstWeaponGo1(from);  
                    if(from.index == 16) Destroy(weaponGo2);
                    if(weaponGo2 != null) weaponGo2.SetActive(false);
                }
                else if(to.index == 16) 
                {
                    InstWeaponGo2(from);
                    if(from.index == 15) Destroy(weaponGo1);
                    weaponGo2.SetActive(false);
                    if(weaponGo1 != null) weaponGo1.SetActive(true);
                    else weaponGo2.SetActive(true);
                }
            }
            else// 슬롯에 아이템이 있을때 (교체)
            {
                if (to.index == 15) 
                {
                    if(from.slotState == ItemSlotUI.SlotState.weapon) // 장비칸끼리 교체
                    {
                        if(weaponGo1 == null)
                        {
                            InstWeaponGo1(from);
                            Destroy(weaponGo2);
                            weaponGo1.SetActive(true);
                        }
                        else
                        {
                            Destroy(weaponGo1);
                            Destroy(weaponGo2);
                            InstWeaponGo1(from);
                            InstWeaponGo2(to);
                            weaponGo1.SetActive(true);
                            weaponGo2.SetActive(false);
                        }
                    }
                    else // 인벤과 장비칸 교체
                    {
                        Destroy(weaponGo1);
                        InstWeaponGo1(from);
                        if(weaponGo2 != null) weaponGo2.SetActive(false);
                    }
                }
                else if (to.index == 16)
                {
                    if (from.slotState == ItemSlotUI.SlotState.weapon) // 장비칸끼리 교체
                    {
                        if(weaponGo2 == null)
                        {
                            InstWeaponGo2(from);
                            Destroy(weaponGo1);
                            weaponGo2.SetActive(true);
                        }
                        else
                        {
                            Destroy(weaponGo1);
                            Destroy(weaponGo2);
                            InstWeaponGo2(from);
                            InstWeaponGo1(to);
                            weaponGo1.SetActive(true);
                            weaponGo2.SetActive(false);
                        }
                    }
                    else
                    {
                        Destroy(weaponGo2);
                        InstWeaponGo2(from);
                        if(weaponGo1 != null) weaponGo1.SetActive(false);
                    }
                }
            }
            SetWeaponGoQuickSlot();
        }
        else if((to.slotState == ItemSlotUI.SlotState.inven && from.slotState == ItemSlotUI.SlotState.weapon) ||
                (to.slotState == ItemSlotUI.SlotState.inven && from.slotState == ItemSlotUI.SlotState.accessary))
                //장비슬롯에서 인벤으로 드래그했을때
        {
            if(!to.hasItem)
            {
                if(from.index == 15) Destroy(weaponGo1); if(weaponGo2!=null) weaponGo2.SetActive(true);
                else if(from.index == 16) Destroy(weaponGo2); if(weaponGo1!=null) weaponGo1.SetActive(true);
            }
            else // 
            {
                if(inventory.GetItemData(to.index) is not WeaponItemData) return;

                if (from.index == 15) 
                {
                    Destroy(weaponGo1); 
                    InstWeaponGo1(to);
                    weaponGo1.SetActive(true);
                    if(weaponGo2!=null) weaponGo2.SetActive(false);
                }
                else if (from.index == 16) 
                {
                    Destroy(weaponGo2); 
                    InstWeaponGo2(to);
                    weaponGo2.SetActive(false);
                    if (weaponGo1 != null) weaponGo1.SetActive(true);
                    else weaponGo2.SetActive(true);
                }
            }
        }
        from.SwapOrMoveIcon(to);
        inventory.Swap(from.index, to.index);
        if((to.slotState == ItemSlotUI.SlotState.inven && from.slotState == ItemSlotUI.SlotState.accessary) ||
            (to.slotState == ItemSlotUI.SlotState.accessary && from.slotState == ItemSlotUI.SlotState.inven))
            inventory.UpdatePlayerStatus();
    }

    public void InstWeaponGo1(ItemSlotUI from)
    {
        weaponGo1 = Instantiate(inventory.GetItemData(from.index).instItemPrefab); // 첫번째 슬롯에 들어갔을때
        weaponGo1.transform.parent = PlayerMove.instance.gameObject.transform.GetChild(1);
        weaponGo1.transform.localPosition = Vector3.zero;
    }
    public void InstWeaponGo2(ItemSlotUI from)
    {
        weaponGo2 = Instantiate(inventory.GetItemData(from.index).instItemPrefab); // 첫번째 슬롯에 들어갔을때
        weaponGo2.transform.parent = PlayerMove.instance.gameObject.transform.GetChild(1);
        weaponGo2.transform.localPosition = Vector3.zero;
    }
    public void SetWeaponGoQuickSlot()
    {
        if(weaponGo1 != null) 
        {
            weaponGo1.transform.parent.GetComponent<WeaponManager>().weapon.Clear();
            weaponGo1.transform.parent.GetComponent<WeaponManager>().weapon.Add(weaponGo1);
            weaponGo1.transform.parent.GetComponent<WeaponManager>().weapon.Add(weaponGo2);
        }
        else
        {
            if(weaponGo2 == null) return;
            weaponGo2.transform.parent.GetComponent<WeaponManager>().weapon.Clear();
            weaponGo2.transform.parent.GetComponent<WeaponManager>().weapon.Add(weaponGo1);
            weaponGo2.transform.parent.GetComponent<WeaponManager>().weapon.Add(weaponGo2);
        }
    }

    void TrySeparateAmount(int indexA, int indexB, int amount)
    {
        if(indexA == indexB) return;

        string itemName = inventory.GetItemName(indexA);

        popup.OpenAmountInputPopup(
            amt => inventory.SeparateAmount(indexA, indexB, amt), amount, itemName
        );
    }

    public void SetItemIcon(int index, Sprite icon){
        slotUIList[index].SetItem(icon);
    }

    public void SetItemAmountText(int index, int amount){
        slotUIList[index].SetItemAmount(amount);
    }

    public void HideItemAmountText(int index){
        slotUIList[index].SetItemAmount(1);
    }

    public void RemoveItem(int index)
    {
        slotUIList[index].RemoveItem();
    }

    bool IsOverUI() => EventSystem.current.IsPointerOverGameObject();

    void ShowOrHideItemTooltip()
    {
        bool isValid = 
            pointerOverSlot != null && pointerOverSlot.hasItem && pointerOverSlot.isAccessible
            && (pointerOverSlot != beginDragSlot);

        if(isValid)
        {
            itemTooltip.gameObject.SetActive(true);
            UpdateTooltipUI(pointerOverSlot);
        }
        else itemTooltip.Hide();
    }

    void UpdateTooltipUI(ItemSlotUI slot)
    {
        print(inventory);   /// ****** null ****** //
        itemTooltip.SetItemInfo(inventory.GetItemData(slot.index));
        itemTooltip.SetRectPosition(slot.SlotRect);
    }

    /// <summary> 특정 슬롯의 필터 상태 업데이트 </summary>
    public void UpdateSlotFilterState(int index, ItemData itemData)
    {
        bool isFiltered = true;

        // null인 슬롯은 타입 검사 없이 필터 활성화
        if (itemData != null)
            switch (currentFilterOption)
            {
                case FilterOption.Equipment:
                    isFiltered = itemData is EquipmentItemData;
                    break;

                case FilterOption.Portion:
                    isFiltered = itemData is PortionItemData;
                    break;
            }

        slotUIList[index].SetItemAccessibleState(isFiltered);
    }

    public void UpdateAllSlotFilters()
    {
        int capacity = inventory.Capacity;

        for (int i = 0; i < capacity; i++)
        {
            ItemData data = inventory.GetItemData(i);
            UpdateSlotFilterState(i, data);
        }
    }

    public void SetInventoryReference(Inventory inventory)
    {
        this.inventory = inventory;
    }

    void TryUseItem(int index)
    {
        inventory.Use(index);
    }

    void TryUseItem(int index, ItemSlotUI slot)
    {
        inventory.Use(index, slot);
    }
}
