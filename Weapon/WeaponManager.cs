using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] public List<GameObject> weapon = new List<GameObject>();
    [SerializeField] Image bulletWeaponImg;
    public static TextMeshProUGUI bulletText;

    private void Start() 
    {
        if(bulletText==null) bulletText = bulletWeaponImg.transform.parent.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (UIManager.instance.stopTime!=0) return;
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if(weapon[0]!=null) 
            {
                weapon[0].SetActive(true);
                bulletWeaponImg.sprite = weapon[0].GetComponent<SpriteRenderer>().sprite;
            }
            if (weapon[1]!=null) weapon[1].SetActive(false);
            
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            if(weapon[1]!=null) 
            {
                weapon[1].SetActive(true);
                bulletWeaponImg.sprite = weapon[1].GetComponent<SpriteRenderer>().sprite;
            }
            if (weapon[0]!=null) weapon[0].SetActive(false);
        }
    }
}
