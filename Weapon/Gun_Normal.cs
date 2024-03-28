using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class Gun_Normal : MonoBehaviour, IWeapon, IUseableItem
{
    [SerializeField] Bullet[] bullet = new Bullet[10];
    [SerializeField] GameObject bulletPos;
    [SerializeField] AudioClip shotClip;
    [SerializeField] AudioClip reloadClip;
    [SerializeField] TextMeshProUGUI bulletText;

    public enum GunState { ready, empty, reloading }
    public GunState gunstate { get; private set; }

    GameObject player;
    AudioSource gunAudio;
    SpriteRenderer playerSprite;
    SpriteRenderer sprite;
    PlayerOnTarget playerAngle;
    WaitForSeconds reloadWFS;
    Coroutine bulletCo;
    GameObject bulletPulling;

    float angle;
    float shotDelay = 0.12f;
    float reloadTime = 1.5f;
    float bulletSpeed = 15;
    int bulletArr;
    int nowBullet = 30;
    int fullBullet = 30;
    bool isShot;
    bool isAttack;
    bool isPushAttBtn;
    bool isReload;

    void Start()
    {
        player = PlayerMove.instance.gameObject;
        gunAudio = GetComponent<AudioSource>();
        playerSprite = player.GetComponent<SpriteRenderer>();
        sprite = GetComponent<SpriteRenderer>();
        playerAngle = player.GetComponent<PlayerOnTarget>();
        gunstate = GunState.ready;
        reloadWFS = new WaitForSeconds(reloadTime / 50);
        bulletText = UIManager.instance.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        bulletPulling = GameObject.FindGameObjectWithTag("Bullet");

        for (int i = 0; i < 10; i++)
        {
            bullet[i] = bulletPulling.transform.GetChild(i).GetComponent<Bullet>();
        }
    }

    private void FixedUpdate()
    {
        if (UIManager.instance.stopTime!=0) return;
        sprite.flipX = false;
        sprite.flipY = (PlayerMove.targetPos.x < 0) ? true : false;
        angle = Mathf.Atan2(PlayerMove.targetPos.y, PlayerMove.targetPos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        bulletPos.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        if (Input.GetMouseButton(0)) Attack();
        else if (Input.GetMouseButtonUp(0)) AttackFinish();
    }

    void Update()
    {
        if(UIManager.instance.stopTime!=0) return;
        if (Input.GetKeyDown(KeyCode.R)) StartCoroutine(Reload());
    }

    private void OnEnable() 
    {
        WeaponManager.bulletText.gameObject.SetActive(true);
        reloadTime = 1.5f * PlayerMove.instance.gameObject.GetComponent<PlayerStatus>().playerStatus[5];
        gunstate = GunState.ready;
        if (nowBullet <= 0) gunstate = GunState.empty;
        isReload = false;
    }

    private void OnDisable() 
    {
        StopAllCoroutines();
    }

    IEnumerator Reload()
    {
        if(!isReload)
        {
            isReload = true;
            gunstate = GunState.reloading;
            nowBullet = 0;
            bulletText.text = nowBullet + " / " + fullBullet;
            for (int i = 0; i < 50; i++)
            {
                yield return reloadWFS;
            }

            nowBullet = fullBullet;
            gunAudio.PlayOneShot(reloadClip);
            bulletText.text = nowBullet + " / " + fullBullet;

            isAttack = isPushAttBtn;
            isReload = false;
            isShot = false;
            gunstate = GunState.ready;
        }
    }

    public void Attack()
    {
        isAttack = true;
        isPushAttBtn = true;
        print("Gun Attack on");
        bulletCo = StartCoroutine(Process());
    }

    IEnumerator Process()
    {
        print("Gun Process on");
        if (gunstate == GunState.ready && !isShot)
        {
            isShot = true;
            bullet[bulletArr].angle = angle;
            bullet[bulletArr].gunFirePos = bulletPos.transform;
            bullet[bulletArr++].gameObject.SetActive(true);
            nowBullet--;
            bulletText.text = nowBullet + " / " + fullBullet;
            gunAudio.PlayOneShot(shotClip);
            if (nowBullet <= 0) gunstate = GunState.empty;
            if (bulletArr > 9) bulletArr = 0;

            yield return new WaitForSeconds(shotDelay);
            isShot = false;
        }
        else if (gunstate == GunState.empty)
        {
            isAttack = false;
            gunstate = GunState.reloading;
            StartCoroutine(Reload());
        }
    }

    public void AttackFinish()
    {
        isAttack = false;
        isPushAttBtn = false;
    }

    public void Skill()
    {
        throw new System.NotImplementedException();
    }

    public bool Use()
    {
        throw new System.NotImplementedException();
    }
}
