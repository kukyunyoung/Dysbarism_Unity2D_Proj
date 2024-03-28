using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

/// <summary>
/// Thorn() 가시 발사하는 코루틴
/// EyeOn() 플레이어가 바라보고있을때 멘탈추가하는 코루틴
/// Bomb() 플레이어 위치에 폭탄 5번 터트리는 코루틴
/// </summary>


public class Hive : MonoBehaviour
{
    [SerializeField] AudioClip hitClip;
    [SerializeField] AudioClip avoidClip;
    [SerializeField] AudioClip dieClip;
    [Space]
    [SerializeField] GameObject[] thorn;
    [SerializeField] GameObject[] bomb;
    [SerializeField] GameObject[] niddle;
    [SerializeField] GameObject[] eyeMob;
    [Space]
    [SerializeField] GameObject hpBack;
    [SerializeField] Image hpImg;
    [SerializeField] Image mentalGaze;
    [SerializeField] PostProcessVolume distortion;
    [Space]
    [SerializeField] GameObject wall;
    [SerializeField] public RectTransform mapSize;


    public bool eyeOn;
    public bool thornOn;
    public bool bombOn;
    public bool isViewing;
    bool mentalFull=false;
    bool hadUntouchable=false;

    int hp;
    int hpMax;
    int mental; // 최대 100
    float critical;
    public int mobDeadCount=0;

    GameObject player;
    Animator anim;
    SpriteRenderer playerSprite;
    AudioSource audioSource;
    WaitForSeconds oneSecond = new WaitForSeconds(1);
    ChromaticAberration chroma;


    public enum HiveState{idle, thorn, bomb, untouchable, eye, die}
    public HiveState hiveState = HiveState.idle;

    void Start()
    {
        player = PlayerMove.instance.gameObject;
        anim = GetComponent<Animator>();
        playerSprite = player.GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        
        distortion.profile.TryGetSettings(out chroma);

        hp = 1000;
        hpMax = 1000;
        mental = 0;

        StartCoroutine(StartPattern());
        anim.Play("Hive_Checking");
    }

    private void OnEnable() 
    {
        critical = PlayerMove.instance.gameObject.GetComponent<PlayerStatus>().playerStatus[2];
    }

    IEnumerator StartPattern()
    {
        yield return new WaitForSeconds(6);
        StartCoroutine(Idle());
        StartCoroutine(ChoicePattern());
    }

    IEnumerator ChoicePattern()
    {
        if(hiveState is HiveState.untouchable) StopCoroutine(ChoicePattern());

        anim.Play("Hive_LeftView");
        yield return new WaitForSeconds(3);

        if (hiveState is HiveState.untouchable) StopCoroutine(ChoicePattern());

        int random = Random.Range(0,3);
        switch(random)
        {
            case 0:
                StartCoroutine(Bomb());
                break;
            case 1:
                StartCoroutine(Thorn());
                break;
            case 2:
                StartCoroutine(EyeOn());
                break;
        }
    }

    // 항상 발동하고있으며 플레이어가 바라보고있으면 멘탈+1 그렇지않으면 -2함
    IEnumerator Idle()
    {
        while (true)
        {
            isViewing = !playerSprite.flipX ? true : false;

            if (isViewing) mental += 1;
            else mental -= 2;
            mentalGaze.fillAmount = (float)mental / (float)100;

            if(mental >= 90) chroma.intensity.value =(float)(mental - 90) / (float)10;

            if (mental >= 100) 
            {
                StartCoroutine(DeathBeam());
                break;
            }
            yield return oneSecond;
        }
    }

    // 멘탈게이지가 다 차올랐을때 실행되는 즉사패턴
    IEnumerator DeathBeam()
    {
        if(!mentalFull)
        {
            mentalFull = true;
            UIManager.instance.stopTime++;
            UIManager.instance.filterPanel.gameObject.SetActive(true);
            anim.Play("Hive_DeathBeam");
            yield return new WaitForSeconds(3);

            UIManager.instance.filterPanel.gameObject.SetActive(false);
            chroma.intensity.value = 0;
            for(int i=0;i <5; i++)
                player.gameObject.GetComponent<PlayerHP>().ChangeStateCanDmg(true);
                player.gameObject.GetComponent<PlayerHP>().GetDmg(99999,"",0,"이블아이");
            anim.Play("Hive_Checking");
            StopAllCoroutines();
        }
    }

