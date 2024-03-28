using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Escape : MonoBehaviour
{
    [SerializeField] GameObject popUp;
    [SerializeField] GameObject mermaid;
    [SerializeField] GameObject mermaidPos;
    [SerializeField] GameObject bubble;

    bool isCall;
    Animator mermaidAnim;

    void Start()
    {
        isCall = false;
        mermaidAnim = mermaid.GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.transform.CompareTag("Player")) popUp.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if(other.transform.CompareTag("Player")) popUp.SetActive(false);
    }

    private void OnTriggerStay2D(Collider2D other) 
    {
        if(other.transform.CompareTag("Player"))
        {
            if(!isCall && Input.GetKey(KeyCode.E))
            {
                isCall = true;
                UIManager.instance.stopTime++;
                StartCoroutine(Mermaid());
            }
        }
    }

    IEnumerator Mermaid()
    {
        // UI 꺼지고 인어 등장해서 노래한다음 
        // 버블로 쌓여서 플레이어는 맵밖으로 탈출하고 
        // 결과UI띄우기
        for (int i = 0; i < UIManager.instance.fadeObj.Length; i++)
            UIManager.instance.fadeObj[i].SetActive(false);

        mermaid.SetActive(true);
        PlayerMove.instance.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;

        Vector3 startPos = mermaid.transform.position;
        Vector3 endPos = mermaidPos.transform.position;
        float startTime = Time.time;
        float duration = 3;

        while (true)
        {
            mermaid.transform.position = Vector3.Lerp(startPos, endPos, Easing.SingleAxisBezier.CubicBezier(Easing.Preset.SlowInSlowOut, Time.time - startTime, duration));

            if (Time.time - startTime > duration)
            {
                break;
            }

            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(1.5f);

        mermaidAnim.Play("Mermaid_Howl");
        yield return new WaitForSeconds(1);

        GameObject go = Instantiate(bubble);
        go.transform.parent = PlayerMove.instance.transform;
        go.transform.localPosition = new Vector3(0,0,-1);
        yield return new WaitForSeconds(0.5f);

        mermaidAnim.Play("Mermaid_Idle");
        StartCoroutine(PlayerMove.instance.BubbleUp());
        yield return new WaitForSeconds(2);

        mermaidAnim.Play("Mermaid_Sing");
        yield return new WaitForSeconds(1);

        mermaidAnim.Play("Mermaid_Idle");
        yield return new WaitForSeconds(2);

        UIManager.instance.dieReason.text = "1회";
        StartCoroutine(UIManager.instance.ClearPanel());
    }
}
