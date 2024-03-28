using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BlowFish : Fish
{
    [SerializeField] GameObject blowFishSpear;
    Vector3 nowPos;
    bool isBoom = false;

    void Update()
    {
        if (UIManager.instance.stopTime!=0) return;
        hpBack.transform.position = transform.position;
        if (!isTrace) StartCoroutine(Move());
        else { anim.SetBool("Trace", true); Trace();}
    }

    protected override void Attack(Collision2D collision)
    {
        if(collision.transform.CompareTag("Player"))
        {
            playerHp = collision.gameObject.GetComponent<PlayerHP>();
            StartCoroutine(Boom());
        }
    }

    protected override void PlayDieAnim()
    {
        anim.SetTrigger("Die");
    }

    protected override void SetStatus()
    {
        hp = 300;
        hpMax = 300;
        dmg = 20;
        speed = 1;
        attDelay = 1;
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if(other.gameObject.CompareTag("Player"))
        {
            anim.SetBool("Trace",false);
            isTrace = false;
        }
    }

    override public void Die()
    {
        if (isDie)
        {
            nowPos = transform.position;
            print("Blowfish Die");
            Reward();
            if(rig != null) rig.velocity = Vector2.zero;
            rig = null;
            GetComponent<CapsuleCollider2D>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
            StartCoroutine(Boom());
        }
    }

    IEnumerator Boom()
    {
        if(!isBoom)
        {
            isBoom = true;
            speed = 0;
            PlayDieAnim();

            for (int i=0; i<4; i++)
            {
                sprite.color = new Color(0.5f, 0.2f, 0.8f, 1); // 보라색
                yield return new WaitForSeconds(0.2f);
                sprite.color = new Color(1,1,1,1);
                yield return new WaitForSeconds(0.2f);
            }

            blowFishSpear.SetActive(true);
            anim.SetTrigger("Boom");

            yield return new WaitForSeconds(1.2f);
            hpBack.gameObject.SetActive(false);
            transform.parent.gameObject.SetActive(false);
        }
    }

    public new void Trace()
    {
        if (!isDie)
        {
            dir = player.transform.position - transform.position;
            sprite.flipX = (dir.x>0) ?true:false;
            sprite.flipY = false;
            //rig.rotation = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            rig.position += (Vector2)dir * speed * Time.deltaTime;
        }
        else
        {
            if(rig != null) rig.rotation = 0;
        }
    }

    public new IEnumerator Move()
    {
        if (!isMove && !isDie)
        {
            isMove = true;
            int random = Random.Range(-1, 2); // 방향전환을 위한 변수
            random = (random == 0) ? 1 : -1;
            sprite.flipX = (random == 1) ? true : false;
            Vector2 dir = new Vector2(random, 0);

            if (isDie) rig.velocity = Vector2.zero;
            else rig.velocity += dir.normalized * speed;
            yield return new WaitForSeconds(2);
            if(rig != null) rig.velocity = Vector2.zero;
            yield return new WaitForSeconds(1);

            isMove = false;
        }
    }
}
