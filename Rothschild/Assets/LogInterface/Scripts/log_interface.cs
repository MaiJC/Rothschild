using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class log_interface : MonoBehaviour
{
    private bool isLoadComplete = false;

    class GameEvent
    {
        public GameEvent(GameObject body, GameObject highlight_body, GameObject name)
        {
            this.body = body;
            description = body.transform.Find("event_description_text").gameObject;
            choice = body.transform.Find("event_choice_text").gameObject;

            this.highlight_body = highlight_body;
            highlight_description = highlight_body.transform.Find("highlight_event_description_text").gameObject;
            highlight_choice = highlight_body.transform.Find("highlight_event_choice_text").gameObject;

            this.name = name;
            name_text = name.transform.Find("event_name_text").gameObject;
            name_image = name.transform.Find("event_name_image").gameObject;
        }

        public GameObject body;
        public GameObject description;
        public GameObject choice;

        public GameObject highlight_body;
        public GameObject highlight_description;
        public GameObject highlight_choice;

        public GameObject name;
        public GameObject name_text;
        public GameObject name_image;
    }


    PlayerDataProc playerdataproc;
    LoadRes loadres;

    int current_role_id;
    List<string> role_name_list;
    int role_total_num;

    GameObject log_canvas_obj;
    GameObject game_canvas_obj;
    GameObject role_portrait_image_obj;
    GameObject role_name_text_obj;
    GameObject wealth_value_text_obj;
    GameObject reputation_value_text_obj;
    GameObject cohesion_fill_image_obj;

    //GameObject enter_log_interface_button_obj;
    GameObject exit_log_interface_button_obj;

    GameObject event_obj;
    GameObject event_body_obj;
    GameObject event_body_prefab_obj;
    GameObject event_body_grid_layout_panel_obj;
    RectTransform event_body_grid_layout_panel_recttransform;

    GameObject highlight_event_body_obj;
    GameObject highlight_event_body_prefab_obj;
    GameObject highlight_event_body_grid_layout_panel_obj;
    RectTransform highlight_event_body_grid_layout_panel_recttransform;

    GameObject event_name_background_image_obj;
    GameObject event_name_prefab_obj;

    List<GameEvent> clone_event_prefab_list = new List<GameEvent>();

    GameObject horizontal_slide_obj;

    float event_body_height;
    float event_body_prefab_height;
    float highlight_event_body_prefab_height;
    float event_body_grid_layout_panel_world_init_pos_y;
    float event_body_grid_layout_panel_world_min_pos_y;
    float event_body_grid_layout_panel_world_max_pos_y;

    float highlight_event_body_world_center_pos_y;

    Vector2 touch_start_pos;
    float event_body_grid_layout_panel_world_target_pos_y;
    float event_body_grid_layout_panel_adjust_rate;
    bool vertical_slide_effective;
    float canvas_width;
    float canvas_height;
    float event_width;
    float event_height;
    float event_below_height;

    float horizontal_slide_range_lower_limit;
    float horizontal_slide_range_upper_limit;
    float vertical_slide_range_lower_limit;
    float vertical_slide_range_upper_limit;
    float horizontal_slide_threshold;
    float vertical_slide_threshold;

    float event_name_background_image_width;
    float event_name_background_image_height;
    float event_name_prefab_width;
    float event_name_prefab_height;

    float circle_world_x_upper_limit;
    float circle_world_center_pos_x;
    float circle_world_center_pos_y;
    float circle_radius;
    float circle_world_y_upper_limit;
    float circle_world_y_lower_limit;

    float center_event_world_y_range_upper_limit;
    float center_event_world_y_range_lower_limit;

    //test
    int enter_log_interface_click_count = 0;
    int exit_log_interface_click_count = 0;
    int horizontal_slide_count = 0;
    int vertical_slide_count = 0;


    ////进入日志界面
    //void enter_log_interface_click()
    //{
    //    enter_log_interface_click_count++;

    //    enter_log_interface();

    //    //下面是JC的操作
    //    game_canvas_obj.SetActive(false);
    //}

    //给JC调用
    public void enter_log_interface()
    {
        log_canvas_obj.SetActive(true);

        refresh_role_portrait_image();
        refresh_role_name_text();
        refresh_property();
        refresh_cohesion();
        refresh_event();
    }


    //退出日志界面
    void exit_log_interface_click()
    {
        exit_log_interface_click_count++;

        //to change
        //调用JC的接口
        game_canvas_obj.SetActive(true);

        log_canvas_obj.SetActive(false);
    }


    //用于：进入日志界面、水平滑动
    void refresh_role_portrait_image()
    {
        var role_image_path = string.Format("role_portrait/{0}", current_role_id);
        role_portrait_image_obj.GetComponent<Image>().overrideSprite = Resources.Load(role_image_path, typeof(Sprite)) as Sprite;
    }

    //用于：进入日志界面、水平滑动
    void refresh_role_name_text()
    {
        var role_name = role_name_list[current_role_id];
        role_name_text_obj.GetComponent<Text>().text = role_name;
    }

    //用于：进入日志界面、水平滑动
    void refresh_property()
    {
        var playerattr = playerdataproc.GetPlayerAttr();

        int wealth_value = playerattr[current_role_id].money;
        int reputation_value = playerattr[current_role_id].reputation;

        wealth_value_text_obj.GetComponent<Text>().text = wealth_value.ToString();
        reputation_value_text_obj.GetComponent<Text>().text = reputation_value.ToString();
    }

    //用于：进入日志界面
    void refresh_cohesion()
    {
        var cohesion_value = (float)playerdataproc.GetTeamworkValue() / 200;

        cohesion_fill_image_obj.GetComponent<Image>().fillAmount = cohesion_value;
    }


    class EventInformation
    {
        public EventInformation(string name, string description, string choice)
        {
            this.name = name;
            this.description = description;
            this.choice = choice;
        }

        public string name;
        public string description;
        public string choice;
    }

    //先从后端获取已发生的事件序列（按发生时间排序），再筛选出与当前role有关的事件，最后根据事件id补全event_information
    List<EventInformation> get_event_information_list()
    {
        var event_information_list = new List<EventInformation>();

        //一头一尾空白event
        event_information_list.Add(new EventInformation("", "", ""));

        //to change
        var event_log_list = playerdataproc.GetRoleEventList("loongzhang", current_role_id + 1);

        foreach (var event_log in event_log_list)
        {
            event_information_list.Add(new EventInformation(event_log.eventTitle, event_log.eventDesc, event_log.eventChoice));
        }

        //一头一尾空白event
        event_information_list.Add(new EventInformation("", "", ""));

        //如果没有事件，强行展示三个空事件
        while (event_information_list.Count < 3)
        {
            event_information_list.Add(new EventInformation("", "", ""));
        }

        return event_information_list;
    }

    //载入日志，水平滑动
    void refresh_event()
    {
        foreach (var clone_event_prefab in clone_event_prefab_list)
        {

            Destroy(clone_event_prefab.body);
            Destroy(clone_event_prefab.highlight_body);
            Destroy(clone_event_prefab.name);
        }
        clone_event_prefab_list.Clear();

        var event_information_list = get_event_information_list();

        var event_body_grid_layout_panel_height = event_information_list.Count * event_body_prefab_height;
        var highlight_event_body_grid_layout_panel_height = event_information_list.Count * highlight_event_body_prefab_height;

        event_body_grid_layout_panel_world_max_pos_y = event_body_grid_layout_panel_world_min_pos_y + (event_information_list.Count - 3) * event_body_prefab_height;

        //设置height
        event_body_grid_layout_panel_recttransform.sizeDelta = new Vector2(event_body_grid_layout_panel_recttransform.sizeDelta.x, event_body_grid_layout_panel_height);
        highlight_event_body_grid_layout_panel_recttransform.sizeDelta = new Vector2(highlight_event_body_grid_layout_panel_recttransform.sizeDelta.x, highlight_event_body_grid_layout_panel_height);

        //event_body、event_body_grid_layout_panel的pivot必须置顶（x轴方向不重要），RectTransform.position返回的是pivot的世界坐标
        //滚动文本上对齐
        event_body_grid_layout_panel_recttransform.position = new Vector3(event_body_grid_layout_panel_recttransform.position.x, event_body_grid_layout_panel_world_init_pos_y, event_body_grid_layout_panel_recttransform.position.z);

        //test
        int test_value = enter_log_interface_click_count + exit_log_interface_click_count + horizontal_slide_count + vertical_slide_count;

        foreach (var event_information in event_information_list)
        {
            var clone_event_prefab = new GameEvent(Instantiate(event_body_prefab_obj), Instantiate(highlight_event_body_prefab_obj), Instantiate(event_name_prefab_obj));

            clone_event_prefab.body.transform.SetParent(event_body_grid_layout_panel_obj.transform);
            clone_event_prefab.description.GetComponent<Text>().text = string.Format("{0}", event_information.description);
            clone_event_prefab.choice.GetComponent<Text>().text = string.Format("{0}", event_information.choice);

            clone_event_prefab.highlight_body.transform.SetParent(highlight_event_body_grid_layout_panel_obj.transform);
            clone_event_prefab.highlight_description.GetComponent<Text>().text = string.Format("{0}", event_information.description);
            clone_event_prefab.highlight_choice.GetComponent<Text>().text = string.Format("{0}", event_information.choice);

            clone_event_prefab.name.transform.SetParent(event_name_background_image_obj.transform);
            clone_event_prefab.name_image.GetComponent<Image>().color = new Color(255 / 255, 255 / 255, 255 / 255, 255 / 255);
            clone_event_prefab.name_text.GetComponent<Text>().text = string.Format("{0}", event_information.name);

            clone_event_prefab_list.Add(clone_event_prefab);
        }
    }

    //用于update
    void update_event_body()
    {
        float event_body_grid_layout_panel_world_next_pos_y;

        if (Math.Abs(event_body_grid_layout_panel_recttransform.position.y - event_body_grid_layout_panel_world_target_pos_y) < event_body_grid_layout_panel_adjust_rate)
        {
            event_body_grid_layout_panel_world_next_pos_y = event_body_grid_layout_panel_world_target_pos_y;
        }
        else if (event_body_grid_layout_panel_recttransform.position.y < event_body_grid_layout_panel_world_target_pos_y)
        {
            event_body_grid_layout_panel_world_next_pos_y = event_body_grid_layout_panel_recttransform.position.y + event_body_grid_layout_panel_adjust_rate;
        }
        else
        {
            event_body_grid_layout_panel_world_next_pos_y = event_body_grid_layout_panel_recttransform.position.y - event_body_grid_layout_panel_adjust_rate;
        }

        event_body_grid_layout_panel_recttransform.position = new Vector3(event_body_grid_layout_panel_recttransform.position.x, event_body_grid_layout_panel_world_next_pos_y, event_body_grid_layout_panel_recttransform.position.z);
    }

    //用于update
    void update_highlight_event_body()
    {
        //计算与初始化相比，向上滑动了多少个clone_event_body_obj
        var upper_scroll_event_body_num = (event_body_grid_layout_panel_recttransform.position.y - event_body_grid_layout_panel_world_init_pos_y) / event_body_prefab_height;

        //highlight_event_body的pivot必须居中，highlight_event_body_grid_layout_panel的pivot必须置顶（x轴方向不重要）
        var highlight_event_body_grid_layout_panel_world_current_pos_y = (float)(upper_scroll_event_body_num + 1.5) * highlight_event_body_prefab_height + highlight_event_body_world_center_pos_y;

        highlight_event_body_grid_layout_panel_recttransform.position = new Vector3(highlight_event_body_grid_layout_panel_recttransform.position.x, highlight_event_body_grid_layout_panel_world_current_pos_y, highlight_event_body_grid_layout_panel_recttransform.position.z);
    }


    //用于update
    void update_event_name()
    {
        foreach (var clone_event_prefab in clone_event_prefab_list)
        {
            var clone_event_body_prefab_world_center_pos_y = clone_event_prefab.body.GetComponent<RectTransform>().position.y;
            var clone_event_name_prefab_world_center_pos_y = clone_event_body_prefab_world_center_pos_y;
            var clone_event_name_prefab_world_center_pos_x = clone_event_name_prefab_circle_get_world_pos_x(clone_event_name_prefab_world_center_pos_y);

            clone_event_prefab.name.GetComponent<RectTransform>().position = new Vector3(clone_event_name_prefab_world_center_pos_x, clone_event_name_prefab_world_center_pos_y, clone_event_prefab.name.GetComponent<RectTransform>().position.z);

            if (center_event_world_y_range_lower_limit <= clone_event_name_prefab_world_center_pos_y && clone_event_name_prefab_world_center_pos_y <= center_event_world_y_range_upper_limit)
            {
                clone_event_prefab.name_image.GetComponent<RectTransform>().sizeDelta = new Vector2(264, 284);
                clone_event_prefab.name_image.GetComponent<Image>().overrideSprite = Resources.Load("event_name/center_event_logo", typeof(Sprite)) as Sprite;
            }
            else
            {
                clone_event_prefab.name_image.GetComponent<RectTransform>().sizeDelta = new Vector2(219, 240);
                clone_event_prefab.name_image.GetComponent<Image>().overrideSprite = Resources.Load("event_name/side_event_logo", typeof(Sprite)) as Sprite;
            }
        }
    }

    float clone_event_name_prefab_circle_get_world_pos_x(float world_pos_y)
    {
        float world_pos_x;

        if (circle_world_y_lower_limit < world_pos_y && world_pos_y < circle_world_y_upper_limit)
        {
            world_pos_x = (float)Math.Sqrt(Math.Pow(circle_radius, 2) - Math.Pow(world_pos_y - circle_world_center_pos_y, 2)) + circle_world_center_pos_x;

        }
        else
        {
            world_pos_x = -1000;
        }

        return world_pos_x;
    }


    // Use this for initialization
    void Start()
    {
        //导入小康的脚本
        playerdataproc = GameObject.Find("LogicHandler").GetComponent<PlayerDataProc>();
        loadres = GameObject.Find("LogicHandler").GetComponent<LoadRes>();

        role_name_list = new List<string>() { "法兰克福", "伦敦", "巴黎", "那不勒斯" };
        role_total_num = role_name_list.Count;
        current_role_id = 0;

        log_canvas_obj = GameObject.Find("log_canvas");
        game_canvas_obj = GameObject.Find("Canvas");
        role_portrait_image_obj = GameObject.Find("role_portrait_image");
        role_name_text_obj = GameObject.Find("role_name_text");
        wealth_value_text_obj = GameObject.Find("wealth_value_text");
        reputation_value_text_obj = GameObject.Find("reputation_value_text");
        cohesion_fill_image_obj = GameObject.Find("cohesion_fill_image");
        event_obj = GameObject.Find("event");
        event_body_obj = GameObject.Find("event_body");
        event_body_prefab_obj = GameObject.Find("event_body_prefab");
        event_body_grid_layout_panel_obj = GameObject.Find("event_body_grid_layout_panel");
        event_body_grid_layout_panel_recttransform = event_body_grid_layout_panel_obj.GetComponent<RectTransform>();
        //enter_log_interface_button_obj = GameObject.Find("enter_log_interface_button");
        exit_log_interface_button_obj = GameObject.Find("exit_log_interface_button");

        horizontal_slide_obj = GameObject.Find("horizontal_slide");

        canvas_width = log_canvas_obj.GetComponent<RectTransform>().sizeDelta.x; //1080
        canvas_height = log_canvas_obj.GetComponent<RectTransform>().sizeDelta.y; //1920
        event_width = event_obj.GetComponent<RectTransform>().sizeDelta.x; //1030
        event_height = event_obj.GetComponent<RectTransform>().sizeDelta.y; //1320
        event_body_obj.GetComponent<RectTransform>().sizeDelta = new Vector2(event_body_obj.GetComponent<RectTransform>().sizeDelta.x, event_height);
        event_body_height = event_body_obj.GetComponent<RectTransform>().sizeDelta.y; //1320
        event_body_prefab_height = event_body_height / 3; //440
        event_body_prefab_obj.GetComponent<RectTransform>().sizeDelta = new Vector2(event_body_prefab_obj.GetComponent<RectTransform>().sizeDelta.x, event_body_prefab_height);
        event_below_height = event_obj.GetComponent<RectTransform>().position.y; //110
        event_body_grid_layout_panel_world_init_pos_y = event_body_obj.GetComponent<RectTransform>().position.y; //1430
        event_body_grid_layout_panel_world_min_pos_y = event_body_grid_layout_panel_world_init_pos_y; //1430
        event_body_grid_layout_panel_world_target_pos_y = event_body_grid_layout_panel_world_init_pos_y; //1430
        event_body_grid_layout_panel_adjust_rate = 10; //玄学调参

        highlight_event_body_obj = GameObject.Find("highlight_event_body");
        highlight_event_body_prefab_obj = GameObject.Find("highlight_event_body_prefab");
        highlight_event_body_grid_layout_panel_obj = GameObject.Find("highlight_event_body_grid_layout_panel");
        event_name_background_image_obj = GameObject.Find("event_name_background_image");
        event_name_prefab_obj = GameObject.Find("event_name_prefab");
        highlight_event_body_grid_layout_panel_recttransform = highlight_event_body_grid_layout_panel_obj.GetComponent<RectTransform>();
        highlight_event_body_prefab_height = highlight_event_body_prefab_obj.GetComponent<RectTransform>().sizeDelta.y; //622


        horizontal_slide_range_lower_limit = (canvas_width - event_width) / 2; //25
        horizontal_slide_range_upper_limit = (canvas_width + event_width) / 2; //1055
        vertical_slide_range_lower_limit = canvas_height - event_height - event_below_height; //490
        vertical_slide_range_upper_limit = canvas_height - event_below_height; //1810
        horizontal_slide_threshold = 300;
        vertical_slide_threshold = event_body_prefab_height / 2; //220
        highlight_event_body_world_center_pos_y = highlight_event_body_obj.GetComponent<RectTransform>().position.y; //770

        event_name_background_image_width = event_name_background_image_obj.GetComponent<RectTransform>().sizeDelta.x; //306
        event_name_background_image_height = event_name_background_image_obj.GetComponent<RectTransform>().sizeDelta.y; //1320
        event_name_prefab_width = event_name_prefab_obj.GetComponent<RectTransform>().sizeDelta.x; //264
        event_name_prefab_height = event_name_prefab_obj.GetComponent<RectTransform>().sizeDelta.y; //284

        circle_world_x_upper_limit = event_name_background_image_width + 20; //玄学调参
        circle_world_center_pos_x = circle_world_x_upper_limit / 2 - (float)Math.Pow(event_name_background_image_height, 2) / (8 * circle_world_x_upper_limit);
        circle_world_center_pos_y = event_below_height + event_name_background_image_height / 2;
        circle_radius = (circle_world_x_upper_limit) / 2 + (float)Math.Pow(event_name_background_image_height, 2) / (8 * circle_world_x_upper_limit);

        circle_world_y_upper_limit = circle_world_center_pos_y + circle_radius;
        circle_world_y_lower_limit = circle_world_center_pos_y - circle_radius;

        center_event_world_y_range_upper_limit = highlight_event_body_world_center_pos_y + event_body_prefab_height / 2; //990
        center_event_world_y_range_lower_limit = highlight_event_body_world_center_pos_y - event_body_prefab_height / 2; //550

        //enter_log_interface_button_obj.GetComponent<Button>().onClick.AddListener(enter_log_interface_click);
        exit_log_interface_button_obj.GetComponent<Button>().onClick.AddListener(exit_log_interface_click);

        //exit_log_interface_click();
    }

    void OnGUI()
    {
        if (Event.current.type == EventType.MouseDown)
        {
            touch_start_pos = Event.current.mousePosition;

            //event_body_grid_layout_panel在触摸开始时的pivot世界坐标，用于确定event_body的展示情况
            //event_body_grid_layout_panel_world_old_pos_y = event_body_grid_layout_panel_recttransform.position.y;

            //通过起始触摸坐标判定垂直滑动是否有效（x向右递增，y向下递增）
            var horizontal_criterion = horizontal_slide_range_lower_limit <= touch_start_pos.x && touch_start_pos.x <= horizontal_slide_range_upper_limit;
            var vertiacal_criterion = vertical_slide_range_lower_limit <= touch_start_pos.y && touch_start_pos.y <= vertical_slide_range_upper_limit;

            if (horizontal_criterion && vertiacal_criterion)
            {
                vertical_slide_effective = true;
            }
            else
            {
                vertical_slide_effective = false;
            }
        }

        if (Event.current.type == EventType.MouseUp)
        {
            var touch_end_pos = Event.current.mousePosition;

            var horizontal_delta_distance = Math.Abs(touch_end_pos.x - touch_start_pos.x);
            var vertical_delta_distance = Math.Abs(touch_end_pos.y - touch_start_pos.y);

            if (horizontal_delta_distance >= horizontal_slide_threshold && vertical_delta_distance < vertical_slide_threshold)
            {
                horizontal_slide_count++;

                Debug.Log("水平滑动有效");
                if (touch_end_pos.x < touch_start_pos.x)
                {
                    Debug.Log("向左滑");
                    current_role_id = (current_role_id + 1) % role_total_num;
                }
                else
                {
                    Debug.Log("向右滑");
                    current_role_id = (current_role_id - 1 + 4) % role_total_num;
                }

                horizontal_slide_obj.GetComponent<AudioSource>().Play();

                refresh_role_portrait_image();
                refresh_role_name_text();
                refresh_property();
                refresh_cohesion();
                refresh_event();
            }

            else if (vertical_slide_effective && vertical_delta_distance >= vertical_slide_threshold && horizontal_delta_distance < horizontal_slide_threshold)
            {
                vertical_slide_count++;

                Debug.Log("垂直滑动有效");
                if (touch_end_pos.y < touch_start_pos.y)
                {
                    Debug.Log("向上滑");
                    if (event_body_grid_layout_panel_world_target_pos_y < event_body_grid_layout_panel_world_max_pos_y)
                    {
                        event_body_grid_layout_panel_world_target_pos_y += event_body_prefab_height;
                    }
                }
                else
                {
                    Debug.Log("向下滑");
                    if(event_body_grid_layout_panel_world_target_pos_y > event_body_grid_layout_panel_world_min_pos_y)
                    {
                        event_body_grid_layout_panel_world_target_pos_y -= event_body_prefab_height;
                    }
                }
            }

            else
            {
                Debug.Log("滑动无效");
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        update_event_body();
        update_highlight_event_body();
        update_event_name();

        if (isLoadComplete == false && Time.time > 2)
        {
            isLoadComplete = true;
            exit_log_interface_click();
        }
    }


    //test
    void print_GameObject_pos(GameObject gameobject)
    {
        var rect = gameobject.GetComponent<RectTransform>();
        print_RectTransform_pos(rect);
    }

    //test
    void print_RectTransform_pos(RectTransform rect)
    {
        Debug.Log(string.Format("RectTransform_pos: ({0}, {1}, {2})", rect.position.x, rect.position.y, rect.position.z));
    }

    //test
    void print_touch_pos(Vector2 touch_pos)
    {
        Debug.Log(string.Format("touch_pos: ({0}, {1})", touch_pos.x, touch_pos.y));
    }
}