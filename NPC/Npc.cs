using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npc : MonoBehaviour
{
    [SerializeField] GameObject popupBtnE;
    [SerializeField] GameObject UI;
    [SerializeField] AudioClip tapClip;

    public bool npcOn;
    GameObject player;
    AudioSource audioSource;
    int talkArr;

    string[] smithTalk = new string[3];
    string[] witchTalk = new string[3];
    string[] priestTalk = new string[3];
    string[] upgradeTalk = new string[3];

    private void Start() 
    {
        npcOn = false;
        audioSource = GetComponent<AudioSource>();
        InitTalk();
    }

    void InitTalk()
    {
        smithTalk[0] = "돈은 있어?";
        smithTalk[1] = "장비를 사보자!";
        smithTalk[2] = "구매한 장비는\n 우클릭으로 장착해봐!";

        witchTalk[0] = "맛있는 냄새...";
        witchTalk[1] = "먹으면 강해지는 음식이라니..\n 독은 아니겠지??";
        witchTalk[2] = "X키를 눌러서\n 스탯이 변하는지 확인해봐!";

        priestTalk[0] = "축복을 바꿔보자!";
        priestTalk[1] = "아이템과 축복의 시너지!!";
        priestTalk[2] = "축복은 한개만 적용할 수 있어!";

        upgradeTalk[0] = "무기 아이템만 강화할 수 있어!";
        upgradeTalk[1] = "골드를 사용해서 무기를 업그레이드 시키자";
        upgradeTalk[2] = "새로운 스킬이 생길거같아!";
    }

    private void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(UIManager.instance.stopTime>0 && UI.activeSelf && npcOn)
            {
                npcOn = false;
                UIManager.instance.stopTime--;
                UI.SetActive(false);
            }
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if(player == null || npcOn) return;

            npcOn = true;
            UIManager.instance.stopTime++;
            audioSource.PlayOneShot(tapClip);
            UI.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.transform.CompareTag("Player"))
        {
            player = other.gameObject;
            popupBtnE.SetActive(true);
        }
    }

    private void OnTriggerStay2D(Collider2D other) 
    {
        if(other.transform.CompareTag("Player") && !Fairy.instance.isTalk)
        {
            switch (gameObject.tag)
            {
                case "Smith":
                    talkArr = Random.Range(0, smithTalk.Length);
                    StartCoroutine(Fairy.instance.Talk(smithTalk[talkArr]));
                    break;
                case "Witch":
                    talkArr = Random.Range(0, witchTalk.Length);
                    StartCoroutine(Fairy.instance.Talk(witchTalk[talkArr]));
                    break;
                case "Priest":
                    talkArr = Random.Range(0, priestTalk.Length);
                    StartCoroutine(Fairy.instance.Talk(priestTalk[talkArr]));
                    break;
                case "UpgradeNpc":
                    talkArr = Random.Range(0, upgradeTalk.Length);
                    StartCoroutine(Fairy.instance.Talk(upgradeTalk[talkArr]));
                    break;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.transform.CompareTag("Player"))
        {
            player = null;
            popupBtnE.SetActive(false);
        }
    }
}
