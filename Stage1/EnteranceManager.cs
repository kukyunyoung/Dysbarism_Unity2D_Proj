using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EnteranceManager : MonoBehaviour
{
    public int doorNum;
    public enum DoorState{ enter, exit}
    public DoorState doorState;
    public EnteranceManager nextDoor;

    Image fadePanel;
    Map nextMap;
    Color fade = new Color(0, 0, 0, 0.01f);
    GameObject player;
    Map nowMap;

    bool checkDoor;

    private void Start() 
    {
        if(gameObject.CompareTag("Enterence")) doorState = DoorState.enter;
        else if(gameObject.CompareTag("Exit")) doorState = DoorState.exit;

        fadePanel = transform.parent.parent.GetComponent<MapManager>().fadePanel;
        nowMap = transform.parent.GetComponent<Map>();
    }

    IEnumerator FadeOut()
    {
        UIManager.instance.gameObject.GetComponent<SaveManager>().JsonSave();
        nextMap = nextDoor.transform.parent.GetComponent<Map>();
        UIManager.instance.stopTime++;
        for (int i = 0; i < 100; i++)
        {
            fadePanel.color += fade;
            yield return new WaitForSeconds(0.01f);
        }

        player.transform.position = nextDoor.transform.position + new Vector3(1, 0, 0);

        // 넘어갈 맵의 몹, 함정 등을 활성화함
        for(int i=0;i<nextMap.mapGo.Length; i++)
        {
            nextMap.mapGo[i].SetActive(true); 
        }

        transform.parent.parent.GetComponent<MapManager>().cam.map = nextDoor.transform.parent.GetComponent<Map>().map.GetComponent<RectTransform>();
        transform.parent.parent.GetComponent<MapManager>().cam.SetViewSize();
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        //UIManager.instance.gameObject.GetComponent<SaveManager>().JsonLoad();
        transform.parent.parent.GetComponent<MapManager>().parallaxBack.GetComponent<ParallaxBackground>().isCamMove = true;
        transform.parent.parent.GetComponent<MapManager>().parallaxBack.transform.localPosition = player.transform.localPosition + new Vector3(Random.Range(-10,10), Random.Range(-10, 10), 20);
        
        UIManager.instance.stopTime--;
        for (int i = 0; i < 100; i++)
        {
            fadePanel.color -= fade;
            yield return new WaitForSeconds(0.01f);
        }

        if(nextMap.mapType == Map.MapType.middleBoss || nextMap.mapType == Map.MapType.boss) StartCoroutine(BossZoom());
        yield return new WaitForSeconds(1);
        transform.parent.parent.GetComponent<MapManager>().parallaxBack.GetComponent<ParallaxBackground>().isCamMove = false;
    }

    IEnumerator BossZoom()
    {
        WaitForSeconds twoSecond = new WaitForSeconds(2);
        print("zoomin");
        UIManager.instance.stopTime++;
        yield return new WaitForSeconds(1);

        for(int i=0; i<UIManager.instance.fadeObj.Length; i++)
            UIManager.instance.fadeObj[i].SetActive(false);
        // 카메라가 보스 추적하고
        transform.parent.parent.GetComponent<MapManager>().cam.SetTarget(nextMap.mapGo[0].transform.GetChild(0).GetChild(0).gameObject);
        // 보스 이름 나오고
        GameObject pont=null;
        if(nextMap.mapType == Map.MapType.middleBoss) pont = UIManager.instance.crabPont;
        else if(nextMap.mapType == Map.MapType.boss) pont = UIManager.instance.hivePont;
        pont.SetActive(true);
        
        yield return new WaitForSeconds(6);
        // UI다시 표시하고
        for (int i = 0; i < UIManager.instance.fadeObj.Length; i++)
            UIManager.instance.fadeObj[i].SetActive(true);
        // 플레이어 조작 가능
        UIManager.instance.stopTime--;
        // 카메라가 다시 플레이어 추적하게
        transform.parent.parent.GetComponent<MapManager>().cam.SetTarget();
    }

    IEnumerator CheckDoor()
    {
        yield return new WaitForSeconds(1);
        checkDoor = false;
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if(other.transform.CompareTag("Player") && doorState == DoorState.exit && !checkDoor)
        {
            checkDoor = true;
            StartCoroutine(CheckDoor());
            for (int i=0; i<nowMap.mapGo.Length; i++)
            {
                if(nowMap.mapGo[i].activeSelf) 
                {
                    print(nowMap.mapGo[i]);
                    UIManager.instance.SetSystemMessage("모든 몬스터를 처치해야 넘어갈 수 있습니다!");
                    return;
                }
            }

            player = other.gameObject;

            if(doorNum == 10) SceneManager.LoadScene("Stage2");
            else StartCoroutine(FadeOut());
        }
    }
}
