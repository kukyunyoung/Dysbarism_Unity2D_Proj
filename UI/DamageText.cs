using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/* https://m.blog.naver.com/liberess/222256450964 */

public class DamageText : MonoBehaviour
{
    float minFontSize;
    float sizeChangeSpeed;

    float moveSpeed = 0.15f;
    float alphaSpeed = 1.5f;
    public float destroyTime = 0.4f;

    float time;
    public string damage;

    Color alpha;
    TextMeshPro text;

    private void Awake()
    {
        minFontSize = 2f;
    }

    void Start()
    {
        time = 0;

        text = GetComponent<TextMeshPro>();
        text.text = damage;
        alpha = text.color;

        Destroy(gameObject, destroyTime);
    }

    void Update()
    {
        transform.Translate(new Vector3(0, moveSpeed * Time.deltaTime, 0));

        if(time < 0.2f)
        {
            text.fontSize += Time.deltaTime * sizeChangeSpeed;
        }
        else
        {
            if (!(text.fontSize <= minFontSize))
            {
                text.fontSize -= Time.deltaTime * sizeChangeSpeed;
            }
        }

        time += Time.deltaTime * sizeChangeSpeed;

        alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * alphaSpeed);
        text.color = alpha;
    }
}
