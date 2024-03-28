using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TooltipUI : MonoBehaviour
{
    [SerializeField] Text titleText;
    [SerializeField] Text contentText;

    RectTransform rt;
    CanvasScaler cs;

    private void Awake() 
    {
        Init();
        Hide();
    }

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);

    void Init()
    {
        TryGetComponent(out rt);
        rt.pivot = new Vector2(0, 1);
        cs = GetComponentInParent<CanvasScaler>();

        DisableAllChildrenRaycastTarget(transform);
    }

    // 모든 자식 UI에 레이캐스트 타겟 해제
    void DisableAllChildrenRaycastTarget(Transform tr)
    {
        // 본인이 UI를 상속하면 레이캐스트 해제
        tr.TryGetComponent(out Graphic gr);
        if(gr != null) gr.raycastTarget = false;

        // 자식이 없으면 종료
        int childCount = tr.childCount;
        if(childCount == 0) return;

        for(int i=0; i<childCount ; i++)
        {
            DisableAllChildrenRaycastTarget(tr.GetChild(i));
        }
    }

    // 툴팁 UI에 아이템 정보 등록
    public void SetItemInfo(ItemData data)
    {
        if(data == null) return;
        
        titleText.text = data.Name;
        contentText.text = data.Tooltip;
    }

    public void SetRectPosition(RectTransform slotRect)
    {
        // 캔버스 스케일러에 따른 해상도 대응
        float wRatio = Screen.width / cs.referenceResolution.x;
        float hRatio = Screen.height / cs.referenceResolution.y;
        float ratio = wRatio * (1 - cs.matchWidthOrHeight) + hRatio * cs.matchWidthOrHeight;

        float slotWidth = slotRect.rect.width * ratio;
        float slotHeight = slotRect.rect.height * ratio;

        // 툴팁 초기 위치 (슬롯 우하단) 설정
        rt.position = slotRect.position + new Vector3(slotWidth, -slotHeight);
        Vector2 pos = rt.position;

        // 툴팁의 크기
        float width = rt.rect.width * ratio;
        float height = rt.rect.height * ratio;

        // 우측, 하단이 잘렸는지 여부
        bool rightTruncated = pos.x + width > Screen.width;
        bool bottomTruncated = pos.y - height < 0;

        ref bool R = ref rightTruncated;
        ref bool B = ref bottomTruncated;

        if(R && !B) rt.position = new Vector2(pos.x - width - slotWidth, pos.y);
        else if(!R && B) rt.position = new Vector2(pos.x, pos.y + height + slotHeight);
        else if(R && B) rt.position = new Vector2 (pos.x - width - slotWidth, pos.y + height + slotHeight);
    }
}
