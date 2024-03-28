using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeObj : MonoBehaviour
{
    Vector3 originPos;
    WaitForSeconds waitTime;

    void Start()
    {
        originPos = transform.localPosition;
        waitTime = new WaitForSeconds(0.02f);
    }

    public IEnumerator ShakeGaze(float amount, bool isCharge)
    {
        while (isCharge)
        {
            transform.localPosition = (Vector3)Random.insideUnitCircle * amount + originPos;

            yield return waitTime;
        }
        transform.localPosition = originPos;
    }
}
