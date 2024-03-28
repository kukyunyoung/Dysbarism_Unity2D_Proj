using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class StartSceneManager : MonoBehaviour
{
    [SerializeField] Text pressText;
    [SerializeField] Image fadePanel;

    Color fade = new Color(0,0,0,0.01f);
    int time;
    bool isPress = false;

    private void Awake() 
    {
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        pressText.gameObject.SetActive(false);
        StartCoroutine(ShowAndHideText());
        fadePanel.color = new Color(0,0,0,1);
    }

    void Update()
    {
        if(Input.anyKeyDown && time > 2 && !isPress)
        {
            isPress = true;
            StartCoroutine(FadeOut());
        }
    }

    IEnumerator ShowAndHideText()
    {
        for(int i=0; i< 100; i++)
        {
            fadePanel.color -= fade;
            yield return new WaitForSeconds(0.02f);
        }

        while(true)
        {
            pressText.gameObject.SetActive(!pressText.gameObject.activeSelf);
            yield return new WaitForSeconds(0.5f);
            time++;
        }
    }

    IEnumerator FadeOut()
    {
        for (int i = 0; i < 100; i++)
        {
            fadePanel.color += fade;
            yield return new WaitForSeconds(0.02f);
        }

        SceneManager.LoadScene("VillageScene");
    }
}
