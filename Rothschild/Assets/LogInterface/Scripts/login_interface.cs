using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class login_interface : MonoBehaviour {

    PlayerDataProc playerdataproc;

    GameObject login_canvas_obj;
    //GameObject login_background_obj;
    GameObject ready_background_obj;
    double loadTime;

    // Use this for initialization
    void Start () {

        login_canvas_obj = GameObject.Find("login_canvas");
        login_canvas_obj.SetActive(true);
        ready_background_obj = GameObject.Find("ready_background");
        ready_background_obj.GetComponent<Button>().onClick.AddListener(ready_background_click);

        loadTime = Time.fixedTime;
    }

    void ready_background_click()
    {
        if (Time.fixedTime - loadTime < 1)
            return;

        login_canvas_obj.SetActive(false);
    }
	

}