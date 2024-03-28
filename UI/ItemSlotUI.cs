using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] float padding = 1;
    [SerializeField] Image iconImage;
    [SerializeField] Text amountText;
    [SerializeField] Image highlightImage;

    [Space]
    [SerializeField] float highlightAlpha = 0.5f;
    [SerializeField] float highlightFadeDuration = 0.2f;

    public int index { get; private set; }
    public bool hasItem => iconImage.sprite != null;
    public bool isAccessible => isAccessibleSlot && isAccessibleItem;
    public RectTransform SlotRect => slotRect;
    public RectTransform IconRect => iconRect;

    RectTransform slotRect;
    RectTransform iconRect;
    RectTransform highlightRect;

    GameObject iconGo;
    GameObject textGo;
    GameObject highlightGo;
    Image slotImg;

    bool isAccessibleSlot = true; 
    bool isAccessibleItem = true; 

    private static readonly Color InaccessibleSlotColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);
    private static readonly Color InaccessibleIconColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    float currentHLAlpha = 0f;

    public enum SlotState {inven, weapon, accessary};
    public SlotState slotState;

    private void Awake() 
    {        
        InitComponents();
        InitValues();
    }

    void InitComponents()
    {
        slotRect = GetComponent<RectTransform>();
        iconRect = iconImage.rectTransform;
        highlightRect = highlightImage.rectTransform;

        iconGo = iconRect.gameObject;
        textGo = amountText.gameObject;
        highlightGo = highlightImage.gameObject;

        slotImg = GetComponent<Image>();
    }

    void InitValues()
    {
        iconRect.pivot = new Vector2(0.5f, 0.5f); // 피벗중앙
        iconRect.anchorMin = Vector2.zero;
        iconRect.anchorMax = Vector2.one;

        iconRect.offsetMin = Vector2.one * padding;
        iconRect.offsetMax = Vector2.one * -padding;

        highlightRect.pivot = iconRect.pivot;
        highlightRect.anchorMin = iconRect.anchorMin;
        highlightRect.anchorMax = iconRect.anchorMax;
        highlightRect.offsetMin = iconRect.offsetMin;
        highlightRect.offsetMax = iconRect.offsetMax;

        iconImage.raycastTarget = false;
        highlightImage.raycastTarget = false;

        HideIcon();
        highlightGo.SetActive(false);
    }

    void ShowIcon() => iconGo.SetActive(true);
    void HideIcon() => iconGo.SetActive(false);

    void ShowText() => textGo.SetActive(true);
    void HideText() => textGo.SetActive(false);

    public void SetSlotIndex(int index) => this.index = index;

    public void SetSlotAccessibleState(bool value) 
    {
        if (isAccessibleSlot == value) return;

        if (value) slotImg.color = Color.black;
        else
        {
            slotImg.color = InaccessibleSlotColor;
            HideIcon();
        }

        isAccessibleSlot = value;
    }

    public void SetItemAccessibleState(bool value)
    {
        if (isAccessibleItem == value) return;

        if (value) {iconImage.color = Color.white; amountText.color = Color.white;}
        else {iconImage.color = InaccessibleIconColor; amountText.color = InaccessibleIconColor;}

        isAccessibleItem = value;
    }

    public void SwapOrMoveIcon(ItemSlotUI other)
    {
        if (other == null) return;
        if (other == this) return;
        if (!this.isAccessible) return;
        if (!other.isAccessible) return;

        var temp = iconImage.sprite;

        if (other.hasItem) SetItem(other.iconImage.sprite);

        else RemoveItem();
        other.SetItem(temp);
    }

    public void SetItem(Sprite itemSprite) 
    {
        if (itemSprite != null)
        {
            iconImage.sprite = itemSprite;
            ShowIcon();
        }
        else RemoveItem();
    }

    public void RemoveItem()
    {
        iconImage.sprite = null;
        HideIcon();
        HideText();
    }

    public void SetIconAlpha(float alpha)
    {
        iconImage.color = new Color(iconImage.color.r, iconImage.color.g, iconImage.color.b, alpha);
    }

    public void SetItemAmount(int amount)
    {
        if(hasItem && amount > 1) ShowText();
        else HideText();

        amountText.text = amount.ToString();
    }

    public void SetHighlightOnTop(bool value)
    {
        if(value) highlightRect.SetAsLastSibling();
        else highlightRect.SetAsFirstSibling();
    }

    public void Highlight(bool show)
    {
        if(show) StartCoroutine(nameof(HighlightFadeInRoutine));
        else StartCoroutine(nameof(HighlightFadeOutRoutine));
    }

    IEnumerator HighlightFadeInRoutine()
    {
        StopCoroutine(nameof(HighlightFadeOutRoutine));
        highlightGo.SetActive(true);

        float unit = highlightAlpha / highlightFadeDuration;

        for(; currentHLAlpha <= highlightAlpha; currentHLAlpha += unit * Time.deltaTime)
        {
            highlightImage.color = new Color(highlightImage.color.r, highlightImage.color.g, highlightImage.color.b, currentHLAlpha);

            yield return null;
        }
    }

    IEnumerator HighlightFadeOutRoutine()
    {
        StopCoroutine(nameof(HighlightFadeInRoutine));

        float unit = highlightAlpha / highlightFadeDuration;

        for (; currentHLAlpha >= 0f; currentHLAlpha -= unit * Time.deltaTime)
        {
            highlightImage.color = new Color(highlightImage.color.r, highlightImage.color.g, highlightImage.color.b, currentHLAlpha);

            yield return null;
        }

        highlightGo.SetActive(false);
    }
}
