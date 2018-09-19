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
    public int currentMaxSelectedPersonCount = 2;
    private int currentMaxSelectedPersonCountCopy = 100;
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
    private int lastChoice;
    private bool[] hasDead = new bool[4];
    private int handleDeadRemains = 0;
    private int currentHanlingDeadPerson = 0;
    private bool hasInitialize = false;
    private int choiceTypeOne, choiceTypeTwo;
    private int choiceTypeOneCopy, choiceTypeTwoCopy;
    private struct ZTPreStoryCondition
    {
        public int storyID;
        public int perRole;
        public int perChoice;
    };
    private Dictionary<int, ZTPreStoryCondition> ZTPreStory = new Dictionary<int, ZTPreStoryCondition>();
    private GameObject deadInterface;
    private List<int> selectedPerson = new List<int>();

    /*this is just use for monkeys*/
    private List<string> cardPath = new List<string>();
    private List<Image> personImage = new List<Image>();
    private List<OnPerson> person = new List<OnPerson>();

    double loadTime;
    // Use this for initialization
    void Start()
    {
        //Initialize();
        //InitializeMonkey();
        //NextLevel();
        //NextEvent();
        loadTime = Time.fixedTime;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        gameObject.SetActive(true);
        if (hasInitialize == false && Time.fixedTime - loadTime > 2)
        {
            hasInitialize = true;
            Initialize();
            InitializeMonkey();
            NextLevel();
            NextEvent();
        }
    }

    public void Confirm(int choice, List<bool> role)
    {
        lastChoice = choice;

        //处理死亡
        if (handleDeadRemains != 0)
        {
            int i;
            for (i = 0; i < role.Count; i++)
            {
                if (role[i] == true) break;
            }
            DeadConfirm(i + 1, choice);
            ClearSelect();
            return;
        }



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
        deadInterface = GameObject.Find("DeadInterface");
        deadInterface.SetActive(false);

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

        for (int i = 0; i < 4; i++)
            hasDead[i] = false;

        float teamworkPercent = (float)playerDataProc.GetTeamworkValue() / 200.0f;
        GameObject.Find("Fill").GetComponent<Image>().transform.localScale
            = new Vector3(teamworkPercent, 1.0f, 1.0f);

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

        for (int i = 0; i < person.Count; i++)
        {
            //person[i].SetReputation(playerDataProc.GetPlayerAttr()[i].reputation);
            //person[i].SetWealth(playerDataProc.GetPlayerAttr()[i].money);
            person[i].SetWealthAndReputation(playerDataProc.GetPlayerAttr()[i].money,
                playerDataProc.GetPlayerAttr()[i].reputation);
        }

        //foreach (Image i in personImage)
        //{
        //    i.overrideSprite = Resources.Load("monkey", typeof(Sprite)) as Sprite;
        //}
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
            if (p == selectPerson && !IsHandlingDead())
                return false;
        }

        onEvent.SetSelectable();
        if (currentSelectedCount >= currentMaxSelectedPersonCount)
        {
            if (currentMaxSelectedPersonCount == 0)
                return false;
            person[selectedPerson[0] - 1].Clear();
            selectedPerson.RemoveAt(0);
            selectedPerson.Add(selectPerson);
            return true;
        }
        else
        {
            /*TODO: 增加如果选择数已达上限，则将未选择人物变灰的功能*/
            if (currentSelectedCount < currentMaxSelectedChooiceCount)
                currentSelectedCount++;
            selectedPerson.Add(selectPerson);
            return true;
        }
    }

    public void RemoveSelect(string personTag)
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
        for (int i = 0; i < selectedPerson.Count; i++)
        {
            if (selectedPerson[i] == selectPerson)
            {
                selectedPerson.RemoveAt(i);
                break;
            }
        }

        /*TODO: 增加如果原本选择数满上线，则将灰色的卡恢复彩色的功能*/
        currentSelectedCount--;
        if (currentSelectedCount == 0 && currentMaxSelectedPersonCount != 0)
            onEvent.SetUnselectable();
        if (currentMaxSelectedPersonCount == 0)
        {
            foreach (OnPerson pp in person)
                pp.SetUnselectable();
        }

        if (currentSelectedCount < 0)
            Debug.LogError("currentSelectedCount less than 0!");
    }

    void NextEvent()
    {
        /*TODO: 增加是否为下一关的判断*/
        selectedPerson.Clear();
        currentRound++;

        if (IsFinish())
            return;

        if (isInStory)
        {
            Debug.Log("in story or in jump story");
            int fatherEventID = currentEventID;
            //找出被选中的角色
            List<int> roles = new List<int>();
            for (int i = 0; i < person.Count; i++)
            {
                if (person[i].IsSelected()) roles.Add(i + 1);
            }
            if (lastChoice == 2)
            {
                choiceTypeOne = choiceTypeTwo;
            }
            currentEventID = loadRes.GetNextStoryEvent(currentStoryID, fatherEventID, lastChoice,
                choiceTypeOne, roles);

            Debug.Log("Story ID:" + currentStoryID.ToString() + " ,current event ID:" + currentEventID.ToString());
            //单条故事线结束，判断是否跳跳跳，以决定是否退出
            if (currentEventID == 0)
            {
                Debug.Log("a story end");
                isInStory = false;
                if (levelStoryID[currentLevel - 1].Count == 0)
                {
                    NextLevel();
                }
            } 
        }
        if (!isInStory)
        {
            if (IsFinish())
                return;
            int maxRange = levelEventID[currentLevel - 1].Count + levelStoryID[currentLevel - 1].Count;
            bool randomSuccess;
            do
            {
                randomSuccess = true;
                int idx = Random.Range(0, maxRange);
                //发生该关卡特有的事件
                if (idx < levelEventID[currentLevel - 1].Count)
                {
                    Debug.Log("in a normal event");
                    int preEvent = loadRes.GetPreEventID(levelEventID[currentLevel - 1][idx]);
                    Debug.Log("pre eventID:" + preEvent.ToString());
                    if (preEvent != 0 && !happenedEvent.Contains(preEvent))
                    {
                        randomSuccess = false;
                        Debug.Log("have pre event and pre event hasn't happen");
                    }
                    else
                    {
                        if (preEvent != 0)
                            Debug.Log("have pre event and pre event has happened");
                        else
                            Debug.Log("don't have per event");
                        currentEventID = levelEventID[currentLevel - 1][idx];
                        levelEventID[currentLevel - 1].RemoveAt(idx);
                    }
                }
                //生成故事
                else
                {
                    if (levelStoryID[currentLevel - 1].Count == 0)
                        return;
                    Debug.Log("a story start");
                    isInStory = true;
                    //永远都取出第一个故事
                    currentStoryID = levelStoryID[currentLevel - 1][0];
                    currentEventID = loadRes.GetStoryHeadEventID(currentStoryID);
                    levelStoryID[currentLevel - 1].RemoveAt(0);
                    currentStoryHead = currentEventID;
                    Debug.Log("the first event of the story:" + currentEventID.ToString());
                }
            } while (!randomSuccess);

        }

        /*TODO: 添加事件切换特效*/
        //为事件槽设置新的图片和文字描述
        Debug.Log("Next event: " + currentEventID.ToString());
        happenedEvent.Add(currentEventID);
        if (currentEventID == 0)
            Debug.LogError("Error");


        onEvent.SetEventTitle(loadRes.GetEventTitle(currentEventID));
        //设置该关卡的图片，文字，可选择角色等信息
        //int choiceTypeOne, choiceTypeTwo;
        choiceTypeOne = loadRes.GetChoiceType(currentEventID, 1);
        choiceTypeTwo = loadRes.GetChoiceType(currentEventID, 2);
        onEvent.SetEventText(loadRes.GetEventText(currentEventID));
        onEvent.SetEventID(currentEventID);
        onEvent.SetChoiceType(choiceTypeOne, choiceTypeTwo);
        Debug.Log("CT1:" + choiceTypeOne.ToString() + " ,CT2:" + choiceTypeTwo.ToString());
        onEvent.SetUnselectable();

        roleLimit = loadRes.GetRoleLimit(currentEventID);

        currentMaxSelectedPersonCount = loadRes.GetRoleCountLimit(currentEventID);
        if (currentMaxSelectedPersonCount == 0)
            onEvent.SetSelectable();

        //设置不能选的人的颜色
        foreach (OnPerson personTmp in person)
        {
            personTmp.Clear();
        }
        foreach (int pp in roleLimit)
        {
            person[pp - 1].SetUnselectable();
        }
        if (currentMaxSelectedPersonCount == 0)
        {
            foreach (OnPerson pp in person)
            {
                pp.SetUnselectable();
            }
        }
    }

    void GetCrrentEvent(int idx)
    {

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
                break;
            }
        }
    }

    public bool IsHandlingDead()
    {
        return handleDeadRemains > 0;
    }

    public void HandleDead()
    {
        int newDeadPerson = 0;
        for (int i = 0; i < 4; i++)
        {
            if (person[i].IsDead() != hasDead[i])
                newDeadPerson++;
        }
        for (int i = 0; i < 4; i++)
        {
            if (person[i].IsDead() != hasDead[i])
            {
                //GameObject.Find("DeadInterface").SetActive(true);
                deadInterface.SetActive(true);
                currentHanlingDeadPerson = i + 1;
                handleDeadRemains = newDeadPerson;
                if (currentMaxSelectedPersonCountCopy == 100)
                    currentMaxSelectedPersonCountCopy = currentMaxSelectedPersonCount;
                currentMaxSelectedPersonCount = 1;
                onEvent.SetChoiceType(1, 2);
                onEvent.SetUnselectable();
                foreach (OnPerson p in person)
                    p.Clear();
                break;
            }

        }
    }

    private void DeadConfirm(int saver, int choice)
    {
        foreach (OnPerson pp in person)
            pp.Clear();
        //不救，就存下来这个人已经死了
        if (choice == 2)
        {
            hasDead[currentHanlingDeadPerson - 1] = true;
            person[currentHanlingDeadPerson - 1].DeadDead();
        }
        //救了
        else
        {
            person[currentHanlingDeadPerson - 1].SetAlive();
            PlayerAttr[] tmpAttr = playerDataProc.HandleDeath(currentHanlingDeadPerson, saver);

            //person[currentHanlingDeadPerson - 1].SetWealthAndReputation(
            //    tmpAttr[currentHanlingDeadPerson - 1].money,
            //    tmpAttr[currentHanlingDeadPerson - 1].reputation);

            //person[saver - 1].SetWealthAndReputation(tmpAttr[saver - 1].money,
            //    tmpAttr[saver - 1].reputation);

            for (int i = 0; i < 4; i++)
            {
                person[i].SetWealthAndReputation(tmpAttr[i].money, tmpAttr[i].reputation);
            }

        }

        //没有新死的人了，就把死亡界面去掉
        handleDeadRemains--;
        if (handleDeadRemains == 0)
        {
            currentMaxSelectedPersonCount = currentMaxSelectedPersonCountCopy;
            currentMaxSelectedPersonCountCopy = 100;
            onEvent.SetEventText(loadRes.GetEventText(currentEventID));
            onEvent.SetEventID(currentEventID);
            onEvent.SetChoiceType(choiceTypeOne, choiceTypeTwo);
            onEvent.SetSelectable();
            foreach (OnPerson personTmp in person)
            {
                personTmp.Clear();
            }
            if (currentMaxSelectedPersonCount == 0)
            {
                foreach (OnPerson p in person)
                    p.SetUnselectable();
            }
            foreach (int pp in roleLimit)
            {
                person[pp - 1].SetUnselectable();
            }
            deadInterface.SetActive(false);
        }
    }

    public bool IsFinish()
    {
        if (currentLevel == levelCount && levelStoryID[currentLevel - 1].Count == 0)
            return true;
        return false;
    }
}
