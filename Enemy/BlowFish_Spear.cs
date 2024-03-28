using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlowFish_Spear : MonoBehaviour
{
    WaitForSeconds wfs = new WaitForSeconds(0.01f);

    private void OnEnable() 
    {
        print("niddle on");
        StartCoroutine(Shot());
    }

    IEnumerator Shot()
    {
        float angle = transform.localEulerAngles.z;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        for(int i=0; i<100; i++)
        {
            transform.Translate(Vector2.up.normalized * Time.deltaTime * 5);
            yield return wfs;
        }
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.transform.CompareTag("Player"))
        {
            PlayerHP player = other.gameObject.GetComponent<PlayerHP>();
            player.GetDmg(10, "Poison", 100, "복어");
            gameObject.SetActive(false);
        }
    }
}
