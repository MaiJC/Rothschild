using Mono.Xml;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Xml;
using UnityEngine;


public class LoadRes : MonoBehaviour {

    // 1是唯key，2是泛key，3是通用
    public const int WEI_KEY_TYPE = 1;
    public const int FAN_KEY_TYPE = 2;
    public const int GENERAL_TYPE = 3;

    private XmlNode eventRootNode;
    private XmlNode uiResRootNode;
    private XmlNode storyEventRootNode;

 //   private int storySeq = 0;

    void LoadXml()
    {
        // 加载事件表
        string eventPath = Application.dataPath + "/Data/Xml/event.xml";
        XmlDocument eventDoc = new XmlDocument();
        eventDoc.Load(eventPath);
        eventRootNode = eventDoc.SelectSingleNode("TEventTable_Tab");

        // 加载UI资源表
        string uiResPath = Application.dataPath + "/Data/Xml/ui_res.xml";
        XmlDocument uiResDoc = new XmlDocument();
        uiResDoc.Load(uiResPath);
        uiResRootNode = uiResDoc.SelectSingleNode("TUIResTable_Tab");

        // 加载故事事件表
        string storyEventPath = Application.dataPath + "/Data/Xml/story_event.xml";
        XmlDocument storyEventDoc = new XmlDocument();
        storyEventDoc.Load(storyEventPath);
        storyEventRootNode = storyEventDoc.SelectSingleNode("TStoryEventTable_Tab");

    }

    /*******************事件表处理*********************/

    public List<int> GetCommonEventID()
    {
        List<int> comEventList = new List<int>();
        foreach (XmlElement item in eventRootNode)
        {
            int id = int.Parse(item.ChildNodes[GetIDIndex()].InnerText);
            int level = int.Parse(item.ChildNodes[GetLevelIndex()].InnerText);
            if (0 == level)  
            {
                if (!comEventList.Contains(id))
                {
                    comEventList.Add(id);
                }  
            }
        }
        return comEventList;
    }

    public int GetLevelCount()
    {
        int maxLevel = 0;
        foreach (XmlElement item in eventRootNode)
        {
            int level = int.Parse(item.ChildNodes[GetLevelIndex()].InnerText);
            if (level > maxLevel)
            {
                maxLevel = level;
            }
        }
        return maxLevel;
    }

    public List<int> GetLevelEventID(int levelIndex)
    {
        List<int> levelEventList = new List<int>();
        foreach (XmlElement item in eventRootNode)
        {
            int id = int.Parse(item.ChildNodes[GetIDIndex()].InnerText);
            int level = int.Parse(item.ChildNodes[GetLevelIndex()].InnerText);
            if (level == levelIndex)
            {
                if (!levelEventList.Contains(id))
                {
                    levelEventList.Add(id);
                }
            }
        }
        return levelEventList;
    }

    /************************************************/


    /*******************故事表处理*********************/

    public List<int> GetLevelStoryID(int levelIndex)
    {
        List<int> levelStoryList = new List<int>();
        foreach (XmlElement item in storyEventRootNode)
        {
            int storyID = int.Parse(item.ChildNodes[0].InnerText);
            int level = int.Parse(item.ChildNodes[1].InnerText);
            if (level == levelIndex)
            {
                if (!levelStoryList.Contains(storyID))
                {
                    levelStoryList.Add(storyID);
                }  
            }
        }
        return levelStoryList;
    }

    public int GetStoryHeadEventID(int storyID)
    {
        int headEventID = 0;

        foreach (XmlElement item in storyEventRootNode)
        {
            int iStoryID = int.Parse(item.ChildNodes[0].InnerText);
            int before = int.Parse(item.ChildNodes[4].InnerText);
            int next = int.Parse(item.ChildNodes[5].InnerText);

            if (storyID == iStoryID && 0 == before)
            {
                headEventID = next;
                break;
            }
        }

        return headEventID;
    }

