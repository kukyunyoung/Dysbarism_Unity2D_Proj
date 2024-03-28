using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    [SerializeField] Map[] maps; // 0~9 : battle / 6,7 : npc / 8 : middleboss / 9 : boss
    [SerializeField] Stage1Manager stage1Manager;
    [SerializeField] public GameObject parallaxBack;
    public Image fadePanel;
    public FollowCamera cam;

    List<int> battleDoor = new List<int>() {0,1,2,5,6,7};
    List<int> battleMaps = new List<int>() {0,1,2,3,4,5,6,7,8,9};


    private void Start() 
    {
        for(int i=0; i<=10; i++)
        {
            maps[i].mapType = Map.MapType.battle;
        }
        maps[10].mapType = Map.MapType.npc;
        maps[11].mapType = Map.MapType.npc;
        maps[12].mapType = Map.MapType.middleBoss;
        maps[13].mapType = Map.MapType.boss;
        InitDoorNum();
    }

    void InitDoorNum()
    {
        maps[10].mapType = Map.MapType.npc;
        maps[10].enterence.doorNum = 3;
        maps[10].exit.doorNum = 4;
        maps[11].mapType = Map.MapType.npc;
        maps[11].enterence.doorNum = 8; // npc
        maps[11].exit.doorNum = 9;

        maps[12].enterence.doorNum = 4;
        maps[12].exit.doorNum = 5;
        maps[12].mapType = Map.MapType.middleBoss;
        maps[13].enterence.doorNum = 9; // middleboss, boss
        maps[13].exit.doorNum = 10;
        maps[13].mapType = Map.MapType.boss;

        // 10개의 battle맵 중에서 6개를 골라 무작위로 배치함
        // battleMaps[randMap] == randomMap
        // battleDoor[randDoor] != randDoor
        for (int i=0; i<6; i++)
        {
            int randMap = Random.Range(0, battleMaps.Count); // 0~9
            int randDoor = Random.Range(0, battleDoor.Count); // 0~5

            maps[battleMaps[randMap]].enterence.doorNum = battleDoor[randDoor]; // 입구
            maps[battleMaps[randMap]].exit.doorNum = battleDoor[randDoor] + 1;  // 출구
            maps[battleMaps[randMap]].mapType = Map.MapType.battle;
            if (battleDoor[randDoor] == 0) // 첫번째 맵에 카메라 및 플레이어 할당
            {
                cam.map = maps[battleMaps[randMap]].map.GetComponent<RectTransform>();
                stage1Manager.SetFirstDoor(maps[battleMaps[randMap]]);
                cam.SetViewSize();
            }
            battleMaps.RemoveAt(randMap);
            battleDoor.RemoveAt(randDoor);
        }

        // 선택안된 맵들은 비활성화
        maps[battleMaps[0]].gameObject.SetActive(false);
        maps[battleMaps[1]].gameObject.SetActive(false);
        maps[battleMaps[2]].gameObject.SetActive(false);
        maps[battleMaps[3]].gameObject.SetActive(false);

        // 현재맵 출구와 다음맵 입구를 이어줌
        for (int i=0; i<maps.Length; i++){
            for(int j=0; j<maps.Length; j++){
                if(maps[i].exit.doorNum == maps[j].enterence.doorNum){  
                    maps[i].exit.nextDoor = maps[j].enterence;
                    break;
                }
            }
        }
        StartCoroutine(CamDelay());
    }

    IEnumerator CamDelay()
    {
        parallaxBack.GetComponent<ParallaxBackground>().isCamMove = true;
        parallaxBack.transform.localPosition = PlayerMove.instance.gameObject.transform.localPosition + new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), 20);
        yield return new WaitForSeconds(1f);
        parallaxBack.GetComponent<ParallaxBackground>().isCamMove = false;
    }
}
