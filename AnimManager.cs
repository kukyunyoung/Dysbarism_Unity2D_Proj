using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AnimManager : MonoBehaviour
{
    private void OnEnable() 
    {   
        StartCoroutine(DieTime());
    }

    IEnumerator DieTime()
    {
        yield return new WaitForSeconds(3);
        gameObject.SetActive(false);
    }
}
