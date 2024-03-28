using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AttBtn : MonoBehaviour//, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] TextMeshProUGUI stateText;
    [SerializeField] Button attBtn;
    [SerializeField] Button reloadBtn;

    GameObject weapon;
    Sword_Normal swordAtt;
    Gun_Normal gunAtt;
    Staff_Normal staffAtt;

    void Start()
    {
        weapon = GameObject.FindGameObjectWithTag("Sword"); // ½ÃÀÛ ¹«±â Ä®
        swordAtt = weapon.GetComponent<Sword_Normal>();
        attBtn.onClick.AddListener(() => swordAtt.Attack());
    }

    public void SetWeapon(GameObject weapon)
    {
        this.weapon = weapon;
        switch (this.weapon.tag) 
        {
            case "Sword":
                swordAtt = weapon.GetComponent<Sword_Normal>();
                reloadBtn.gameObject.SetActive(false);
                attBtn.onClick.RemoveAllListeners();
                attBtn.onClick.AddListener(() => swordAtt.Attack());
                break;
            case "Gun":
                gunAtt = weapon.GetComponent<Gun_Normal>();
                reloadBtn.gameObject.SetActive(true);
                attBtn.onClick.RemoveAllListeners();
                break;
            case "Staff":
                staffAtt = weapon.GetComponent<Staff_Normal>();
                reloadBtn.gameObject.SetActive(false);
                attBtn.onClick.RemoveAllListeners();
                break;
            default:
                break;
        }
    }

    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    if (weapon.transform.CompareTag("Gun")) gunAtt.Attack();
    //    else if (weapon.transform.CompareTag("Staff")) staffAtt.Attack();
    //}

    //public void OnPointerUp(PointerEventData eventData)
    //{
    //    if (weapon.transform.CompareTag("Gun")) gunAtt.AttackFinish();
    //    else if (weapon.transform.CompareTag("Staff")) staffAtt.AttackFinish();
    //}
}
