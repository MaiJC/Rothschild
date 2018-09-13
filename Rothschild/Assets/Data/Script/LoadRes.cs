using Mono.Xml;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Xml;
using UnityEngine;

public class LoadRes : MonoBehaviour {

    private XmlNode eventRootNode;
    private XmlNode uiResRootNode;
    private XmlNode storyEventRootNode;
    private XmlNode storyLevelRootNode;

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

        // 加载故事关卡表
        string storyLevelPath = Application.dataPath + "/Data/Xml/story_level.xml";
        XmlDocument storyLevelDoc = new XmlDocument();
        storyLevelDoc.Load(storyLevelPath);
        storyLevelRootNode = storyLevelDoc.SelectSingleNode("TStoryLevelTable_Tab");

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

    public List<int> GetCommonEventID()
    {
        List<int> comEventList = new List<int>();
        foreach (XmlElement item in eventRootNode)
        {
            int id = int.Parse(item.ChildNodes[0].InnerText);
            int type = int.Parse(item.ChildNodes[1].InnerText);
            if (0 == type)
            {
                comEventList.Add(id);
            }
        }
        return comEventList;
    }

    public int GetLevelCount()
    {
        int maxLevel = 0;
        foreach (XmlElement item in eventRootNode)
        {
            int level = int.Parse(item.ChildNodes[4].InnerText);
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
            int id = int.Parse(item.ChildNodes[0].InnerText);
            int level = int.Parse(item.ChildNodes[4].InnerText);
            if (level == levelIndex)
            {
                levelEventList.Add(id);
            }
        }
        return levelEventList;
    }

    public List<int> GetLevelStoryID(int levelIndex)
    {
        List<int> levelStoryList = new List<int>();
        foreach (XmlElement item in storyLevelRootNode)
        {
            int storyID = int.Parse(item.ChildNodes[0].InnerText);
            int level = int.Parse(item.ChildNodes[1].InnerText);
            if (level == levelIndex)
            {
                levelStoryList.Add(storyID);
            }
        }
        return levelStoryList;
    }

    public int GetNextStoryEvent(int storyID, int fatherEventID, int eventChoice)
    {
        int nextStoryEvent = 0;
        foreach (XmlElement item in storyEventRootNode)
        {
            int id = int.Parse(item.ChildNodes[0].InnerText);
            int choice = int.Parse(item.ChildNodes[1].InnerText);
            int before = int.Parse(item.ChildNodes[2].InnerText);
            int later = int.Parse(item.ChildNodes[3].InnerText);
            if (id == storyID && before == fatherEventID && choice == eventChoice)
            {
                nextStoryEvent = later;
            }
        }
        return nextStoryEvent;
    }

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
                textList.Add(choice1Text);
                textList.Add(choice2Text);
                break;
            }
        }

        return textList;
    }
    // Use this for initialization
    void Start () {

        LoadXml();
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

    void PrintNextStoryID(int storyID, int storySeq, int fatherEventID)
    {
        print("\n-------StoryID  " + storyID +  ", StorySeq " + storySeq  + ", Before " + fatherEventID + ", Next Event" + "-----\n");
        List<int> nextStoryList = GetNextStoryID(storyID, storySeq, fatherEventID);
        foreach (int eventID in nextStoryList)
        {
            print(eventID);
        }
    }

}
