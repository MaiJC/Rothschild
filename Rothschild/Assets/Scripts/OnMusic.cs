using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OnMusic : MonoBehaviour
{

    GameObject bgm_obj;
    GameObject horizontal_slide_obj;
    GameObject Music;

    enum STAT
    {
        DEVELOP, SETTING
    };
    STAT sss = STAT.DEVELOP;

    // Use this for initialization
    void Start()
    {
        GameObject.Find("MusicToggle").GetComponent<Toggle>().onValueChanged.AddListener(OnToggle);
        bgm_obj = GameObject.Find("bgm");
        horizontal_slide_obj = GameObject.Find("horizontal_slide");

        GameObject.Find("DevelopButton").GetComponent<Button>().onClick.AddListener(OnDevelopButton);
        Music = GameObject.Find("Music");

        GameObject.Find("BackButton").GetComponent<Button>().onClick.AddListener(OnBack);
        GameObject.Find("GoBackBack").GetComponent<Button>().onClick.AddListener(OnBack);
        GameObject.Find("QuitButton").GetComponent<Button>().onClick.AddListener(OnQuit);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnToggle(bool is_on)
    {
        switch (is_on)
        {
            case true:
                bgm_obj.GetComponent<AudioSource>().mute = false;
                horizontal_slide_obj.GetComponent<AudioSource>().mute = false;
                break;
            case false:
                bgm_obj.GetComponent<AudioSource>().mute = true;
                horizontal_slide_obj.GetComponent<AudioSource>().mute = true;
                break;
        }
    }

    private void OnDevelopButton()
    {
        Music.SetActive(false);
        sss = STAT.SETTING;
    }

    private void OnBack()
    {
        if (sss == STAT.DEVELOP)
        {
            this.gameObject.SetActive(false);
        }
        else
        {
            Music.SetActive(true);
            sss = STAT.DEVELOP;
        }
    }

    private void OnQuit()
    {
        Application.Quit();
    }
}
