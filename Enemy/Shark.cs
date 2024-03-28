using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Shark : Fish
{
    private void Update() 
    {
        if (UIManager.instance.stopTime!=0) return;
        if (isDie) return;
        
        hpBack.transform.position = transform.position;

        if (!isTrace) StartCoroutine(Move());
        else Trace();
        if (playerHp.charState == PlayerHP.CharState.blood) isTrace = true;
    }

    protected override void PlayDieAnim()
    {
        GetComponent<CapsuleCollider2D>().enabled = false;
        isTrace = false;
        sprite=null;
        anim.SetTrigger("Die");
        StartCoroutine(ActiveFalse());
    }

    protected override void Attack(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player") && !isDie)
        {
            anim.SetBool("Attack", true);
            playerHp = collision.gameObject.GetComponent<PlayerHP>();
            playerHp.GetDmg(dmg, "Blood", 20, "백상아리");
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if(other.transform.CompareTag("Player")) anim.SetBool("Attack", false);
    }

    protected override void SetStatus()
    {
        hp = 150;
        hpMax = 150;
        dmg = 25;
        speed = 3.3f;
        attDelay = 2;
    }
}
