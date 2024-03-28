using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeThorn : MonoBehaviour
{
    private void OnEnable() 
    {
        StartCoroutine(Die());
    }

    IEnumerator Die()
    {
        yield return new WaitForSeconds(2.5f);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Player"))
        {
            PlayerMove.instance.gameObject.GetComponent<PlayerHP>().GetDmg(5, "Blood", 30, "이블아이의 눈");
            StopAllCoroutines();
            gameObject.SetActive(false);
        }
    }
}
