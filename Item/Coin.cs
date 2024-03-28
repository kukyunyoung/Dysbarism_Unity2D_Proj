using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Coin : MonoBehaviour
{
    GameObject player;
    Rigidbody2D rig;

    Vector3 endPos;

    public static int coinNum;

    void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rig = GetComponent<Rigidbody2D>();
        StartCoroutine(SpreadCoinRoutine());
    }

    private void Update()
    {
        endPos = player.transform.position;
    }

    IEnumerator SpreadCoinRoutine()
    {
        
        float dirX = Random.Range(-1f, 1f);
        float dirY = Random.Range(-1f, 1f);
        Vector2 dir = new Vector2 (dirX, dirY);

        rig.AddForce(dir, ForceMode2D.Impulse);

        yield return new WaitForSeconds(1.5f);

        Vector3 startPos = transform.position;
        
        float startTime = Time.time;
        float duration = 0.6f;

        while(true)
        {
            transform.position = Vector3.Lerp(startPos, endPos, Easing.SingleAxisBezier.CubicBezier(Easing.Preset.FastInSlowOut, Time.time - startTime, duration));

            if (Time.time - startTime > duration)
            {
                break;
            }

            yield return new WaitForEndOfFrame();
        }

        transform.position = new Vector3(0, 0, 0);
        UIManager.coin++;
        gameObject.SetActive(false);
    }
}
