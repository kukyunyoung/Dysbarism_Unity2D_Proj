using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 추상메소드
/// Attack(Collision collision);
/// PlayDieAnim();
/// SetStatus();
/// </summary>
/// Move() : idle 상태일때 좌우 이동하는 메소드 (코루틴)
/// Reward() : 몬스터 사망시 재화 생성하는 메소드
/// Die() : 사망하는 메소드 (코루틴)
/// GetDmg(int dmg, float knockBack = 0) : 데미지 받는 메소드, 넉백수준에 따라 뒤로 물러남, 플레이어의 무기에 OnCollisionEnter으로 가져와서 사용함
/// Trace() : 플레이어를 쫓아가는 메소드

public abstract class Fish : MonoBehaviour
{
    [SerializeField] AudioClip hitSound;
    [SerializeField] AudioClip dieSound;
    [SerializeField] GameObject[] coinReward;
    [SerializeField] GameObject[] ItemReward;
    [SerializeField] protected RectTransform hpBack; 
    [SerializeField] Image hpImg;            
    [SerializeField] TextMeshPro dmgText;

    protected int hp;
    protected int hpMax;
    protected int dmg;
    protected float speed;
    protected float attDelay;

    public bool isTrace;
    protected bool isMove;
    public bool isDie;

    protected Vector2 dir;

    protected Animator anim;
    AudioSource audioSource;

    protected Rigidbody2D rig;
    protected SpriteRenderer sprite;

    protected GameObject player;
    protected PlayerHP playerHp;
    float critical;

    private void OnEnable() 
    {
        for(int i=0;i<coinReward.Length; i++)
        {
            coinReward[i] = ObjPullingManager.instance.transform.GetChild(0).GetChild(i).gameObject;
            critical = PlayerMove.instance.gameObject.GetComponent<PlayerStatus>().playerStatus[2];
        }
    }

    void Start()
    {
        SetStatus();

        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        rig = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        player = PlayerMove.instance.gameObject;
        playerHp = player.GetComponent<PlayerHP>();
    }

    void Update()
    {
        if (UIManager.instance.stopTime!=0) return;

        hpBack.transform.position = transform.position;

        if (!isTrace) StartCoroutine(Move());
        else Trace();
    }

    public IEnumerator Move()
    {
        if (!isMove && !isDie)
        {
            isMove = true;
            int random = Random.Range(-1, 2); // 방향전환을 위한 변수
            random = (random == 0) ? 1 : -1;
            sprite.flipX = (random == 1) ? false : true;
            Vector2 dir = new Vector2(random, 0);

            if (isDie) rig.velocity = Vector2.zero;
            else rig.velocity += dir * speed;
            yield return new WaitForSeconds(2);
            rig.velocity = Vector2.zero;
            yield return new WaitForSeconds(1);

            isMove = false;
        }
    }

    protected void Reward()
    {
        int coinNum = Random.Range(1, 4);
        for (int i = 0; i < coinNum; i++)
        {
            coinReward[Coin.coinNum].transform.position = transform.position;
            coinReward[Coin.coinNum].gameObject.SetActive(true);
            if(Coin.coinNum >= 10) Coin.coinNum = 0;
        }
        switch(gameObject.name)
        {
            case "Piranha":
                UIManager.instance.piranha ++;
                break;
            case "Shark":
                UIManager.instance.shark++;
                break;
            case "JellyFish":
                UIManager.instance.jellyfish++;
                break;
            case "Octopus":
                UIManager.instance.octopus++;
                break;
            case "BlowFish":
                UIManager.instance.blowfish++;
                break;
        }
    }

    public virtual void Die()
    {
        if (isDie)
        {
            print("FishDie");
            Reward();
            hpBack.gameObject.SetActive(false);
            GetComponent<BoxCollider2D>().enabled = false;
            PlayDieAnim();
        }
    }

    protected IEnumerator ActiveFalse()
    {
        yield return new WaitForSeconds(2);
        transform.parent.gameObject.SetActive(false);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Attack(collision);
    }

    public void GetDmg(int dmg, float knockBack = 0)
    {
        int random = Random.Range(0,100);
        if(random < critical) dmg =(int)(dmg * 1.5f);
        
        hp -= dmg;
        hpImg.fillAmount = (float)hp / (float)hpMax;
        audioSource.PlayOneShot(hitSound);

        TextMeshPro damage = Instantiate(dmgText);
        damage.transform.position = new Vector3(transform.position.x, transform.position.y + 0.4f, -6);
        damage.GetComponent<DamageText>().damage = dmg.ToString();

        if (hp <= 0 && !isDie)
        {
            //rig = null;
            isDie = true;
            hpBack.gameObject.SetActive(false);
            audioSource.PlayOneShot(dieSound);
            Die();
        }
        else
        {
            StartCoroutine(KnockBackRoutine(knockBack));
        }
    }

    IEnumerator KnockBackRoutine(float knockBack)
    {
        if(!isDie && gameObject.name!="BlowFish")
        {
            rig.velocity -= dir.normalized * knockBack;
            yield return new WaitForSeconds(1);
            if(rig != null) rig.velocity = Vector2.zero;
        }
    }

    public void Trace()
    {
        if (!isDie)
        {
            dir = player.transform.position - transform.position;
            sprite.flipX = false;
            sprite.flipY = (dir.x > 0) ? false : true;
            rig.rotation = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            rig.position += (Vector2)dir.normalized * speed * Time.deltaTime;
        }
        else sprite.flipY = false;
    }

    protected abstract void PlayDieAnim();
    protected abstract void SetStatus();
    protected abstract void Attack(Collision2D collision);
}
