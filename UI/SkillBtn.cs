using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillBtn : MonoBehaviour
{
    public Button btn;
    public Text CoolTimeText;
    private Coroutine coolTimeRoutine;
    public Image CoolImg;
    public PlayerHP php;
    public GameObject PoisonPanel;

    public float coolTime = 6f;

    public void Awake()
    {
        CoolTimeText.gameObject.SetActive(false);
        CoolImg.fillAmount = 0;
        PoisonPanel.gameObject.SetActive(false);
    }

    void Start()
    {
        btn = this.GetComponent<Button>();
        btn.onClick.AddListener(() =>
        {
            // ��ư�� �������� ��Ÿ���� �ƴϸ�
            if (coolTimeRoutine == null)
            {
                coolTimeRoutine = StartCoroutine(CoolTimeRoutine());
            }
        });
    }

    // ��ư�� ������
    // ��Ÿ�� �ؽ�Ʈ�� ��Ÿ�� �̹����� Ȱ��ȭ
    // coolTimeRoutine�� �ڷ�ƾ�� Ȱ��ȭ �ǰ� �ִµ��� null���� �ƴ� -> 0�� ���ϰ� �Ǹ� null
    private IEnumerator CoolTimeRoutine()
    {
        CoolTimeText.gameObject.SetActive(true);
        PoisonPanel.SetActive(true);
        float time = coolTime;

        while (true)
        {
            time -= Time.deltaTime;
            CoolTimeText.text = time.ToString("F0") + "S";

            float per = time / coolTime;
            CoolImg.fillAmount = per;

            if (time <= 0)
            {
                CoolTimeText.gameObject.SetActive(false);
                PoisonPanel.SetActive(false);
                break;
            }
            yield return null;
        }

        coolTimeRoutine = null;
    }
}
