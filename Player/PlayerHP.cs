using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHP : MonoBehaviour
{
    [SerializeField] AudioClip avoidSound;
    [SerializeField] AudioClip hitSound;
    [SerializeField] AudioClip dieSound;
    [SerializeField] TextMeshPro dmgText;
    [SerializeField] FollowCamera cam;

    public static int hp_Max;
    public static int hP;

    bool isNormal = true;
    bool isDie = false;
    bool canDmg = true;
    bool isPanel;

    public Text hPText;
    public Slider hPBar;

    PlayerStatus ps;
    PlayerMove pm;
    Image playerStatePanel;

    public enum CharState{ normal, blood, poison, electric}
    public CharState charState = CharState.normal;


    void Start()
    {
        ps = GetComponent<PlayerStatus>();
        pm = GetComponent<PlayerMove>();
        cam = GameObject.FindWithTag("MainCamera").GetComponent<FollowCamera>();
        playerStatePanel = UIManager.instance.gameObject.transform.GetChild(1).GetComponent<Image>();
    }

    void FixedUpdate()
    {
        hPText.text = hP + " / " + hp_Max;
        hPBar.value = hP;
        hPBar.maxValue = hp_Max;
    }

    public void GetDmg(int dmg, string state = null, int per = 0, string name = null)
    {
        if (canDmg && !isDie)
        {
            int avoid = Random.Range(1, 101);
            if(avoid <= ps.playerStatus[3])
            {
                UIManager.instance.uiAudio.PlayOneShot(avoidSound);
                UIManager.instance.SetSystemMessage("회피!");
                StartCoroutine(CanDmgDelay());
                return;
            }

            hP -= (int)(dmg * (1 - ps.playerStatus[1] * 0.01f));
            if(cam == null) cam = GameObject.FindWithTag("MainCamera").GetComponent<FollowCamera>();
            UIManager.instance.uiAudio.PlayOneShot(hitSound);
            cam.Hit();
            StartCoroutine(CanDmgDelay());

            TextMeshPro damage = Instantiate(dmgText);
            damage.transform.position = new Vector3(transform.position.x, transform.position.y + 1, -6);
            damage.GetComponent<DamageText>().damage = ((int)(dmg * (1 - ps.playerStatus[1] * 0.01f))).ToString();

            if (state != null && per > 0)
            {
                int random = Random.Range(1, 101);
                if (random < per) CharStateEffect(state);
            }
        }

        if (hP <= 0)
        {
            hP = 0;
            isDie = true;
            UIManager.instance.stopTime += 1;
            UIManager.instance.uiAudio.PlayOneShot(dieSound);
            if (!isPanel)
            {
                isPanel = true;
                UIManager.instance.dieReason.text = name;
                StartCoroutine(UIManager.instance.PlayerDie());
            }
        }
    }

    public void ChangeStateCanDmg(bool can)
    {
        canDmg = can;
    }

    IEnumerator CanDmgDelay()
    {
        canDmg = false;
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
        yield return new WaitForSeconds(1.5f);
        canDmg = true;
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
    }

    public void CharStateEffect(string State)
    {
        switch (State)
        {
            case "Poison":
                Debug.Log("State : Poison");
                StartCoroutine(Poison());
                StartCoroutine(UIManager.instance.StatePanel(0));
                break;
            case "Blood":
                Debug.Log("State : Blood");
                StartCoroutine(Blood());
                StartCoroutine(UIManager.instance.StatePanel(1));
                break;
            case "Electric":
                Debug.Log("State : Electric");
                StartCoroutine(Electric());
                StartCoroutine(UIManager.instance.StatePanel(2));
                break;
        }
    }

    IEnumerator Poison()
    {
        if (charState is not CharState.poison && isNormal)
        {
            playerStatePanel.color = new Color(0.3f, 0, 0.8f, 0.4f);
            playerStatePanel.gameObject.SetActive(true);
            isNormal = false;
            charState = CharState.poison;
            for(int i=0; i<6; i++)
            {
                yield return new WaitForSeconds(1);
                hP -= 2;
            }
            playerStatePanel.gameObject.SetActive(false);
            charState = CharState.normal;
            isNormal = true;
        }
    }

    IEnumerator Blood()
    {
        if(charState is not CharState.blood && isNormal)
        {
            playerStatePanel.color = new Color(0.6f, 0, 0, 0.4f);
            playerStatePanel.gameObject.SetActive(true);
            charState = CharState.blood;
            float armory = ps.playerStatus[1];
            ps.playerStatus[1] *= 0.7f;  // 방어력 30프로 깎임
            for(int i=0; i<3; i++)
            {
                yield return new WaitForSeconds(1);
                hP-=2;
            }
            playerStatePanel.gameObject.SetActive(false);
            charState = CharState.normal;
            ps.playerStatus[1] = armory;
            isNormal = true;
        }
    }

    IEnumerator Electric()
    {
        if(charState is not CharState.electric && isNormal)
        {
            playerStatePanel.color = new Color(0.4f, 0.4f, 0, 0.4f);
            playerStatePanel.gameObject.SetActive(true);
            charState = CharState.electric;
            for(int i=0; i<3; i++)
            {
                yield return new WaitForSeconds(2.7f);
                hP-=1;
                float speed = pm.speed;
                pm.speed = 0.5f;
                yield return new WaitForSeconds(0.3f);
                pm.speed = speed;
            }
            playerStatePanel.gameObject.SetActive(false);
            charState = CharState.normal;
            isNormal = true;
        }
    }

    
}
