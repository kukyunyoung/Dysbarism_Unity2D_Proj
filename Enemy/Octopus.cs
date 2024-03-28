using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Octopus : Fish
{
    bool isDash = false;

    void Update()
    {
        if (UIManager.instance.stopTime!=0) return;
        
        hpBack.transform.position = transform.position;
        if (!isTrace) StartCoroutine(Move());
        else StartCoroutine(Trace());    
    }

    protected override void Attack(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player") && !isDie)
        {
            playerHp = collision.gameObject.GetComponent<PlayerHP>();
            playerHp.GetDmg(dmg,"",0,"문어");
        }
    }

    protected override void PlayDieAnim()
    {
        anim.SetTrigger("Die");
        GetComponent<CapsuleCollider2D>().enabled = false;
        StartCoroutine(ActiveFalse());
    }

    new IEnumerator Trace()
    {
        if (!isDie && !isDash)
        {
            isDash = true;
            yield return new WaitForSeconds(0.5f);
            if(isDie) StopCoroutine(Trace());
            dir = player.transform.position - transform.position;
            rig.rotation = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg; // 방향바꾸기

            anim.SetTrigger("Charge");
            yield return new WaitForSeconds(1.5f);
            if (isDie) StopCoroutine(Trace());

            dir = player.transform.position - transform.position;
            sprite.flipX = true;
            sprite.flipY = (dir.x > 0) ? false : true;
            anim.SetTrigger("Attack");
            rig.rotation = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            Vector3 startPos = transform.position;
            Vector3 endPos = player.transform.position;
            float startTime = Time.time;
            float duration = 1;

            while (true)
            {
                transform.position = Vector3.Lerp(startPos, endPos, Easing.SingleAxisBezier.CubicBezier(Easing.Preset.FastInSlowOut, Time.time - startTime, duration));
                if (Time.time - startTime > duration || isDie) break;

                yield return new WaitForEndOfFrame();
            }
            anim.SetTrigger("AttackFinish");
            
            isDash = false;
            isTrace = false;
        }
        else if(isDie)
        {
            rig.rotation = 0;
            isTrace = false;
            sprite.flipX = false;
            sprite.flipY = true;
        }
    }

    private void OnTriggerStay2D(Collider2D other) 
    {
        if(other.transform.CompareTag("Player")) isTrace = true;
    }

    protected override void SetStatus()
    {
        hp = 100;
        hpMax = 100;
        dmg = 20;
        speed = 1;
        attDelay = 3;
    }
}
