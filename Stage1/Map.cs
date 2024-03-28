using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Map : MonoBehaviour
{
    public enum MapType{ battle, npc, middleBoss, boss};
    public MapType mapType;
    public EnteranceManager enterence;
    public EnteranceManager exit;
    public GameObject map;
    [Header("활성화 할 맵 요소들")]
    public GameObject[] mapGo;
}
