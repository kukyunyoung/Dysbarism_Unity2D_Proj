using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI coinText;
    [SerializeField] Button newGameBtn;
    [SerializeField] Button bgmsoundBtn;
    [SerializeField] Button effsoundBtn;
    [SerializeField] Button devModeBtn;
    [SerializeField] Button mainSceneBtn;
    [SerializeField] AudioClip bgm;
    [SerializeField] GameObject inventoryUI;
    [SerializeField] Text[] systemMessage;
    [SerializeField] public GameObject statusPanel;
    [SerializeField] public Image filterPanel;
    [SerializeField] public Image fadePanel;
    [SerializeField] Image[] statePanel; // poison, blood, electric
    [Space]
    [SerializeField] GameObject playerDiePanel;
    [SerializeField] GameObject clearPanel;
    [SerializeField] Text playtime;
    public Text dieReason;
    public Text bless;
    [SerializeField] Text bestStage;
    [SerializeField] GameObject[] diePanelGo;
    [SerializeField] GameObject[] clearPanelGo;
    public Image[] equipments;
    [SerializeField] GameObject killedMob;
    [SerializeField] Text killedMobText;
    [SerializeField] Text clearKilledMobText;
    [SerializeField] public GameObject[] fadeObj;
    [SerializeField] public GameObject crabPont;
    [SerializeField] public GameObject hivePont;

    public int piranha=0;
    public int jellyfish=0;
    public int blowfish=0;
    public int octopus=0;
    public int shark=0;

    public static UIManager instance;

    public static int coin;
    public static float playTime;
    public int stopTime = 0;

    int sysNum=0;
    int timeScaleNum=1;
    int invenOn = 1;
    bool goMain;

    public AudioSource uiAudio;
    Vector3 UIPos = new Vector3(0, -1500, 0);
    Color fade = new Color(0, 0, 0, 0.01f);

    private void Awake()
    {
        #region singleTon
        if(instance == null) instance = this;
        else Destroy(this);

        DontDestroyOnLoad(this);
        #endregion singleTon

        newGameBtn.onClick.AddListener(() => SceneChangeBtn("VillageScene"));
        bgmsoundBtn.onClick.AddListener(() => BgmSoundBtn());
        devModeBtn.onClick.AddListener(() => SceneChangeBtn("DevScene"));
        mainSceneBtn.onClick.AddListener(() => SceneChangeBtn("MainScene"));

        diePanelGo[4].GetComponent<Button>().onClick.AddListener(() => GoMainBtn());
        clearPanelGo[4].GetComponent<Button>().onClick.AddListener(() => GoMainBtn());
    }

    void Start()
    {
        uiAudio = GetComponent<AudioSource>();
        uiAudio.PlayOneShot(bgm);
    }

    private void Update()
    {
        playTime = Time.time;
        if (Input.GetKeyDown(KeyCode.V)) 
        {
            inventoryUI.transform.position += UIPos;
            stopTime += invenOn;
            invenOn *= -1;
            UIPos *= -1;
        }
        else if(Input.GetKeyDown(KeyCode.X))
        {
            statusPanel.SetActive(!statusPanel.activeSelf);
        }
    }

    public void EscapeInvenUI()
    {
        if (stopTime>0 && UIPos.y>0)
        {
            inventoryUI.transform.position += UIPos;
            UIPos *= -1;
            stopTime=0;
        }
    }

    void FixedUpdate()
    {
        coinText.text = coin * 100 + " G";
    }

    void SceneChangeBtn(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    void BgmSoundBtn()
    {
        uiAudio.mute = !uiAudio.mute;
    }

    public void SetSystemMessage(string text)
    {
        systemMessage[sysNum].text = text;
        systemMessage[sysNum].gameObject.SetActive(true);
        sysNum++;
        if(sysNum > systemMessage.Length-1) sysNum = 0;
    }

    public IEnumerator PlayerDie()
    {
        bestStage.text = "1 스테이지";
        playtime.text = (playTime/60).ToString() + " : " + (playTime % 60).ToString("F0");
        killedMobText.text = "";
        yield return new WaitForSeconds(2);

        playerDiePanel.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        killedMobText.text = "피라냐 : " + piranha + "\n";
        yield return new WaitForSeconds(0.2f);
        killedMobText.text += "해파리 : " + jellyfish + "\n";
        yield return new WaitForSeconds(0.2f);
        killedMobText.text += "상어 : " + shark + "\n";
        yield return new WaitForSeconds(0.2f);
        killedMobText.text += "문어 : " + octopus + "\n";
        yield return new WaitForSeconds(0.2f);
        killedMobText.text += "복어 : " + blowfish;

        yield return new WaitForSeconds(0.5f);
        diePanelGo[0].SetActive(true);
        yield return new WaitForSeconds(0.5f);
        diePanelGo[1].SetActive(true);
        yield return new WaitForSeconds(0.5f);
        diePanelGo[2].SetActive(true);
        yield return new WaitForSeconds(0.5f);
        diePanelGo[3].SetActive(true);
        yield return new WaitForSeconds(0.5f);
        diePanelGo[4].SetActive(true);
    }

    public IEnumerator ClearPanel()
    {
        bestStage.text = "1 스테이지";
        playtime.text = (playTime / 60).ToString() + " : " + (playTime % 60).ToString("F0");
        clearKilledMobText.text = "";
        yield return new WaitForSeconds(2);

        clearPanel.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        clearKilledMobText.text = "피라냐 : " + piranha + "\n";
        yield return new WaitForSeconds(0.2f);
        clearKilledMobText.text += "해파리 : " + jellyfish + "\n";
        yield return new WaitForSeconds(0.2f);
        clearKilledMobText.text += "상어 : " + shark + "\n";
        yield return new WaitForSeconds(0.2f);
        clearKilledMobText.text += "문어 : " + octopus + "\n";
        yield return new WaitForSeconds(0.2f);
        clearKilledMobText.text += "복어 : " + blowfish;

        yield return new WaitForSeconds(0.5f);
        clearPanelGo[0].SetActive(true);
        yield return new WaitForSeconds(0.5f);
        clearPanelGo[1].SetActive(true);
        yield return new WaitForSeconds(0.5f);
        clearPanelGo[2].SetActive(true);
        yield return new WaitForSeconds(0.5f);
        clearPanelGo[3].SetActive(true);
        yield return new WaitForSeconds(0.5f);
        clearPanelGo[4].SetActive(true);
    }

    public void GoMainBtn()
    {
        if(!goMain)
        {
            goMain = true;
            fadePanel.gameObject.SetActive(true);
            StartCoroutine(FadeOut());
        }
    }

    IEnumerator FadeOut()
    {
        for (int i = 0; i < 100; i++)
        {
            fadePanel.color += fade;
            yield return new WaitForSeconds(0.02f);
        }

        GetComponent<SaveManager>().InitJson();
        playerDiePanel.SetActive(false);
        clearPanel.SetActive(false);
        stopTime = 0;
        SceneManager.LoadScene("StartScene");
        Destroy(PlayerMove.instance.gameObject);
        Destroy(ObjPullingManager.instance.gameObject);
        Destroy(Fairy.instance.gameObject);
        Destroy(instance.gameObject);
    }

    IEnumerator FadeIn()
    {
        for (int i = 0; i < 100; i++)
        {
            fadePanel.color -= fade;
            yield return new WaitForSeconds(0.02f);
        }
    }

    public IEnumerator StatePanel(int arr)
    {
        statePanel[arr].gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        statePanel[arr].gameObject.SetActive(false);
    }
}
