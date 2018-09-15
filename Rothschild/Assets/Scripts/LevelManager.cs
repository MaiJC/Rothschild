using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    LoadRes loadRes;
    PlayerDataProc playerDataProc;
    private List<int> commonEventID = new List<int>();
    private int levelCount;
    private List<List<int>> levelEventID = new List<List<int>>();
    private List<List<int>> levelStoryID = new List<List<int>>();
    //private List<List<int>> levelZTPreStoryID = new List<List<int>>();
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
    private bool isInJumpStory = false;
    private int jumpStoryTimesRemain;
    private const int JUMP_STORY_TIMES_MAX = 4;
    private Dictionary<int, string> eventUIPath = new Dictionary<int, string>();
    private Dictionary<int, string> eventText = new Dictionary<int, string>();
    private OnEvent onEvent;
    private int originLevelEventCount;
    private int originLevelStoryCount;
    private List<int> roleLimit = new List<int>();
    private List<int> happenedEvent = new List<int>();
    private List<bool> jumpStorySelectedRole = new List<bool>();
    private bool isJumpStoryFirstHappen = false;
    private int currentJumpStoryPerson;
    private struct ZTPreStoryCondition
    {
        public int storyID;
        public int perRole;
        public int perChoice;
    };
    private Dictionary<int, ZTPreStoryCondition> ZTPreStory = new Dictionary<int, ZTPreStoryCondition>();


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

    public void Confirm(int choice, List<bool> role)
    {
        //判断并把重廷前置故事加入故事池
        if (ZTPreStory.ContainsKey(currentEventID))
        {
            ZTPreStoryCondition tmpCondition = ZTPreStory[currentEventID];
            //只要前置事件出现了故事就能发生
            if (tmpCondition.perRole == 0 && tmpCondition.perChoice == 0)
            {
                InsertPreStory();
            }
            //需要前置事件出现和选择特定角色
            else if (tmpCondition.perRole != 0 && tmpCondition.perChoice == 0)
            {
                for (int i = 0; i < role.Count; i++)
                {
                    if (role[i] == true && ZTPreStory[currentEventID].perRole == i + 1)
                    {
                        InsertPreStory();
                    }
                }
            }
            else if (tmpCondition.perRole == 0 && tmpCondition.perChoice != 0)
            {
                if (choice == ZTPreStory[currentEventID].perChoice)
                {
                    InsertPreStory();
                }
            }
            else if (tmpCondition.perRole != 0 && tmpCondition.perChoice != 0)
            {
                for (int i = 0; i < role.Count; i++)
                {
                    if (role[i] == true && ZTPreStory[currentEventID].perRole == i + 1)
                    {
                        if (choice == ZTPreStory[currentEventID].perChoice)
                        {
                            InsertPreStory();
                        }
                    }
                }
            }

        }

        if (isInJumpStory)
        {
            //在跳跳跳的起始事件
            if (isJumpStoryFirstHappen)
            {
                isJumpStoryFirstHappen = false;
                //跳跳跳的出口
                //TODO: 跳跳跳的出口要不要进行结算？
                if (choice == 2)
                {
                    isInJumpStory = false;
                }
                //记录跳跳跳当前的主角
                else
                {
                    for (int i = 0; i < role.Count; i++)
                    {
                        if (role[i] == true)
                        {
                            currentJumpStoryPerson = i + 1;
                            break;
                        }
                    }
                }
            }
        }

        ClearSelect();
        NextEvent();
    }

    private void JudgeZTPreStory()
    {

    }

    void Initialize()
    {
        loadRes = GameObject.Find("DataHandler").GetComponent<LoadRes>();
        onEvent = GameObject.Find("EventSlot").GetComponent<OnEvent>();
        playerDataProc = GameObject.Find("LogicHandler").GetComponent<PlayerDataProc>();
        //onEvent.SetUnselectable();

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
                string path = loadRes.GetEventUIPath(eventID);
                //string text = loadRes.GetEventText(eventID);
                eventUIPath.Add(eventID, path);
                //eventText.Add(eventID, text);
            }
        }

        for (int i = 0; i < levelEventID.Count; i++)
        {
            for (int j = 0; j < levelEventID[i].Count; j++)
            {
                if (loadRes.IsStroryEvent(levelEventID[i][j]))
                {
                    levelEventID[i].RemoveAt(j);
                    j--;
                }
            }
        }

        //设置重廷前置故事数组
        for (int i = 0; i < levelStoryID.Count; i++)
        {
            for (int j = 0; j < levelStoryID[i].Count; j++)
            {
                int tmpStoryID = levelStoryID[i][j];
                int tmpPreEvent = 0, tmpPreRole = 0, tmpPreChoice = 0;
                if (playerDataProc.IsPreStroy(tmpStoryID, ref tmpPreEvent, ref tmpPreRole, ref tmpPreChoice))
                {
                    ZTPreStoryCondition tmpZTCondition;
                    tmpZTCondition.storyID = tmpStoryID;
                    tmpZTCondition.perRole = tmpPreRole;
                    tmpZTCondition.perChoice = tmpPreChoice;
                    ZTPreStory.Add(tmpPreEvent, tmpZTCondition);
                    levelStoryID[i].RemoveAt(j);
                    j--;
                }
            }
        }

        for (int i = 0; i < 4; i++)
            jumpStorySelectedRole.Add(false);

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

    public bool AddSelect(string personTag)
    {
        int selectPerson = 0;
        switch (personTag)
        {
            case "PersonOne":
                selectPerson = 1;
                break;
            case "PersonTwo":
                selectPerson = 2;
                break;
            case "PersonThree":
                selectPerson = 3;
                break;
            case "PersonFour":
                selectPerson = 4;
                break;
        }
        foreach (int p in roleLimit)
        {
            if (p == selectPerson)
                return false;
        }

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

        if (isInStory || isInJumpStory)
        {
            Debug.Log("in story or in jump story");
            //currentEventID=loadRes.GetNextStoryID(currentStoryHead,)
            int chooice = this.tag == "ChoiceOne" ? 1 : 2;
            int fatherEventID = currentEventID;
            //currentEventID = loadRes.GetNextStoryID(currentStoryHead, choose, fatherEventID);

            //找出被选中的角色
            List<int> roles = new List<int>();
            for (int i = 0; i < person.Count; i++)
            {
                if (person[i].IsSelected()) roles.Add(i + 1);
            }
            currentEventID = loadRes.GetNextStoryEvent(currentStoryID, fatherEventID, chooice, roles);

            //单条故事线结束，判断是否跳跳跳，以决定是否退出
            if (currentEventID == 0)
            {
                if (isInJumpStory)
                {
                    //跳跳跳单条线结束，返回故事开头，同时将之前的主角加入已经选择名单
                    if (jumpStoryTimesRemain != 0)
                    {
                        Debug.Log("back to jump story head");
                        jumpStoryTimesRemain--;
                        currentEventID = currentStoryHead;
                        jumpStorySelectedRole[currentJumpStoryPerson - 1] = true;
                    }
                    else
                    {
                        Debug.Log("a jump story end");
                        isInJumpStory = false;
                        if (levelStoryID[currentLevel - 1].Count == 0)
                        {
                            NextLevel();
                        }
                    }

                }
                else if (isInStory)
                {
                    Debug.Log("a story end");
                    isInStory = false;
                    if (levelStoryID[currentLevel - 1].Count == 0)
                    {
                        NextLevel();
                    }
                }
            }
            Debug.Log("Story ID:" + currentStoryID.ToString() + " ,current event ID:" + currentEventID.ToString());
        }
        if (!isInStory && !isInJumpStory)
        {
            /*TODO: 目前故事的出现是完全随机，没有限制回合数的，后期需要加入故事出现的回合数判断*/
            //从事件池里面随机事件
            //int idx = Random.Range(0, currentEventCount + levelStoryID[currentLevel - 1].Count);

            int maxRange = commonEventID.Count + levelEventID[currentLevel - 1].Count
                + levelStoryID[currentLevel - 1].Count;
            int idx = Random.Range(0, maxRange);

            //发生所有关卡都能出现的事件
            if (idx < commonEventID.Count)
            {
                //确保事件如果有前置事件，前置事件已经发生
                bool havePerEventAndNotHappen = false;
                do
                {
                    int preEvent = loadRes.GetPreEventID(commonEventID[idx]);
                    if (preEvent != 0)
                    {
                        if (!happenedEvent.Contains(preEvent))
                        {
                            havePerEventAndNotHappen = true;
                            idx = Random.Range(0, commonEventID.Count);
                        }
                        else
                        {
                            havePerEventAndNotHappen = false;
                        }
                    }
                } while (havePerEventAndNotHappen);

                currentEventID = commonEventID[idx];
                commonEventID.RemoveAt(idx);
                currentEventCount--;
            }
            //发生该关卡特有的事件
            else if (idx < levelEventID[currentLevel - 1].Count)
            {
                Debug.Log("in a normal event");
                //确保事件如果有前置事件，前置事件已经发生
                bool havePerEventAndNotHappen = false;
                do
                {
                    //idx = Random.Range(commonEventID.Count, levelEventID[currentLevel - 1].Count);
                    int preEvent = loadRes.GetPreEventID(levelEventID[currentLevel - 1][idx]);
                    Debug.Log(preEvent.ToString());
                    if (preEvent != 0)
                    {
                        if (!happenedEvent.Contains(preEvent))
                        {
                            havePerEventAndNotHappen = true;
                            idx = Random.Range(commonEventID.Count, levelEventID[currentLevel - 1].Count);
                            Debug.Log("have pre event and pre event hasn't happen");
                        }
                        else
                        {
                            Debug.Log("have pre event and pre event has happened");
                            havePerEventAndNotHappen = false;
                        }
                    }
                    else
                    {
                        Debug.Log("don't have per event");
                        havePerEventAndNotHappen = false;
                    }
                } while (havePerEventAndNotHappen);

                currentEventID = levelEventID[currentLevel - 1][idx - commonEventID.Count];
                levelEventID[currentLevel - 1].RemoveAt(idx - commonEventID.Count);
                currentEventCount--;
            }
            //生成故事
            else
            {
                Debug.Log("a story start");
                //永远都取出第一个故事
                currentStoryID = levelStoryID[currentLevel - 1][0];
                currentEventID = loadRes.GetStoryHeadEventID(currentStoryID);
                levelStoryID[currentLevel - 1].RemoveAt(0);
                if (playerDataProc.IsJumpStory(currentEventID))
                {
                    isInJumpStory = true;
                    isJumpStoryFirstHappen = true;
                    jumpStoryTimesRemain = JUMP_STORY_TIMES_MAX;
                }
                else
                    isInStory = true;
                currentStoryHead = currentEventID;
            }
        }


        /*TODO: 添加事件切换特效*/
        //为事件槽设置新的图片和文字描述
        //onEvent.SetImage(eventUIPath[currentEventID]);
        //onEvent.SetText(eventText[currentEventID]);
        Debug.Log("Next event: " + currentEventID.ToString());

        //设置该关卡的图片，文字，可选择角色等信息
        if (isInJumpStory)//这里其实是错的
        {
            //跳跳跳的开头
            if (isJumpStoryFirstHappen)
            {
                onEvent.SetEventText(loadRes.GetEventText(currentEventID));
                onEvent.SetEventID(currentEventID);
                onEvent.SetChoiceType(1, 2);
                onEvent.SetUnselectable();
                for (int i = 0; i < jumpStorySelectedRole.Count; i++)
                {
                    if (jumpStorySelectedRole[i])
                    {
                        roleLimit.Add(i + 1);
                        person[i].SetUnselectable();
                        //onEvent.SetPersonUnselectable(i + i);
                    }
                }
                currentMaxSelectedPersonCount = 1;
                happenedEvent.Add(currentEventID);
            }
            //不是跳跳跳的开头
            else
            {
                onEvent.SetEventText(loadRes.GetEventText(currentEventID));
                onEvent.SetEventID(currentEventID);
                onEvent.SetChoiceType(2, 2);
                onEvent.SetUnselectable();
                for (int i = 0; i < 4; i++)
                {
                    if (i == currentJumpStoryPerson - 1)
                    {
                        person[i].SetSelected();
                    }
                    else
                    {
                        person[i].SetUnselectable();
                    }
                }
            }
        }
        else
        {
            int choiceTypeOne, choiceTypeTwo;
            choiceTypeOne = loadRes.GetChoiceType(currentEventID, 1);
            choiceTypeTwo = loadRes.GetChoiceType(currentEventID, 2);
            onEvent.SetEventText(loadRes.GetEventText(currentEventID));
            onEvent.SetEventID(currentEventID);
            onEvent.SetChoiceType(choiceTypeOne, choiceTypeTwo);
            Debug.Log("CT1:" + choiceTypeOne.ToString() + " ,CT2:" + choiceTypeTwo.ToString());
            onEvent.SetUnselectable();

            roleLimit = loadRes.GetRoleLimit(currentEventID);
            currentMaxSelectedPersonCount = loadRes.GetRoleCountLimit(currentEventID);

            happenedEvent.Add(currentEventID);
        }

    }

    void NextLevel()
    {
        Debug.Log("Next level");
        currentLevel++;
        originLevelEventCount = levelEventID[currentLevel - 1].Count;
        originLevelStoryCount = levelStoryID[currentLevel - 1].Count;

        currentEventCount = commonEventID.Count + levelEventID[currentLevel - 1].Count;
    }

    void ClearSelect()
    {
        currentSelectedCount = 0;
    }

    void InsertPreStory()
    {
        int tmpZTStoryID = ZTPreStory[currentEventID].storyID;
        ZTPreStory.Remove(currentEventID);
        for (int i = 0; i < levelStoryID[currentLevel - 1].Count; i++)
        {
            if (tmpZTStoryID < levelStoryID[currentLevel - 1][i])
            {
                levelStoryID[currentLevel - 1].Insert(i, tmpZTStoryID);
            }
        }
    }

}
