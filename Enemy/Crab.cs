using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Crab : MonoBehaviour
{
    public enum CrabState {idle, attack, guard, die, niddle, hand, ground, bubble}
    public CrabState crabState = CrabState.idle;

    [SerializeField] TextMeshPro dmgText;
    [SerializeField] GameObject hpBack;
    [SerializeField] Image hpImg;
    [Space]
    [SerializeField] AudioClip hitSound;
    [SerializeField] AudioClip guardSound;
    [SerializeField] AudioClip dieSound;
    [Space]
    [SerializeField] GameObject hitCol;
    [Space]
    [SerializeField] GameObject[] bubble;
    [SerializeField] FollowCamera cam;
    [Space]
    [SerializeField] RectTransform mapSize;

    public GameObject player;
    public Animator anim;
    AudioSource audioSource;
    SpriteRenderer sprite;
    IEnumerator attack;

    float hp;
    float hpMax;
    float critical;
    bool isDie;
    bool isDelay;
    public bool isAttack;
    public bool isGround;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        player = GameObject.FindWithTag("Player");
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        cam = GameObject.FindWithTag("MainCamera").GetComponent<FollowCamera>();

        InitStatus();
    }

    private void FixedUpdate() 
    {
        if(isGround) StartCoroutine(cam.Shake());

        if(crabState == CrabState.attack && !isAttack)
        {
            isAttack = true;
            if(attack!= null) StartCoroutine(attack);
            attack = null;
        }
    }

    private void OnEnable() 
    {
        critical = PlayerMove.instance.gameObject.GetComponent<PlayerStatus>().playerStatus[2];
    }

    void InitStatus()
    {
        hp = 1000;
        hpMax = 1000;
        crabState = CrabState.guard;
        anim.SetBool("Guard", true);
    }

    public IEnumerator ChoiceAttack()
    {
        if(!isAttack)
        {
            isAttack = true;
            int random = Random.Range(0,4);
            print("pattern num : " + random);
            switch(random)
            {
                case 0:
                    attack = NiddleAttack();
                    break;
                case 1:
                    attack = HandAttack();
                    break;
                case 2:
                    attack = GroundAttack();
                    break;
                case 3:
                    attack = BubbleAttack();
                    break;
            }
            yield return new WaitForSeconds(2);
            anim.SetTrigger("BeforeAttack");

            while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1) {yield return new WaitForEndOfFrame(); print("choice11");}

            crabState = CrabState.attack;
            isAttack = false;
        }
    }

    IEnumerator NiddleAttack()
    {
        crabState = CrabState.niddle;
        anim.SetBool("Niddle", true);

        while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1) yield return new WaitForEndOfFrame();
        isAttack = false;
        anim.SetBool("Niddle", false);
        crabState = CrabState.idle;
        StartCoroutine(ChoiceAttack());
    }

    IEnumerator HandAttack()
    {
        crabState = CrabState.hand;
        anim.SetBool("Hand",true);

        while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1) yield return new WaitForEndOfFrame();
        isAttack = false;
        anim.SetBool("Hand", false);
        crabState = CrabState.idle;
        StartCoroutine(ChoiceAttack());
    }

    IEnumerator GroundAttack()
    {
        crabState = CrabState.ground;
        anim.SetBool("Ground", true);

        while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1) yield return new WaitForEndOfFrame();
        isAttack = false;
        anim.SetBool("Ground", false);
        crabState = CrabState.idle;
        StartCoroutine(ChoiceAttack());
    }

    IEnumerator BubbleAttack()
    {
        crabState = CrabState.bubble;
        anim.SetBool("Bubble", true);
        yield return new WaitForSeconds(2.5f);

        for(int i=0;i<bubble.Length; i++)
        {
            bubble[i].SetActive(true);
            yield return new WaitForSeconds(0.3f);
        }
        isAttack = false;
        anim.SetBool("Bubble", false);
        crabState = CrabState.idle;
        StartCoroutine(ChoiceAttack());
    }

    public void GetDmg(int dmg, float knockBack = 0)
    {
        AudioClip hit;
        if (crabState == CrabState.guard) 
        {
            dmg = 1;
            hit = guardSound;
        }
        else hit = hitSound;

        int random = Random.Range(0, 100);
        if (random < critical) dmg = (int)(dmg * 1.5f);

        hp -= dmg;
        audioSource.PlayOneShot(hit);
        hpImg.fillAmount = (float)hp / (float)hpMax;

        TextMeshPro damage = Instantiate(dmgText);
        damage.transform.position = new Vector3(transform.position.x, transform.position.y + 1, -4);
        damage.GetComponent<DamageText>().damage = dmg.ToString();

        if (hp <= 0 && !isDie)
        {
            StopAllCoroutines();
            isDie = true;
            hpBack.gameObject.SetActive(false);
            audioSource.PlayOneShot(dieSound);
            StartCoroutine(Die());
        }
    }

    IEnumerator Die()
    {
        Color color = new Color(0.02f, 0.02f, 0.02f, 0);
        crabState = CrabState.die;
        anim.SetTrigger("Die");
        GetComponent<CapsuleCollider2D>().enabled = false;

        for(int i=0; i<30; i++)
        {
            sprite.color -= color;
            yield return new WaitForSeconds(0.04f);
        }

        yield return new WaitForSeconds(3f);
        mapSize.localScale = new Vector3(34.5f, 10.5f, 1);
        mapSize.localPosition = new Vector3(7.5f, 0.5f, 0);
        mapSize.sizeDelta = new Vector2(34.5f, 10.5f);
        GameObject.FindWithTag("MainCamera").GetComponent<FollowCamera>().SetViewSize();

        yield return new WaitForSeconds(3f);
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        transform.parent.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.transform.CompareTag("Player"))
        {
            if(anim.GetCurrentAnimatorStateInfo(0).IsName("Niddle") && !isDelay)
            {
                StartCoroutine(Delay());
                Vector3 dir = player.transform.position - gameObject.transform.position;
                player.GetComponent<PlayerHP>().GetDmg(20, "Blood", 30, "거대 소라게");
                player.GetComponent<Rigidbody2D>().AddForce(dir.normalized * 3, ForceMode2D.Impulse);
            }
            else if(anim.GetCurrentAnimatorStateInfo(0).IsName("Hand") && !isDelay)
            {
                StartCoroutine(Delay());
                player.GetComponent<PlayerHP>().GetDmg(10, "Blood", 30, "거대 소라게");
            }
            else if(anim.GetCurrentAnimatorStateInfo(0).IsName("Ground") && !isDelay)
            {
                StartCoroutine(Delay());
                player.GetComponent<PlayerHP>().GetDmg(50,"", 0, "거대 소라게");
                player.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 3, ForceMode2D.Impulse);
            }
        }
    }

    IEnumerator Delay()
    {
        isDelay = true;
        yield return new WaitForSeconds(3);
        isDelay = false;
    }
}
