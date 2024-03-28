using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1Manager : MonoBehaviour
{
    [SerializeField] EnteranceManager interence;
    [SerializeField] FollowCamera cam;
    [SerializeField] WeaponUpgrade weaponUpgrade;

    GameObject player;
    Rigidbody2D playerBody;
    AudioSource audioSource;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerBody = player.GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        InitPlayer();
    }

    void InitPlayer()
    {
        player.GetComponent<PlayerMove>().canJump = false;
        PlayerHP.hP = 1;
        playerBody.gravityScale = 0.3f;
        // 저장된 골드, 체력, 아이템 로드
        UIManager.instance.gameObject.GetComponent<SaveManager>().JsonLoad();
        UIManager.instance.gameObject.transform.GetChild(5).GetComponent<InventoryManager>().weaponUpgrade = weaponUpgrade;
    }

    public void SetFirstDoor(Map enter)
    {
        if(player == null) player = GameObject.FindWithTag("Player");
        interence = enter.enterence;
        for (int i = 0; i < enter.mapGo.Length; i++)
        {
            enter.mapGo[i].SetActive(true);
        }
        player.transform.position = interence.transform.position + new Vector3(1, 0, 0);
    }
}
