using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SystemMessage : MonoBehaviour
{
    Vector3 upPos = new Vector3(0,100,0);

    private void OnEnable() 
    {
        StartCoroutine(PrintSystemMessage());
    }

    private void OnDisable() 
    {
        transform.GetComponent<Text>().text = "";
        transform.position = transform.parent.position;
    }

    IEnumerator PrintSystemMessage()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + upPos;

        float startTime = Time.time;
        float duration = 0.6f;

        while (true)
        {
            transform.position = Vector3.Lerp(startPos, endPos, Easing.SingleAxisBezier.CubicBezier(Easing.Preset.FastInSlowOut, Time.time - startTime, duration));

            if (Time.time - startTime > duration)
            {
                break;
            }

            yield return new WaitForEndOfFrame();
        }

        gameObject.SetActive(false);
    }

}
