using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class PlayerMove : MonoBehaviour
{
    [Header("PlayerMove")]
    [SerializeField] float dashSpeed = 150f;
    [Header("Dash")]
    [SerializeField] GameObject dashBack;
    [SerializeField] Image[] dashImg;
    [SerializeField] AudioClip dashClip;
    [SerializeField] AudioClip dashCountClip;
    [Header("Slow")]
    [SerializeField] GameObject slowBack;
    [SerializeField] float gazeSpeed = 20;
    [SerializeField] Image slowImg;
    [SerializeField] GameObject[] ghost;
    [SerializeField] GameObject ghostBack;
    [SerializeField, Range(0, 1)] public float slowGaze;
    [SerializeField] AudioClip slowClip;
    [Header("Rage")]
    [SerializeField] GameObject rageSkillImg;
    [SerializeField] Text rageCoolTimeText;
    [SerializeField] Image rageCoolImg;
    [SerializeField] AudioClip rageClip;

    public Rigidbody2D body;
    SpriteRenderer playerSprite;
    Animator playerAnim;
    AudioSource audioSource;
    public PlayerStatus ps;

    Vector3 mousePos;
    public static Vector3 targetPos;

    float h, v;
    public float speed = 3;
    bool isMoving;
    public bool canJump = false;
    bool isFloor;

    // Dash()관리
    bool canDash = true;
    bool isCoolTime;
    int dashCount = 3;
    int maxDashCount = 3;
    float dashCooltime = 0.5f;
    float dashRecovertime = 2f;

    // Slow()관리
    bool isSlow;
    float currentTime = 0;
    float duration = 5;
    float nowGaze =1;
    float ghostTime=0;
    int ghostNum=0;

    // Rage()관리
    float prevArmory;
    bool isRage;

    public enum BlessState { Dash, Slow, Rage, None}
    public BlessState blessState = BlessState.None;
    public static PlayerMove instance;

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
        playerSprite = GetComponent<SpriteRenderer>();
        playerAnim = GetComponent<Animator>();
        ps = GetComponent<PlayerStatus>();
        body = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        slowGaze = 1000;
    }

    // �̵�
    void FixedUpdate()
    {
        if(UIManager.instance.stopTime != 0) return;
        mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
        targetPos = mousePos - transform.position;

        Move();
    }

    void Update()
    {
        if (UIManager.instance.stopTime != 0) return;

        if (blessState == BlessState.Dash) 
        {
            StartCoroutine(DashCountRecover());
            if(Input.GetMouseButtonDown(1)) Dash();
        }
        else if (blessState == BlessState.Slow)
        {
            if(Input.GetMouseButtonDown(1))
            {
                currentTime = 0;
                nowGaze = slowGaze;
                if (nowGaze > 0.4) isSlow = true;
                else UIManager.instance.SetSystemMessage("침착 사용 불가!");
            }
            else if(Input.GetMouseButton(1) && isSlow) Slow();
            else if(Input.GetMouseButtonUp(1))
            {
                Time.timeScale = 1;
                ghostBack.SetActive(false);
                ghostTime = 0;
                currentTime = 0;
                nowGaze = slowGaze;
                isSlow = false;
            }
            ChargeSlowGaze();
        }
        else if (blessState == BlessState.Rage)
        {
            if (Input.GetMouseButtonDown(1))
            {
                if(isRage) { UIManager.instance.SetSystemMessage("분노 쿨타임!"); return;}
                
                StartCoroutine(RageCoolTimeRoutine());
                StartCoroutine(Rage());
            }
        }

        if(canJump && Input.GetKeyDown(KeyCode.W) && isFloor)
        {
            print("Jump");
            body.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
        }
    }

    float GetAngle(Vector2 p1, Vector2 p2)
    {
        float angle = 0;
        float x = p2.x - p1.x;
        float y = p2.y - p1.y;
        angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;

        return angle;
    }

    void Move()
    {
        Vector3 upMove = Vector2.up * speed * Time.deltaTime * Input.GetAxis("Vertical");
        Vector3 rightMove = Vector2.right * speed * Time.deltaTime * Input.GetAxis("Horizontal");

        if(canJump) upMove = Vector3.zero;

        transform.position += upMove;
        transform.position += rightMove;

        playerSprite.flipX = (targetPos.x > 0) ? false : true;
    }

    public IEnumerator Rage()
    {
        playerSprite.color = Color.red;
        audioSource.PlayOneShot(rageClip);
        speed = 5;
        ps.playerStatus[0] *= 1.5f;
        prevArmory = ps.playerStatus[1];
        ps.playerStatus[1] = 0;
        UIManager.instance.gameObject.transform.GetChild(7).GetComponent<StatusPanel>().SetText();

        yield return new WaitForSeconds(10);
        playerSprite.color = Color.white;
        yield return new WaitForSeconds(0.5f);
        playerSprite.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        playerSprite.color = Color.white;
        yield return new WaitForSeconds(0.5f);
        playerSprite.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        playerSprite.color = Color.white;

        speed = 1.5f;
        ps.playerStatus[0] /= 1.5f;
        ps.playerStatus[1] = prevArmory;
        UIManager.instance.gameObject.transform.GetChild(7).GetComponent<StatusPanel>().SetText();
        yield return new WaitForSeconds(2);

        speed = 3;
    }

    private IEnumerator RageCoolTimeRoutine()
    {
        rageCoolTimeText.gameObject.SetActive(true);
        float time = 20;
        isRage = true;

        while (true)
        {
            time -= Time.deltaTime;
            rageCoolTimeText.text = time.ToString("F0") + "S";

            float per = time / 20;
            rageCoolImg.fillAmount = per;

            if (time <= 0)
            {
                rageCoolTimeText.gameObject.SetActive(false);
                break;
            }
            yield return null;
        }
        isRage = false;
    }

    public void Dash()
    {
        if(dashCount<=0) {UIManager.instance.SetSystemMessage("대쉬 횟수 없음!"); return;}

        if (canDash)
        {
            canDash = false;
            audioSource.PlayOneShot(dashClip);
            dashCount--;
            dashImg[dashCount].gameObject.SetActive(false);
            body.AddForce(targetPos.normalized * 10 , ForceMode2D.Impulse);
            StartCoroutine(DashCoolTimeRoutine());
        }
    }

    public void Slow()
    { 
        audioSource.PlayOneShot(slowClip);
        Time.timeScale = 0.5f;
        ghostBack.SetActive(true);
        if(currentTime > ghostTime)
        {
            ghostTime = currentTime+0.1f;
            ghost[ghostNum].GetComponent<SpriteRenderer>().sprite = playerSprite.sprite;
            ghost[ghostNum].gameObject.transform.localPosition = transform.localPosition;
            ghost[ghostNum++].SetActive(true);
            if(ghostNum>=10) ghostNum=0;
        }

        slowGaze = Mathf.Lerp(nowGaze, 0, currentTime/(nowGaze * 3));
        if(slowGaze <= 0)
        {
            Time.timeScale =1;
            ghostBack.SetActive(false);
            ghostTime = 0;
            nowGaze = 0;
            currentTime = 0;
            isSlow = false;
        } 
        slowImg.fillAmount = slowGaze;
        slowImg.color = (slowGaze<0.4f) ? Color.red : Color.blue;
        currentTime += Time.deltaTime;
    }

    public void ChargeSlowGaze()
    {
        if(isSlow) return;

        slowGaze = Mathf.Lerp(nowGaze, 1, currentTime/((1-nowGaze) * 10));
        slowImg.fillAmount = slowGaze;
        slowImg.color = (slowGaze < 0.4f) ? Color.red : Color.blue;
        currentTime += Time.deltaTime;
    }

    IEnumerator DashCoolTimeRoutine()
    {
        yield return new WaitForSeconds(dashCooltime);
        canDash = true;
    }

    IEnumerator DashCountRecover()
    {
        if(dashCount < maxDashCount && !isCoolTime)
        {
            isCoolTime = true;
            yield return new WaitForSeconds(dashRecovertime);
            dashCount++;
            audioSource.PlayOneShot(dashCountClip);
            dashImg[dashCount -1].gameObject.SetActive(true);
            isCoolTime = false;
        }
    }

    public void SetAxis(float x, float y)
    {
        h = x;
        v = y;

        if (h == 0 && v == 0) isMoving = false;
        else isMoving = true;
    }

    public void SetBless(string name)
    {
        switch(name)
        {
            case "Dash":
                blessState = BlessState.Dash;
                dashBack.SetActive(true);
                slowBack.gameObject.SetActive(false);
                rageSkillImg.gameObject.SetActive(false);
                UIManager.instance.bless.text = "신속";
                UIManager.instance.SetSystemMessage("신속 축복이 부여됩니다!");
                break;
            case "Slow":
                blessState = BlessState.Slow;
                dashBack.SetActive(false);
                slowBack.gameObject.SetActive(true);
                rageSkillImg.gameObject.SetActive(false);
                UIManager.instance.bless.text = "침착";
                UIManager.instance.SetSystemMessage("침착 축복이 부여됩니다!");
                break;
            case "Rage":
                blessState = BlessState.Rage;
                dashBack.SetActive(false);
                slowBack.gameObject.SetActive(false);
                rageSkillImg.gameObject.SetActive(true);
                UIManager.instance.bless.text = "분노";
                UIManager.instance.SetSystemMessage("분노 축복이 부여됩니다!");
                break;
            default:
                blessState = BlessState.None;
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if(other.transform.CompareTag("Floor")) isFloor = true;
    }
    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.transform.CompareTag("Floor")) isFloor = false;
    }

    public IEnumerator BubbleUp()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + new Vector3(0, 10, 0);
        float startTime = Time.time;
        float duration = 5;

        while (true)
        {
            transform.position = Vector3.Lerp(startPos, endPos, Easing.SingleAxisBezier.CubicBezier(Easing.Preset.SlowInSlowOut, Time.time - startTime, duration));

            if (Time.time - startTime > duration)
            {
                break;
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
