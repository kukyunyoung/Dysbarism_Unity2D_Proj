using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyFish : Fish
{
    Vector2 up = new Vector2(0, 1.5f);
    WaitForSeconds wfs = new WaitForSeconds(1);

    private void Update() 
    {
        if (UIManager.instance.stopTime!=0) return;
        
        hpBack.transform.position = transform.position;
        if (!isTrace) StartCoroutine(Move());
        else {anim.SetBool("Trace", true); Trace();}
    }

    new IEnumerator Move()
    {
        if(!isMove && !isDie)
        {
            isMove = true;
            
            Vector3 startPos = transform.position;
            Vector3 endPos = (Vector2)transform.position + up;
            float startTime = Time.time;
            float duration = 3;

            while (true)
            {
                transform.position = Vector3.Lerp(startPos, endPos, Easing.SingleAxisBezier.CubicBezier(Easing.Preset.SlowInSlowOut, Time.time - startTime, duration));
                if (Time.time - startTime > duration || isTrace) break;

                yield return new WaitForEndOfFrame();
            }

            up *= -1;
            yield return new WaitForSeconds(1);

            isMove = false;
        }
    }

    protected override void Attack(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player") && !isDie)
        {
            playerHp = collision.gameObject.GetComponent<PlayerHP>();
            anim.SetTrigger("Attack");
            playerHp.GetDmg(dmg, "Electric", 50, "전기해파리");
        }
    }

    protected override void PlayDieAnim()
    {
        GetComponent<CapsuleCollider2D>().enabled = false;
        anim.SetTrigger("Die");
        StartCoroutine(ActiveFalse());
    }

    protected override void SetStatus()
    {
        hp = 75;
        hpMax = 75;
        dmg = 10;
        speed = 1;
        attDelay = 2;
    }
}
