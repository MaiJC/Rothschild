using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisSetting : MonoBehaviour
{

    double loadTime;
    bool isLoad = false;
    GameObject setting;
    // Use this for initialization
    void Start()
    {
        GameObject.Find("SettingButton").GetComponent<Button>().onClick.AddListener(OnSetting);
        loadTime = Time.fixedTime;
        setting = GameObject.Find("Setting");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Time.fixedTime - loadTime > 1 && isLoad == false)
        {
            isLoad = true;
            setting.SetActive(false);
        }
    }

    void OnSetting()
    {
        setting.SetActive(true);
    }
}
