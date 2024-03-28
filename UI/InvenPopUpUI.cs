using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InvenPopUpUI : MonoBehaviour
{
    [Header("Confirmation Popup")]
    [SerializeField] GameObject confirmationPopupObj;
    [SerializeField] TextMeshProUGUI confirmationItemNameText;
    [SerializeField] TextMeshProUGUI confirmationText;
    [SerializeField] Button confirmationOkBtn;
    [SerializeField] Button confirmationNoBtn;

    [Header("Amount Input Popup")]
    [SerializeField] GameObject amountInputPopupObj;
    [SerializeField] TextMeshProUGUI amountInputItemNameText;
    [SerializeField] InputField amountInputField;
    [SerializeField] Button amountPlusBtn;
    [SerializeField] Button amountMinusBtn;
    [SerializeField] Button amountInputOkBtn;
    [SerializeField] Button amountInputNoBtn;

    event Action OnConfirmationOk; // 버림 팝업에서 버튼을 누를경우 실행할 이벤트
    event Action<int> OnAmountInputOk; // 나눔 팝업에서 확인 버튼 눌렀을때 동작할 이벤트

    int maxAmount;

    private void Awake() 
    {
        confirmationOkBtn.onClick.AddListener(HidePanel);
        confirmationOkBtn.onClick.AddListener(HideConfirmationPopup);
        confirmationOkBtn.onClick.AddListener(() => OnConfirmationOk?.Invoke());

        confirmationNoBtn.onClick.AddListener(HidePanel);
        confirmationNoBtn.onClick.AddListener(HideConfirmationPopup);

        // 수량 - 버튼 이벤트
        amountMinusBtn.onClick.AddListener(()=>
        {
            int.TryParse(amountInputField.text, out int amount);
            if(amount > 1)
            {
                int nextAmount = Input.GetKey(KeyCode.LeftShift) ? amount -10 : amount -1;
                if(nextAmount < 1) nextAmount = 1;
                amountInputField.text = nextAmount.ToString();
            }
        });

        // 수량 + 버튼 이벤트
        amountPlusBtn.onClick.AddListener(() =>
        {
            int.TryParse(amountInputField.text, out int amount);
            if (amount < maxAmount)
            {
                int nextAmount = Input.GetKey(KeyCode.LeftShift) ? amount + 10 : amount + 1;
                if (nextAmount > maxAmount) nextAmount = maxAmount;
                amountInputField.text = nextAmount.ToString();
            }
        });

        // 입력 값 범위 제한
        amountInputField.onValueChanged.AddListener(str =>
        {
            int.TryParse(str, out int amount);
            bool flag = false;

            if(amount < 1) {flag = true; amount =1;}
            else if(amount > maxAmount) {flag = true; amount = maxAmount;}

            if(flag) amountInputField.text = amount.ToString();
        });
    }

    void ShowPanel() => gameObject.SetActive(true);
    void HidePanel() => gameObject.SetActive(false);

    void ShowConfirmationPopup(string itemName)
    {
        confirmationItemNameText.text = itemName;
        confirmationPopupObj.SetActive(true);
    }

    void HideConfirmationPopup() => confirmationPopupObj.SetActive(false);
    void SetconfirmationOKEvent(Action handler) => OnConfirmationOk = handler;

    public void OpenConfirmationPopup(Action okCallback, string itemName)
    {
        ShowPanel();
        ShowConfirmationPopup(itemName);
        OnConfirmationOk = okCallback;
    }

    public void OpenAmountInputPopup(Action<int> okCallback, int currentAmount, string itemName)
    {
        maxAmount = currentAmount -1;
        amountInputField.text = "1";

        ShowPanel();
        ShowAmountInputPopup(itemName);
        OnAmountInputOk = okCallback;
    }

    void ShowAmountInputPopup(string itemName)
    {
        amountInputItemNameText.text = itemName;
        amountInputPopupObj.SetActive(true);
    }
}
