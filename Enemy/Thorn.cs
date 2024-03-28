using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thorn : MonoBehaviour
{
    public float bulletSpeed = 3;
    PlayerHP player;

    private void OnEnable() 
    {
        int randomAngle = Random.Range(30, 91);
        transform.localRotation = Quaternion.AngleAxis(randomAngle, Vector3.forward);
        StartCoroutine(Shot());
    }

    private void OnDisable() 
    {
        transform.localPosition = Vector3.zero;
    }

    void Start()
    {
        player = PlayerMove.instance.gameObject.GetComponent<PlayerHP>();
    }

    IEnumerator Shot()
    {
        for (int i = 0; i < 200; i++)
        {
            transform.Translate(Vector2.up.normalized * bulletSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.transform.CompareTag("Player"))
        {
            player.GetDmg(5, "Blood", 10, "이블아이");
            gameObject.SetActive(false);
        }
    }
}
