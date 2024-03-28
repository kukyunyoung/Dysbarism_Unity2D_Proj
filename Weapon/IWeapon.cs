using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    public void Attack(); // bool 타입 변수 isAtt을 제어해서 OnTriggerEnter 등으로 적과 상호작용
    public void Skill();
}
