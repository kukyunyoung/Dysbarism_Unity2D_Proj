using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Transform gunFirePos;
    public float angle;
    bool isHit = false;
    int dmg = 20;

    private void OnEnable()
    {
        StartCoroutine(DestroyTime());
        StartCoroutine(Shot(angle, 0.1f));
        isHit=false;
        transform.position = gunFirePos.position;
        dmg = (int)((PlayerMove.instance.gameObject.GetComponent<PlayerStatus>().playerStatus[0] * 0.01f + 1) * 20);
    }

    IEnumerator DestroyTime()
    {
        yield return new WaitForSeconds(1);
        gameObject.SetActive(false);
    }

    public IEnumerator Shot(float angle, float bulletSpeed)
    {
        int randAngle = Random.Range(-7, 8);
        transform.rotation = Quaternion.AngleAxis(angle - randAngle - 90, Vector3.forward);
        for(int i=0; i< 150; i++)
        {
            transform.Translate(Vector2.up.normalized  * bulletSpeed * Time.deltaTime * 150);
            yield return new WaitForEndOfFrame();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("TraceRange")) return;

        if (collision.transform.CompareTag("Enemy"))
        {
            if(isHit) return;
            isHit = true;
            Fish enemy = collision.gameObject.GetComponent<Fish>();
            enemy.isTrace = true;
            enemy.GetDmg(dmg , 0.05f);
            StopCoroutine(Shot(0, 0));
            gameObject.SetActive(false);
        }

        else if (collision.transform.CompareTag("Crab"))
        {
            if (isHit) return;
            isHit = true;
            Crab enemy = collision.gameObject.GetComponent<Crab>();
            enemy.GetDmg(dmg, 0);
            StopCoroutine(Shot(0, 0));
            gameObject.SetActive(false);
        }

        else if (collision.transform.CompareTag("Hive"))
        {
            if (isHit) return;
            isHit = true;
            Hive enemy = collision.gameObject.GetComponent<Hive>();
            enemy.GetDmg(dmg);
            StopCoroutine(Shot(0, 0));
            gameObject.SetActive(false);
        }

        else if (collision.transform.CompareTag("Eye"))
        {
            if (isHit) return;
            isHit = true;
            Hive_EyeMob enemy = collision.gameObject.GetComponent<Hive_EyeMob>();
            enemy.GetDmg(dmg);
            StopCoroutine(Shot(0, 0));
            gameObject.SetActive(false);
        }
    }
}
