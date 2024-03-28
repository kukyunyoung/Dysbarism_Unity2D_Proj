using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Bubble : MonoBehaviour
{
    GameObject player;
    Vector3 target;

    private void OnEnable() 
    {
        player = transform.parent.parent.GetChild(0).GetComponent<Crab>().player;
        target = player.transform.position + (Vector3)Random.insideUnitCircle;
        StartCoroutine(ActiveTime());
        StartCoroutine(Trace());
    }

    private void OnDisable() 
    {
        transform.position = transform.parent.position;
    }

    IEnumerator Trace()
    {
        Vector3 startPos = transform.position;

        float startTime = Time.time;
        float duration = 3f;

        while (true)
        {
            transform.position = Vector3.Lerp(startPos, target, Easing.SingleAxisBezier.CubicBezier(Easing.Preset.SlowInSlowOut, Time.time - startTime, duration));

            if (Time.time - startTime > duration)
            {
                break;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator ActiveTime()
    {
        yield return new WaitForSeconds(3.5f);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.transform.CompareTag("Player"))
        {
            player.GetComponent<PlayerHP>().GetDmg(5, "Poison", 50, "거대 소라게");
            gameObject.SetActive(false);
        }
    }
}
