using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusPanel : MonoBehaviour
{
    [SerializeField] Text powerText;
    [SerializeField] Text armoryText;
    [SerializeField] Text criticalText;
    [SerializeField] Text avoidText;
    [SerializeField] Text swSpeedText;
    [SerializeField] Text reloadSpeedText;
    [SerializeField] Text chargeSpeedText;

    [SerializeField]PlayerStatus ps;

    private void OnEnable() 
    {
        powerText.text      = "위력 : " + ps.playerStatus[0];
        armoryText.text     = "방어력 : " + ps.playerStatus[1];
        criticalText.text   = "크리티컬 : " + ps.playerStatus[2];
        avoidText.text      = "회피 : " + ps.playerStatus[3];
        swSpeedText.text    = "공격속도 : " + ps.playerStatus[4];
        reloadSpeedText.text = "장전속도 : " + ps.playerStatus[5];
        chargeSpeedText.text = "충전속도 : " + ps.playerStatus[6];
    }

    public void SetText()
    {
        powerText.text = "위력 : " + ps.playerStatus[0];
        armoryText.text = "방어력 : " + ps.playerStatus[1];
        criticalText.text = "크리티컬 : " + ps.playerStatus[2];
        avoidText.text = "회피 : " + ps.playerStatus[3];
        swSpeedText.text = "공격속도 : " + ps.playerStatus[4];
        reloadSpeedText.text = "장전속도 : " + ps.playerStatus[5];
        chargeSpeedText.text = "충전속도 : " + ps.playerStatus[6];
    }
}
