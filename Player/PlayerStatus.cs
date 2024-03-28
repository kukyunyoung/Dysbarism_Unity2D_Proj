using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour 
{
    // power = 30 이면 공격력 * 1.3
    // armory = 30 이면 getDmg()의 받는데미지 dmg * (1 - armory * 0.01) = 70프로의 데미지 - 상한70
    // critical의 수치만큼 크리티컬 확률 - 상한x
    // avoidance의 수치만큼 회피확률 - 상한 75

    // swAttSpeed의 수치만큼 칼타입 공격속도에 곱함
    // reloadTime의 수치만큼 총타입 장전속도에 나눔
    // chargeTime의 수치만큼 스태프타입 충전속도에 나눔

    public int[] paca = new int[4]; // power, armory, critical, avoidance
    public float[] src = new float[3]; // swAttSpeed, reloadTime, chargeTime
    public List<float> playerStatus = new List<float>();
    public StatusPanel sp;

    private void Awake() 
    {
        playerStatus.Add(10);
        playerStatus.Add(10);
        playerStatus.Add(20);
        playerStatus.Add(10);

        playerStatus.Add(1);
        playerStatus.Add(1);
        playerStatus.Add(1);

        for(int i=0;i<paca.Length;i++)
        {
            paca[i] = 0;
        }
        for (int i = 0; i < src.Length; i++)
        {
            src[i] = 1;
        }
    }

    public void InitStatus()
    {
        playerStatus.Clear();

        playerStatus.Add(10);
        playerStatus.Add(10);
        playerStatus.Add(20);
        playerStatus.Add(10);

        playerStatus.Add(1);
        playerStatus.Add(1);
        playerStatus.Add(1);
    }
}
