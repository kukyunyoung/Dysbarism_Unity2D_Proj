using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PortionItem : CountableItem, IUseableItem
{
    public PortionItem(PortionItemData data, int amount = 1) : base(data, amount) {item = data;}
    PortionItemData item;
    public bool Use()
    {
        switch(CountableData.ID)
        {
            case 201: // hp포션
                Amount--;
                PlayerHP.hP += 10;
                if(PlayerHP.hP > PlayerHP.hp_Max) PlayerHP.hP = PlayerHP.hp_Max;
                return true;
            case 202: // 엘릭서
                Amount--;
                PlayerHP.hp_Max += 10;
                PlayerMove.instance.ps.paca[0] += item.Power;
                return true;
            case 203: // 스펠인챈터
                Amount--;
                PlayerMove.instance.ps.paca[2] += item.Critical;
                PlayerMove.instance.ps.src[2] *= item.ChargeSpeed;
                return true;
            case 204: // 경화의돌
                Amount--;
                PlayerMove.instance.ps.paca[1] += item.Armory;
                return true;
            case 205: // 호크아이
                Amount--;
                PlayerMove.instance.ps.paca[2] += item.Critical;
                PlayerMove.instance.ps.src[1] *= item.ReloadSpeed;
                return true;
        }
        return true;
    }

    protected override CountableItem Clone(int amount)
    {
        return new PortionItem(CountableData as PortionItemData, amount);
    }
}