    public int GetNextStoryEvent(int storyID, int fatherEventID, int eventChoice, List<int> roles)
    {
        int nextStoryEvent = 0;
        int keyCharacter = 2;   // 默认有key限制，但没匹配上

        if (0 != fatherEventID)
        {
            foreach (XmlElement item in eventRootNode)
            {
                int eventID = int.Parse(item.ChildNodes[GetIDIndex()].InnerText);
                int type = int.Parse(item.ChildNodes[GetTypeIndex()].InnerText);
                int key1 = int.Parse(item.ChildNodes[GetKey1Index()].InnerText);
                int key2 = int.Parse(item.ChildNodes[GetKey2Index()].InnerText);
  
                if (eventID == fatherEventID)   
                {
                    int roleNum = roles.Count;
                    if (WEI_KEY_TYPE == type)  // 唯key事件
                    {
                        if (1 == roleNum)   // 只选择一个人物
                        {
                            if (key1 == roles[0] && 0 == key2)
                            {
                                keyCharacter = 1;
                            }
                        }
                        else if (2 == roleNum) // 选择两个人物
                        {
                            if (roles.Contains(key1) && roles.Contains(key2))
                            {
                                keyCharacter = 1;
                            }
                        }
                    }
                    else if (FAN_KEY_TYPE == type) // 泛key事件
                    {
                        if (0 != key1)
                        {
                            if (0 != key2)
                            {
                                if (roles.Contains(key1) && roles.Contains(key2))
                                {
                                    keyCharacter = 1;
                                }
                            }
                            else
                            {
                                if (roles.Contains(key1))
                                {
                                    keyCharacter = 1;
                                }
                            }
                        }
                    }
                    else if (GENERAL_TYPE == type) // 通用事件
                    {
                        keyCharacter = 0;
                    }
                    break;
                }
            }
        }
        
        foreach (XmlElement item in storyEventRootNode)
        {
            int id = int.Parse(item.ChildNodes[0].InnerText);
            int character = int.Parse(item.ChildNodes[2].InnerText);
            int choice = int.Parse(item.ChildNodes[3].InnerText);
            int before = int.Parse(item.ChildNodes[4].InnerText);
            int next = int.Parse(item.ChildNodes[5].InnerText);
            if (id == storyID && before == fatherEventID && 
                choice == eventChoice && keyCharacter == character)
            {
                nextStoryEvent = next;
                break;
            }
        }
        return nextStoryEvent;
    }

    /***************************************************/

    /*********************UI资源表**********************/

    public string GetEventUIPath(int eventID)
    {
        string uiPath = "";
        foreach (XmlElement item in uiResRootNode)
        {
            int id = int.Parse(item.ChildNodes[0].InnerText);
            if (id == eventID)
            {
                uiPath = item.ChildNodes[1].InnerText;
                break;
            }
        }
        print("eventID: " + eventID + ", Picture: " + uiPath); 
        return uiPath;
    }

    public List<string> GetEventText(int eventID)
    {
        List<string> textList = new List<string>();

        foreach (XmlElement item in uiResRootNode)
        {
            int id = int.Parse(item.ChildNodes[0].InnerText);
            if (id == eventID)
            {
                string descText = item.ChildNodes[2].InnerText;
                string choice1Text = item.ChildNodes[3].InnerText;
                string choice2Text = item.ChildNodes[4].InnerText;

                textList.Add(descText);
                if (choice1Text.CompareTo("") != 0)
                {
                    textList.Add(choice1Text);
                } 
                if (choice2Text.CompareTo("") != 0)
                {
                    textList.Add(choice2Text);
                }
                break;
            }
        }

        if (0 == textList.Count)
        {
            print("!!!!!!!!!!!!GetEventText Error!!!!, eventID : " + eventID);
        }
        return textList;
    }

    /***************************************************/

