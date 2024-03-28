using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiveBomb : MonoBehaviour
{
    PlayerHP player;
    GameObject Hive;

    private void OnEnable() 
    {
        player = PlayerMove.instance.gameObject.GetComponent<PlayerHP>();
        Hive = transform.parent.parent.parent.gameObject;

        transform.localPosition = player.gameObject.transform.localPosition - Hive.transform.position;
        StartCoroutine(Boom());
    }

    private void OnDisable() 
    {
        transform.localPosition = Vector3.zero;
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.transform.CompareTag("Player"))
        {
            player.GetDmg(20,"",0, "이블아이");
        }
    }

    IEnumerator Boom()
    {
        yield return new WaitForSeconds(2);
        gameObject.SetActive(false);
    }
}