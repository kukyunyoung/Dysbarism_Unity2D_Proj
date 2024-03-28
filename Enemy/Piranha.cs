using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piranha : Fish
{
    protected override void PlayDieAnim()
    {
        GetComponent<CircleCollider2D>().enabled = false;
        sprite.flipY = true;
        StartCoroutine(Die_UpAnim());
    }

    IEnumerator Die_UpAnim()
    {
        sprite.color = Color.red;
        UIManager.instance.piranha++;
        rig.rotation = Mathf.Atan2(-1, 0) * Mathf.Rad2Deg;
        Vector3 up = new Vector3(0, 0.01f);
        for (int i = 0; i < 100; i++)
        {
            transform.position += up;
            yield return new WaitForSeconds(0.02f);
        }
        transform.parent.gameObject.SetActive(false);
    }

    protected override void Attack(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player") && !isDie)
        {
            playerHp = collision.gameObject.GetComponent<PlayerHP>();
            playerHp.GetDmg(dmg, "Blood", 20, "피라냐");
        }
    }

    protected override void SetStatus()
    {
        hp = 100;
        hpMax = 100;
        dmg = 10;
        speed = 2.5f;
        attDelay = 2;
    }
}