    // Use this for initialization
    void Start () {

        LoadXml();

        GetEventText(100140002);
/*
        PrintLevelCount();
        PrintLevelStoryID(1);
        PrintLevelStoryID(2);

        PrintNextStoryID(1, 0, 0);
        PrintNextStoryID(1, 1, 1001);
        PrintNextStoryID(1, 2, 1002);
        PrintNextStoryID(1, 2, 1003);
        PrintNextStoryID(1, 3, 1004);
        print(GetEventUIPath(1004));
        print(GetEventText(1004)); 
*/
    }
	
	// Update is called once per frame
	void Update () {
		
	}


    int GetIDIndex()
    {
        return 0;
    }

    int GetTypeIndex()
    {
        return 1;
    }

    int GetRoleLimitIndex()
    {
        return 2;
    }

    int GetCountLimitIndex()
    {
        return 3;
    }

    int GetChoiceIndex()
    {
        return 4;
    }

    int GetKey1Index()
    {
        return 5;
    }

    int GetKey2Index()
    {
        return 6;
    }

    int GetLevelIndex()
    {
        return 7;
    }


    void PrintLevelCount()
    {
        print("\nLevel Cout: " + GetLevelCount() + "\n");
    }

    void PrintLevelEvent(int levelIndex)
    {
        print("\n-------Level " + levelIndex + " Event------\n");
        List<int> level1EventList = GetLevelEventID(levelIndex);
        foreach (int eventID in level1EventList)
        {
            print(eventID);
        }
    }

    void PrintLevelStoryID(int levelIndex)
    {
        print("\n-------Level " + levelIndex + " StoryID------\n");
        List<int> levelStoryList = GetLevelStoryID(levelIndex);
        foreach (int storyID in levelStoryList)
        {
            print(storyID);
        }
    }

    void PrintEventTable()
    {
        print("\n --------Event Table--------\n");

        foreach (XmlElement item in eventRootNode)
        {
            int id = int.Parse(item.ChildNodes[0].InnerText);
            int type = int.Parse(item.ChildNodes[1].InnerText);
            int key1 = int.Parse(item.ChildNodes[2].InnerText);
            int key2 = int.Parse(item.ChildNodes[3].InnerText);
            int level = int.Parse(item.ChildNodes[4].InnerText);
            int withkeyMoney = int.Parse(item.ChildNodes[5].InnerText);
            int withkeyReputation = int.Parse(item.ChildNodes[6].InnerText);
            int withkeyTeamwork = int.Parse(item.ChildNodes[7].InnerText);
            int withkeyCd = int.Parse(item.ChildNodes[8].InnerText);
            int withoutkeyMoney = int.Parse(item.ChildNodes[9].InnerText);
            int withoutkeyReputation = int.Parse(item.ChildNodes[10].InnerText);
            int withoutkeyTeamwork = int.Parse(item.ChildNodes[11].InnerText);
            int withoutkeyCd = int.Parse(item.ChildNodes[12].InnerText);
            print("ID: " + id + " Type: " + type + " Key1: " + key1 + " Key2: " + key2 + " Level: " + level +
                " withkeyMoney: " + withkeyMoney + " WithKeyReputation: " + withkeyReputation + " WithKeyTeamwork: " + withkeyTeamwork +
                " WithKeyCd: " + withkeyCd + " WithoutKeyMoney: " + withoutkeyMoney + " WithoutKeyReputation: " + withoutkeyReputation +
                " WithoutKeyTeamwork: " + withoutkeyTeamwork + " WithoutKeyCd: " + withoutkeyCd);
        }
    }

    //void PrintNextStoryID(int storyID, int storySeq, int fatherEventID)
    //{
    //    print("\n-------StoryID  " + storyID +  ", StorySeq " + storySeq  + ", Before " + fatherEventID + ", Next Event" + "-----\n");
    //    List<int> nextStoryList = GetNextStoryID(storyID, storySeq, fatherEventID);
    //    foreach (int eventID in nextStoryList)
    //    {
    //        print(eventID);
    //    }
    //}

}
