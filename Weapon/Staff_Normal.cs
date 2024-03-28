using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Staff_Normal : MonoBehaviour, IUseableItem
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject idleStaff;
    [SerializeField] GameObject attStaff;
    [SerializeField] GameObject[] energyVolt;    // short, middle, full
    [SerializeField] GameObject gazeBack;
    [SerializeField] Image chargeImg;

    public enum StaffState { ready, charging, fire }
    public enum FireState { noCharge, shortCharge, middleCharge, fullCharge }
    public StaffState staffstate { get; private set; }
    public FireState firestate { get; private set; }

    public Gradient gradient;

    SpriteRenderer playerSprite;
    SpriteRenderer idleSprite;
    SpriteRenderer attSprite;

    PlayerOnTarget playerAngle;
    WaitForSeconds chargeWait;
    Coroutine myCoroutine;

    Vector3 originPos;

    float angle;
    float chargeTime;
    float charge;
    bool isCharge;
    bool voltSet;

    void Start()
    {
        idleStaff.SetActive(true); attStaff.SetActive(false);
        player = PlayerMove.instance.gameObject;

        playerSprite = player.GetComponent<SpriteRenderer>();
        playerAngle = player.GetComponent<PlayerOnTarget>();
        idleSprite = idleStaff.GetComponent<SpriteRenderer>();
        attSprite = attStaff.GetComponent<SpriteRenderer>();
        originPos = transform.position;
        staffstate = StaffState.ready;
        firestate = FireState.noCharge;

        if(!voltSet)
        {
            voltSet = true;
            for(int i=0; i<3; i++)
            {
                energyVolt[i] = ObjPullingManager.instance.gameObject.transform.GetChild(1).GetChild(i).gameObject;
                energyVolt[i].SetActive(true);
            }

            gazeBack = player.transform.GetChild(1).GetChild(0).GetChild(0).gameObject;
            chargeImg = player.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
        }

        chargeWait = new WaitForSeconds(0.02f);
    }

    void Update()
    {
        if(UIManager.instance.stopTime!=0) {StopAllCoroutines(); return;}

        idleSprite.flipX = (PlayerMove.targetPos.x > 0) ? false : true;
        angle = Mathf.Atan2(PlayerMove.targetPos.y, PlayerMove.targetPos.x) * Mathf.Rad2Deg;

        if (Input.GetMouseButtonDown(0)) Attack();
        else if (Input.GetMouseButtonUp(0)) AttackFinish(); transform.rotation = Quaternion.identity;

        if (Input.GetMouseButton(0))
        {
            angle = Mathf.Atan2(PlayerMove.targetPos.y, PlayerMove.targetPos.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        }
    }

    public void Attack()
    {
        isCharge = true;
        gazeBack.SetActive(true);
        idleStaff.SetActive(false); attStaff.SetActive(true);
        StartCoroutine(Process());
    }

    private void OnEnable() 
    {
        WeaponManager.bulletText.gameObject.SetActive(false);
        chargeTime = PlayerMove.instance.gameObject.GetComponent<PlayerStatus>().playerStatus[6];
        charge = 0.004f * chargeTime;

        for (int i = 0; i < 3; i++)
        {
            energyVolt[i] = ObjPullingManager.instance.gameObject.transform.GetChild(1).GetChild(i).gameObject;
            energyVolt[i].transform.position = transform.position - new Vector3(1000,0,0);
            energyVolt[i].SetActive(true); energyVolt[i].SetActive(false);
        }

        gazeBack = player.transform.GetChild(1).GetChild(0).GetChild(0).gameObject;
        chargeImg = player.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
    }

    public void AttackFinish()
    {
        isCharge = false;
        chargeImg.fillAmount = 0;

        switch (firestate)
        {
            case FireState.noCharge:
                break;
            case FireState.shortCharge:
                energyVolt[0].SetActive(true);
                energyVolt[0].GetComponent<EnergyVolt>().Shot(angle, 15);
                break;
            case FireState.middleCharge:
                energyVolt[1].SetActive(true);
                energyVolt[1].GetComponent<EnergyVolt>().Shot(angle, 25);
                break;
            case FireState.fullCharge:
                energyVolt[2].SetActive(true);
                energyVolt[2].GetComponent<EnergyVolt>().Shot(angle, 35);
                break;
            default:
                break;
        }

        attStaff.SetActive(false); idleStaff.SetActive(true);
        firestate = FireState.noCharge;
        StopCoroutine(myCoroutine);
        gazeBack.SetActive(false);
    }

    IEnumerator Process()
    {
        if (isCharge)
        {
            myCoroutine = StartCoroutine(chargeImg.GetComponent<ShakeObj>().ShakeGaze(0.5f, isCharge));

            for (int i = 0; i < 250; i++)
            {
                if (!isCharge) break;
                chargeImg.fillAmount += charge;

                if (0.4f <= chargeImg.fillAmount && chargeImg.fillAmount < 0.7f) { firestate = FireState.shortCharge; StopCoroutine(myCoroutine); myCoroutine = StartCoroutine(chargeImg.GetComponent<ShakeObj>().ShakeGaze(2, isCharge)); }
                else if (0.7f <= chargeImg.fillAmount && chargeImg.fillAmount < 1) { firestate = FireState.middleCharge; StopCoroutine(myCoroutine); myCoroutine = StartCoroutine(chargeImg.GetComponent<ShakeObj>().ShakeGaze(3.5f, isCharge)); }
                else if (1 <= chargeImg.fillAmount) { firestate = FireState.fullCharge; StopCoroutine(myCoroutine); myCoroutine = StartCoroutine(chargeImg.GetComponent<ShakeObj>().ShakeGaze(5, isCharge)); }

                chargeImg.color = gradient.Evaluate(chargeImg.fillAmount);
                yield return chargeWait;
            }
        }
    }

    public IEnumerator Shake(float amount, bool isCharge)
    {
        while (isCharge)
        {
            transform.position = (Vector3)Random.insideUnitCircle * amount + originPos;

            yield return chargeWait;
        }

        transform.position = originPos;
    }

    public bool Use()
    {
        throw new System.NotImplementedException();
    }
}
