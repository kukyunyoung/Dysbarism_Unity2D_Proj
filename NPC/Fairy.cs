using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// 플레이어 주위를 맴도는 안내npc
// 플레이어와 상호작용 할 수 있는 요소들과 닿을때 대사 출력
// 전투 시작시에 사라졌다가 전투가 끝나면 다시 등장함
// 플레이어와 일정거리 이상 벗어나면 다시 플레이어 위치에 옴


public class Fairy : MonoBehaviour
{
    [SerializeField] TextMeshPro text;
    [SerializeField] GameObject textBack;

    GameObject player;
    SpriteRenderer sr;
    SpriteRenderer psr;
    Vector3 randomRot;
    Vector3 x = new Vector3(1, 0, 0);
    Vector3 z = new Vector3(0,0,0.1f);

    public bool isTalk;
    bool isMove;
    float textW, textH;

    public enum MapState{ village, stage, boss}
    MapState mapState = MapState.village;

    public static Fairy instance;

    private void Awake()
    {
        #region singleTon
        if (instance == null) instance = this;
        else Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);
        #endregion singleTon
    }

    void Start()
    {
        player = PlayerMove.instance.gameObject;
        sr = gameObject.GetComponent<SpriteRenderer>();
        StartCoroutine(Float());
        text.gameObject.SetActive(false);
        textBack.SetActive(false);
    }

    IEnumerator Float()
    {
        float startTime = Time.time;
        float duration = 1.5f;
        bool isFar = false;
        randomRot = (Vector3)Random.insideUnitCircle;
        Vector3 startPos = transform.position;
        Vector3 endPos = randomRot + transform.position;
        sr.flipX = (randomRot.x > 0)? true : false;

        while (true)
        {
            transform.position = Vector3.Lerp(startPos, endPos, Easing.SingleAxisBezier.CubicBezier(Easing.Preset.FastInSlowOut, Time.time - startTime, duration));

            if (Time.time - startTime > duration)
            {
                yield return new WaitForSeconds(3);
                startTime = Time.time;
                randomRot = (Vector3)Random.insideUnitCircle;
                isFar = Vector3.Distance(transform.position, player.transform.position) > 3 ? true : false; 
                startPos = transform.position;
                endPos = isFar ? player.transform.position + z : randomRot + transform.position;
                sr.flipX = (randomRot.x > 0) ? true : false;
            }

            yield return new WaitForEndOfFrame();    
        }
    }

    public IEnumerator Talk(string str)
    {
        isTalk = true;
        text.gameObject.SetActive(true);
        textBack.SetActive(true);
        text.text = str;
        textW = text.preferredWidth;
        textH = text.preferredHeight;
        textBack.transform.localScale = new Vector3(textW+1,textH,1);

        yield return new WaitForSeconds(3);
        
        text.gameObject.SetActive(false);
        textBack.SetActive(false);

        yield return new WaitForSeconds(5);
        isTalk = false;
    }
}
