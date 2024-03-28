using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VillageSceneManager : MonoBehaviour
{
    [SerializeField] Image fadePanel;
    [SerializeField] GameObject blessPanel;
    [SerializeField] Npc Priest;
    [SerializeField] GameObject playerPos;
    [SerializeField] AudioClip bgm;
    GameObject player;
    Rigidbody2D playerBody;
    Color fade = new Color(0, 0, 0, 0.01f);
    bool nextScene = false;
    bool firstStart;

    void Start()
    {
        nextScene = false;
        firstStart = true;
        fadePanel.gameObject.SetActive(true);
        StartCoroutine(FadeIn());
        player = PlayerMove.instance.gameObject;
        player.transform.position = playerPos.transform.position;
        playerBody = player.GetComponent<Rigidbody2D>();
        InitPlayer();
        blessPanel.SetActive(true);
        UIManager.instance.stopTime++;
    }

    // 시작 마을에서 플레이어 스탯 등등을 초기화
    // 스탯, 체력, 골드, 시작무기, 물이없는곳에서 점프가능, 물이없는곳에서 중력, 
    void InitPlayer()
    {
        UIManager.instance.gameObject.GetComponent<SaveManager>().InitJson();
        player.GetComponent<PlayerMove>().canJump = true;
        player.GetComponent<PlayerStatus>().InitStatus(); // 스탯 초기화
        player.GetComponent<PlayerStatus>().sp.SetText(); // 스탯 작성
        // 저장된 골드, 체력, 아이템 로드
        //PlayerHP.hP = 200;
        //PlayerHP.hp_Max = 200;
        playerBody.gravityScale = 1;
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.transform.CompareTag("Player") && !nextScene)
        {
            nextScene = true;
            UIManager.instance.gameObject.GetComponent<SaveManager>().JsonSave();
            StartCoroutine(FadeOut());
        }
    }

    IEnumerator FadeOut()
    {
        for (int i = 0; i < 100; i++)
        {
            fadePanel.color += fade;
            yield return new WaitForSeconds(0.02f);
        }

        SceneManager.LoadScene("Stage1");
    }

    IEnumerator FadeIn()
    {
        for (int i = 0; i < 100; i++)
        {
            fadePanel.color -= fade;
            yield return new WaitForSeconds(0.02f);
        }
    }

    public void SetBless(string name)
    {
        blessPanel.SetActive(false);
        Priest.npcOn=false;
        UIManager.instance.stopTime--;
        PlayerMove.instance.SetBless(name);
    }
}
