using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGif : MonoBehaviour
{
    [SerializeField] Sprite[] gif;
    
    WaitForSeconds frame = new WaitForSeconds(0.1f);
    Image img;
    int i=0;

    private void OnEnable() 
    {
        img = gameObject.GetComponent<Image>();
        StartCoroutine(Gif());
    }

    private void OnDisable() 
    {
        StopCoroutine(Gif());
    }

    IEnumerator Gif()
    {
        while(true)
        {
            img.sprite = gif[i];
            i++;
            if(i>=gif.Length) i=0;
            yield return frame;
        }
    }
}
