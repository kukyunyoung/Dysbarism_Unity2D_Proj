using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Hive_EyeMob : MonoBehaviour
{
    [SerializeField] GameObject niddle;
    [SerializeField] TextMeshPro dmgText;
    [SerializeField] GameObject hpBack;
    [SerializeField] Image hpImg;
    [SerializeField] AudioClip hitSound;
    [SerializeField] AudioClip dieSound;

    Hive hive;
    Animator anim;
    GameObject player;
    AudioSource audioSource;
    SpriteRenderer sprite;
    Rigidbody2D rig;
    WaitForSeconds oneSecond = new WaitForSeconds(1);

    Vector3 dir;

    float hp;
    float hpMax;
    float speed;
    float niddleTime=0;
    float minX, minY, maxX, maxY;
    float critical;
    bool isDie;
    bool isPattern;
    bool isWall;

    private void Start() 
    {
        hp = 75;
        hpMax = 75;
        speed =1;

        minX = hive.mapSize.transform.localPosition.x - hive.mapSize.rect.width / 2;
        maxX = hive.mapSize.transform.localPosition.x + hive.mapSize.rect.width / 2;
        minY = hive.mapSize.transform.localPosition.y - hive.mapSize.rect.height / 2;
        maxY = hive.mapSize.transform.localPosition.y - hive.mapSize.rect.height / 2;
        print(minX + " " + maxX + " " + minY + " " + maxY);
    }

    private void FixedUpdate() 
    {
        if(Time.time > niddleTime + 8 && !isPattern) StartCoroutine(NIddleShot());
        hpBack.transform.position = transform.position;
    }

    private void OnEnable() 
    {
        hive = transform.parent.parent.parent.gameObject.GetComponent<Hive>();
        player = PlayerMove.instance.gameObject;
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        sprite = GetComponent<SpriteRenderer>();
        rig = GetComponent<Rigidbody2D>();
        critical = PlayerMove.instance.gameObject.GetComponent<PlayerStatus>().playerStatus[2];

        StartCoroutine(Summoned());
    }

    private void OnDisable() 
    {
        hive.mobDeadCount ++;
    }

    IEnumerator Summoned()
    {
        isPattern = true;
        anim.Play("Eye_Onenable");
        yield return oneSecond;
        isPattern = false;
        StartCoroutine(Floating());
    }

    IEnumerator NIddleShot()
    {
        isPattern = true;
        anim.Play("Eye_niddle");
        Vector3 target = (player.transform.position - transform.position).normalized;
        niddle.transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg - 90, Vector3.forward);
        niddle.transform.position = transform.position;
        yield return oneSecond;

        transform.localRotation = Quaternion.AngleAxis(Mathf.Atan2(target.y, target.x), Vector3.forward);
        niddle.SetActive(true);

        Vector3 startPos = transform.position;
        Vector3 targetPos = player.transform.position;
        float startTime = Time.time;
        float duration = 2;

        while (true)
        {
            niddle.transform.position = Vector3.Lerp(startPos, targetPos, Easing.SingleAxisBezier.CubicBezier(Easing.Preset.SlowInFastOut, Time.time - startTime, duration));

            if (Time.time - startTime > duration)
            {
                break;
            }

            yield return new WaitForEndOfFrame();
        }

        anim.Play("Eye_Idle");
        niddleTime = Time.time;
        isPattern = false;
        StartCoroutine(Floating());
    }

    public void GetDmg(int dmg, float knockBack = 0)
    {
        int random = Random.Range(0, 100);
        if (random < critical) dmg = (int)(dmg * 1.5f);

        hp -= dmg;
        hpImg.fillAmount = (float)hp / (float)hpMax;
        audioSource.PlayOneShot(hitSound);

        TextMeshPro damage = Instantiate(dmgText);
        damage.transform.position = new Vector3(transform.position.x, transform.position.y + 0.2f, -4);
        damage.GetComponent<DamageText>().damage = dmg.ToString();

        if (hp <= 0 && !isDie)
        {
            rig = null;
            StopAllCoroutines();
            isDie = true;
            hpBack.gameObject.SetActive(false);
            audioSource.PlayOneShot(dieSound);
            anim.Play("Eye_Die");
            StartCoroutine(Die());
        }
    }

    IEnumerator Die()
    {
        yield return new WaitForSeconds(2);
        gameObject.SetActive(false);
    }

    IEnumerator Floating()
    {
        while (!isDie && !isPattern) 
        {
            if(transform.localPosition.x > maxX ||
               transform.localPosition.x < minX ||
               transform.localPosition.y > maxY ||
               transform.localPosition.y < minY) transform.position = hive.transform.position;

            dir = UnityEngine.Random.insideUnitCircle.normalized;
            sprite.flipX = (dir.x > 0) ? false : true;
            sprite.flipY =  false;

            Vector3 startPos = transform.position;
            Vector3 targetPos = startPos + dir * 3;
            float startTime = Time.time;
            float duration = 3;

            while (true)
            {
                transform.position = Vector3.Lerp(startPos, targetPos, Easing.SingleAxisBezier.CubicBezier(Easing.Preset.SlowInSlowOut, Time.time - startTime, duration));

                if (Time.time - startTime > duration || isWall)
                {
                    break;
                }

                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForSeconds(2);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.transform.CompareTag("Player"))
        {
            player.GetComponent<PlayerHP>().GetDmg(10,"",0,"이블아이의 눈");
        }
        else if (other.transform.CompareTag("Floor"))
        {
            if(!isWall) StartCoroutine(IsWallFalse());
        }
    }

    IEnumerator IsWallFalse()
    {
        isWall = true;
        yield return new WaitForSeconds(2);
        isWall = false;
    }
}
