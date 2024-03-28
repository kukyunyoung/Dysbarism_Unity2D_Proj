using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<Item> invenData = new List<Item>(); // 인벤토리 데이터
    public List<float> playerStatusData = new List<float>(); // 플레이어 스탯 데이터

    public float playTime;
    public int gold;
    public int playerHp;
    public int playerHpMax;
    // 맵 진행도
    // 플레이어 음식

    void Awake()
    {
        instance = this;
    }
}
