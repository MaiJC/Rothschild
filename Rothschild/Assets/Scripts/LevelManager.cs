using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    LoadRes loadRes;
    private List<int> commonEventID = new List<int>();
    private int levelCount;
    private List<List<int>> levelEventID = new List<List<int>>();
    private List<List<int>> levelStoryID = new List<List<int>>();
    private int currentEventID;
    private int currentStoryID;
    private int currentLevel = 0;
    private int currentEventCount = 0;
    private int currentRound = 0;
    private int currentMaxSelectedPersonCount = 2;
    private int currentMaxSelectedChooiceCount = 2;
    private int currentSelectedCount = 0;
    private int currentStoryHead;
    private bool isInStory = false;
    private Dictionary<int, string> eventUIPath = new Dictionary<int, string>();
    private Dictionary<int, string> eventText = new Dictionary<int, string>();
    private OnEvent onEvent;


    /*this is just use for monkeys*/
    private List<string> cardPath = new List<string>();
    private List<Image> personImage = new List<Image>();
    private List<OnPerson> person = new List<OnPerson>();

    // Use this for initialization
    void Start()
    {
        Initialize();
        InitializeMonkey();
        NextLevel();
        NextEvent();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Confirm()
    {
        ClearSelect();
        NextEvent();
    }

    void Initialize()
    {
        loadRes = GameObject.Find("DataHandler").GetComponent<LoadRes>();
        onEvent = GameObject.Find("EventSlot").GetComponent<OnEvent>();
        onEvent.SetUnselectable();

        levelCount = loadRes.GetLevelCount();
        commonEventID = loadRes.GetCommonEventID();
        for (int i = 1; i <= levelCount; i++)
        {
            levelEventID.Add(loadRes.GetLevelEventID(i));
            levelStoryID.Add(loadRes.GetLevelStoryID(i));
        }
        for (int i = 0; i < commonEventID.Count; i++)
        {
            int eventID = commonEventID[i];
            string path = loadRes.GetEventUIPath(eventID);
            //string text = loadRes.GetEventText(eventID);
            eventUIPath.Add(eventID, path);
            //eventText.Add(eventID, text);
        }
        for (int i = 0; i < levelCount; i++)
        {
            for (int j = 0; j < levelEventID[i].Count; j++)
            {
                int eventID = levelEventID[i][j];
                Debug.Log("get event ui path: " + eventID.ToString());
                string path = loadRes.GetEventUIPath(eventID);
                //string text = loadRes.GetEventText(eventID);
                eventUIPath.Add(eventID, path);
                //eventText.Add(eventID, text);
            }
        }
    }

    void InitializeMonkey()
    {
        cardPath.Add("banana");
        cardPath.Add("monkey");
        cardPath.Add("banana_alfa");

        personImage.Add(GameObject.Find("PersonPanelA").transform.GetChild(0).gameObject.GetComponent<Image>());
        personImage.Add(GameObject.Find("PersonPanelB").transform.GetChild(0).gameObject.GetComponent<Image>());
        personImage.Add(GameObject.Find("PersonPanelC").transform.GetChild(0).gameObject.GetComponent<Image>());
        personImage.Add(GameObject.Find("PersonPanelD").transform.GetChild(0).gameObject.GetComponent<Image>());

        person.Add(GameObject.Find("PersonPanelA").GetComponent<OnPerson>());
        person.Add(GameObject.Find("PersonPanelB").GetComponent<OnPerson>());
        person.Add(GameObject.Find("PersonPanelC").GetComponent<OnPerson>());
        person.Add(GameObject.Find("PersonPanelD").GetComponent<OnPerson>());

        foreach (Image i in personImage)
        {
            i.overrideSprite = Resources.Load("monkey", typeof(Sprite)) as Sprite;
        }
    }

    public bool AddSelect()
    {
        onEvent.SetSelectable();
        if (currentSelectedCount == currentMaxSelectedPersonCount)
        {
            return false;
        }
        else
        {
            /*TODO: 增加如果选择数已达上限，则将未选择人物变灰的功能*/
            currentSelectedCount++;
            return true;
        }
    }

    public void RemoveSelect()
    {
        /*TODO: 增加如果原本选择数满上线，则将灰色的卡恢复彩色的功能*/
        currentSelectedCount--;
        if (currentSelectedCount == 0)
            onEvent.SetUnselectable();

        if (currentSelectedCount < 0)
            Debug.LogError("currentSelectedCount less than 0!");
    }

    void NextEvent()
    {
        /*TODO: 增加是否为下一关的判断*/

        currentRound++;

        /*TODO: 判断是否退出故事*/
        if (isInStory)
        {
            //currentEventID=loadRes.GetNextStoryID(currentStoryHead,)
            int chooice = this.tag == "ChooseOne" ? 1 : 2;
            int fatherEventID = currentEventID;
            //currentEventID = loadRes.GetNextStoryID(currentStoryHead, choose, fatherEventID);

            List<int> roles = new List<int>();
            for (int i = 0; i < person.Count; i++)
            {
                if (person[i].IsSelected()) roles.Add(i + 1);
            }
            currentEventID = loadRes.GetNextStoryEvent(currentStoryID, fatherEventID, chooice, roles);
            if (currentEventID == 0)
                isInStory = false;
            Debug.Log("Story ID:" + currentStoryID.ToString() + " ,current event ID:" + currentEventID.ToString());
        }
        if (!isInStory)
        {
            /*TODO: 目前故事的出现是完全随机，没有限制回合数的，后期需要加入故事出现的回合数判断*/
            //从事件池里面随机事件
            int idx = Random.Range(0, currentEventCount + levelStoryID[currentLevel - 1].Count);
            if (idx < commonEventID.Count)
            {
                currentEventID = commonEventID[idx];
                commonEventID.RemoveAt(idx);
                currentEventCount--;
            }
            else if (idx < currentEventCount)
            {
                currentEventID = levelEventID[currentLevel - 1][idx - commonEventID.Count];
                levelEventID[currentLevel - 1].RemoveAt(idx - commonEventID.Count);
                currentEventCount--;
            }
            else
            {
                currentStoryID = levelStoryID[currentLevel - 1][idx - currentEventCount];
                //这里要加一个getcurrenteventid
                currentEventID = loadRes.GetStoryHeadEventID(currentStoryID);
                levelStoryID[currentLevel - 1].RemoveAt(idx - currentEventCount);
                isInStory = true;
                currentStoryHead = currentEventID;
            }
        }

        string tmpS = currentEventID.ToString();
        if (tmpS.StartsWith("2"))
            Debug.Log("error level 2 happen!");

        /*TODO: 添加事件切换特效*/
        //为事件槽设置新的图片和文字描述
        //onEvent.SetImage(eventUIPath[currentEventID]);
        //onEvent.SetText(eventText[currentEventID]);
        Debug.Log("Next event: " + currentEventID.ToString());
        if (loadRes.GetEventText(currentEventID).Count == 0)
        {
            Debug.Log("return event text null");
        }
        onEvent.SetEventText(loadRes.GetEventText(currentEventID));
        onEvent.SetEventID(currentEventID);
    }

    void NextLevel()
    {
        currentLevel++;
        //
        currentEventCount = commonEventID.Count + levelEventID[currentLevel - 1].Count;
    }

    void ClearSelect()
    {
        currentSelectedCount = 0;
    }

}