    // 반피가 됐을때 무적상황이 되면서 쫄몹소환함
    // 패턴 시작할때 StartCoroutine(Summon())
    IEnumerator Summon()
    {
        hiveState = HiveState.untouchable;
        anim.Play("Hive_GoRest");

        for(int i=0; i<5; i++)
        {
            eyeMob[i].SetActive(true);
            yield return new WaitForSeconds(0.5f);
            if(i==2) anim.Play("Hive_Resting");
        }

        while(hiveState is HiveState.untouchable)
        {
            if(mobDeadCount >= 5) 
            {
                hiveState = HiveState.idle;
                StartCoroutine(ChoicePattern());
            }
            hp += 5;
            hpImg.fillAmount = (float)hp / (float)hpMax;
            yield return oneSecond;
        }
        anim.Play("Hive_Checking");
    }

    // 빨개지는 애니메이션 후에 플레이어 위치로 폭탄 다섯번 생성함
    // 패턴 시작할때 StartCoroutine(Bomb());
    IEnumerator Bomb()
    {
        bombOn = true;
        hiveState = HiveState.bomb;
        int count =0;
        anim.Play("Hive_Bomb");

        yield return oneSecond;

        for(int i=0; i<5; i++)
        {
            bomb[count++].SetActive(true);
            if(count >= bomb.Length) count=0;
            yield return oneSecond;
        }

        bombOn = false;
        if(hiveState is not HiveState.untouchable) 
        {
            hiveState = HiveState.idle;
            anim.Play("Hive_Checking");
            StartCoroutine(ChoicePattern());
        }
    }

    // 잠시 눈을감는 애니메이션 후에 눈을뜸 그때 플레이어가 바라보고 있으면 멘탈+30
    // 패턴 시작할때 StartCoroutine(EyeOn());
    IEnumerator EyeOn()
    {
        eyeOn = true;
        hiveState = HiveState.eye;
        anim.Play("Hive_CloseEye");
        yield return new WaitForSeconds(1.5f);

        isViewing = !playerSprite.flipX ? true : false;
        if (eyeOn && isViewing) mental+=30;
        yield return oneSecond;

        eyeOn = false;
        if (hiveState is not HiveState.untouchable)
        {
            hiveState = HiveState.idle;
            anim.Play("Hive_Checking");
            StartCoroutine(ChoicePattern());
        }
    }

    // 잠시동안 준비시간을 갖고 왼쪽으로 가시를 랜덤하게 발사함
    // 패턴 시작할때 StartCoroutine(Thorn());
    IEnumerator Thorn()
    {
        thornOn = true;
        hiveState = HiveState.thorn;
        int count = 0;

        anim.Play("Hive_NiddleShot");
        yield return oneSecond;

        for (int i = 0; i < 20; i++)
        {
            thorn[count++].SetActive(true);
            if (count >= thorn.Length) count = 0;
            yield return new WaitForSeconds(0.2f);
        }

        anim.Play("Hive_NiddleShotEnd");
        yield return oneSecond;

        thornOn = false;
        if (hiveState is not HiveState.untouchable)
        {
            hiveState = HiveState.idle;
            anim.Play("Hive_Checking");
            StartCoroutine(ChoicePattern());
        }
    }
    
    // 맞을때 피가깎임, 50퍼센트 되면 무적상태 되면서 쫄몹소환 패턴, 피회복
    public void GetDmg(int dmg)
    {
        if(hiveState == HiveState.untouchable) { audioSource.PlayOneShot(avoidClip); return; }
        
        int random = Random.Range(0, 100);
        if (random < critical) dmg = (int)(dmg * 1.5f);

        hp -= dmg;
        hpImg.fillAmount = (float)hp / (float)hpMax;

        if(hpImg.fillAmount <= 0.5f && !hadUntouchable) 
        {
            hadUntouchable = true;
            StopAllCoroutines();
            StartCoroutine(Idle());
            StartCoroutine(Summon());
        }
        
        if(hp<=0)
        {
            StopAllCoroutines();
            mental=0;
            hiveState = HiveState.die;
            wall.SetActive(false);
            anim.Play("Hive_Die");
            StartCoroutine(Die());
        }
    }

    IEnumerator Die()
    {
        yield return new WaitForSeconds(2);
        mapSize.localScale = new Vector3(34.5f, 10.5f, 1);
        mapSize.localPosition = new Vector3(7.5f, 0.5f, 0);
        mapSize.sizeDelta = new Vector2(34.5f, 10.5f);
        GameObject.FindWithTag("MainCamera").GetComponent<FollowCamera>().SetViewSize();
        gameObject.SetActive(false);
    }
}
