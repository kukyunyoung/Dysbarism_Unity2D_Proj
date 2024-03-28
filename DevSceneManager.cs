using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DevSceneManager : MonoBehaviour
{
    [SerializeField] GameObject[] blessBtn;

    private void Awake() 
    {
        for(int i=0; i<blessBtn.Length; i++)
        {
            if(!blessBtn[i].activeSelf) blessBtn[i].gameObject.SetActive(true);
        }
        PlayerHP.hP = 9999;
        PlayerHP.hp_Max = 9999;
    }
}
