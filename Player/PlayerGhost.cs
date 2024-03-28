using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGhost : MonoBehaviour
{
    private void OnEnable() 
    {
        StartCoroutine(HideTime());   
    }

    private void OnDisable() 
    {
        transform.position= Vector3.zero;
    }

    IEnumerator HideTime()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }
}
