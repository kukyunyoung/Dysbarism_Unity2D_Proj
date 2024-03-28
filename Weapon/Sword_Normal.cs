using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UIElements;

public class Sword_Normal : MonoBehaviour, IWeapon, IUseableItem
{
    //[SerializeField] GameObject pointer;
    [SerializeField] GameObject player;

    bool isAtt;
    bool isDelay;
    bool isEnemy;
    bool isEnemyTrigger;
    bool attCount;
    float attDelay = 1.5f;
    float angle;
    int dmg;

    SpriteRenderer playerSprite;
    SpriteRenderer sprite;

    void Start()
    {
        if(player==null) player = PlayerMove.instance.gameObject;
        playerSprite = player.GetComponent<SpriteRenderer>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if(UIManager.instance.stopTime!=0) return;
        if (!isEnemy) SwordAnim();
        if (isEnemy) if (!isEnemyTrigger) isEnemy = false;

        if (Input.GetMouseButtonDown(0)) Attack();
    }

    private void OnEnable() 
    {
        WeaponManager.bulletText.gameObject.SetActive(false);
        dmg = 30;
    }

    public void Attack()
    {
        attCount = !attCount;

        if (!isDelay)
        {
            isDelay = true;
            print("Attack!!");
            isAtt = true;
            StartCoroutine(AttDelay());
        }
    }

    public void Skill()
    {
        throw new System.NotImplementedException();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // �� Ž��
        if (collision.transform.CompareTag("Enemy"))
        {
            Fish enemy = collision.gameObject.GetComponent<Fish>();
            isEnemy = true;

            // ����
            if (isAtt)
            {
                print("tryAtt");
                enemy.GetDmg(dmg, 1);
                isAtt = false;
            }
        }

        else if (collision.transform.CompareTag("Crab"))
        {
            Crab enemy = collision.gameObject.GetComponent<Crab>();
            isEnemy = true;

            // ����
            if (isAtt)
            {
                print("tryAtt");
                enemy.GetDmg(dmg);
                isAtt = false;
            }
        }

        else if (collision.transform.CompareTag("Hive"))
        {
            Hive enemy = collision.gameObject.GetComponent<Hive>();
            isEnemy = true;

            // ����
            if (isAtt)
            {
                print("tryAtt");
                enemy.GetDmg(dmg);
                isAtt = false;
            }
        }

        else if (collision.transform.CompareTag("Eye"))
        {
            Hive_EyeMob enemy = collision.gameObject.GetComponent<Hive_EyeMob>();
            isEnemy = true;

            // ����
            if (isAtt)
            {
                print("tryAtt");
                enemy.GetDmg(dmg, 1);
                isAtt = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Enemy"))
        {
            print("Enemy null");
            isEnemy = false;
            isEnemyTrigger = false;
        }
    }


    IEnumerator AttDelay()
    {
        yield return new WaitForSeconds(0.1f);
        isAtt = false;
        yield return new WaitForSeconds(attDelay);
        isDelay = false;
    }

    void SwordAnim()
    {
        sprite.flipX = playerSprite.flipX;

        transform.localRotation = (playerSprite.flipX) ?
            ((attCount) ? Quaternion.Euler(0, 0, 75) : Quaternion.Euler(0, 0, 0)) :
            ((attCount) ? Quaternion.Euler(0, 0, -75) : Quaternion.Euler(0, 0, 0));
    }

    public bool Use()
    {
        throw new System.NotImplementedException();
    }
}
