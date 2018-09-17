using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class login_interface : MonoBehaviour {

    PlayerDataProc playerdataproc;

    GameObject login_canvas_obj;
    GameObject login_background_obj;
    GameObject ready_background_obj;

    GameObject warning_text_obj;

    GameObject user_name_inputfield_obj;
    GameObject password_inputfield_obj;

    GameObject login_button_obj;
    GameObject register_button_obj;
    GameObject confirm_button_obj;
    GameObject cancel_button_obj;


	// Use this for initialization
	void Start () {

        //导入小康的脚本
        playerdataproc = GameObject.Find("LogicHandler").GetComponent<PlayerDataProc>();

        login_canvas_obj = GameObject.Find("login_canvas");
        login_canvas_obj.SetActive(true);

        warning_text_obj = GameObject.Find("warning_text");

        login_background_obj = GameObject.Find("login_background");
        ready_background_obj = GameObject.Find("ready_background");

        user_name_inputfield_obj = GameObject.Find("user_name_inputfield");
        password_inputfield_obj = GameObject.Find("password_inputfield");

        login_button_obj = GameObject.Find("login_button");
        register_button_obj = GameObject.Find("register_button");
        confirm_button_obj = GameObject.Find("confirm_button");
        cancel_button_obj = GameObject.Find("cancel_button");

        login_button_obj.GetComponent<Button>().onClick.AddListener(login_button_click);
        register_button_obj.GetComponent<Button>().onClick.AddListener(register_button_click);
        confirm_button_obj.GetComponent<Button>().onClick.AddListener(confirm_button_click);
        cancel_button_obj.GetComponent<Button>().onClick.AddListener(cancel_button_click);

        ready_background_obj.GetComponent<Button>().onClick.AddListener(ready_background_click);

        confirm_button_obj.SetActive(false);
        cancel_button_obj.SetActive(false);
        ready_background_obj.SetActive(false);
	}


    void login_button_click()
    {
        var user_name_input = user_name_inputfield_obj.GetComponent<InputField>().text;
        var password_input = password_inputfield_obj.GetComponent<InputField>().text;

        switch(playerdataproc.OnLogin(user_name_input, password_input))
        {
            case 1:
                warning("用户名不存在");
                break;
            case 2:
                warning("密码错误");
                break;
            case 0:
                //登录成功，跳到下一界面
                ready_background_obj.SetActive(true);
                login_background_obj.SetActive(false);
                break;
        }
    }


    void register_button_click()
    {
        warning("");

        confirm_button_obj.SetActive(true);
        cancel_button_obj.SetActive(true);

        login_button_obj.SetActive(false);
        register_button_obj.SetActive(false);
    }


    void confirm_button_click()
    {
        var user_name_input = user_name_inputfield_obj.GetComponent<InputField>().text;
        var password_input = password_inputfield_obj.GetComponent<InputField>().text;

        switch (playerdataproc.OnRegister(user_name_input, password_input))
        {
            case 1:
                warning("用户名已存在");
                break;
            case 2:
                warning("用户名或密码不可为空");
                break;
            case 0:
                warning("注册成功，请直接登录");
                login_button_obj.SetActive(true);
                register_button_obj.SetActive(true);

                confirm_button_obj.SetActive(false);
                cancel_button_obj.SetActive(false);

                break;
        }
    }


    void cancel_button_click()
    {
        warning("");

        login_button_obj.SetActive(true);
        register_button_obj.SetActive(true);

        confirm_button_obj.SetActive(false);
        cancel_button_obj.SetActive(false);
    }


    void warning(string warning_text)
    {
        warning_text_obj.GetComponent<Text>().text = warning_text;
    }


    void ready_background_click()
    {
        login_canvas_obj.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

}