using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class login_interface : MonoBehaviour {

    GameObject login_canvas_obj;

    GameObject user_name_inputfield_obj;
    GameObject password_inputfield_obj;

    GameObject login_button_obj;
    GameObject register_button_obj;
    GameObject confirm_button_obj;
    GameObject cancel_button_obj;

    class Account
    {
        public Account(string user_name,string password)
        {
            this.user_name = user_name;
            this.password = password;
        }

        public string user_name;
        public string password;
    }

    List<Account> account_list = new List<Account>();


	// Use this for initialization
	void Start () {

        login_canvas_obj = GameObject.Find("login_canvas");
        login_canvas_obj.SetActive(true);

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

        confirm_button_obj.SetActive(false);
        cancel_button_obj.SetActive(false);

        print_account_list();
	}


    void login_button_click()
    {
        var user_name_input = user_name_inputfield_obj.GetComponent<InputField>().text;
        var password_input = password_inputfield_obj.GetComponent<InputField>().text;


        foreach(var account in account_list)
        {
            if(user_name_input == account.user_name )
            {
                if (password_input == account.password)
                {
                    //登录成功，跳到下一界面
                    Debug.Log("登录成功，跳到下一界面");
                }
                else
                {
                    warning("密码输入错误！");
                }

                return;
            }
        }
        warning("用户名不存在！");
    }


    void register_button_click()
    {
        confirm_button_obj.SetActive(true);
        cancel_button_obj.SetActive(true);

        login_button_obj.SetActive(false);
        register_button_obj.SetActive(false);
    }


    void confirm_button_click()
    {
        var user_name_input = user_name_inputfield_obj.GetComponent<InputField>().text;
        var password_input = password_inputfield_obj.GetComponent<InputField>().text;


        foreach (var account in account_list)
        {
            if (user_name_input == account.user_name)
            {
                warning("用户名已被注册！");
                return;
            }
        }

        account_list.Add(new Account(user_name_input, password_input));

        login_button_obj.SetActive(true);
        register_button_obj.SetActive(true);

        confirm_button_obj.SetActive(false);
        cancel_button_obj.SetActive(false);
    }


    void cancel_button_click()
    {
        login_button_obj.SetActive(true);
        register_button_obj.SetActive(true);

        confirm_button_obj.SetActive(false);
        cancel_button_obj.SetActive(false);
    }


    void warning(string warning_text)
    {
        Debug.Log(warning_text);
    }

	
	// Update is called once per frame
	void Update () {
		
	}

    void print_account_list()
    {
        foreach (var account in account_list)
        {
            Debug.Log(string.Format("user_name:{0}, password:{1}", account.user_name, account.password));
        }
    }
}


//账号密码非空