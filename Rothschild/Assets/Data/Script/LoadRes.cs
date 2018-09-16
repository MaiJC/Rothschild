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
    private XmlNode itemRootNode;

 //   private int storySeq = 0;

    void LoadXml()
    {
        //if(Application.platform==RuntimePlatform.Android)

        {
            // 加载事件表
            TextAsset eventTextAsset = Resources.Load("event") as TextAsset;
            XmlDocument eventDoc = new XmlDocument();
            eventDoc.LoadXml(eventTextAsset.text);
            eventRootNode = eventDoc.SelectSingleNode("TEventTable_Tab");

            // 加载UI资源表
            TextAsset uiResTextAsset = Resources.Load("ui_res") as TextAsset;
            XmlDocument uiResDoc = new XmlDocument();
            uiResDoc.LoadXml(uiResTextAsset.text);
            uiResRootNode = uiResDoc.SelectSingleNode("TUIResTable_Tab");

            // 加载故事事件表
            TextAsset storyEventTextAsset = Resources.Load("story_event") as TextAsset;
            XmlDocument storyEventDoc = new XmlDocument();
            storyEventDoc.LoadXml(storyEventTextAsset.text);
            storyEventRootNode = storyEventDoc.SelectSingleNode("TStoryEventTable_Tab");

            // 加载道具表
            TextAsset itemTextAsset = Resources.Load("item") as TextAsset;
            XmlDocument itemDoc = new XmlDocument();
            itemDoc.LoadXml(itemTextAsset.text);
            itemRootNode = itemDoc.SelectSingleNode("TItemTable_Tab");
        }
        //else
        //{
        //    // 加载事件表
        //    string eventPath = Application.dataPath + "/Data/Xml/event.xml";
        //    XmlDocument eventDoc = new XmlDocument();
        //    eventDoc.Load(eventPath);
        //    eventRootNode = eventDoc.SelectSingleNode("TEventTable_Tab");

        //    // 加载UI资源表
        //    string uiResPath = Application.dataPath + "/Data/Xml/ui_res.xml";
        //    XmlDocument uiResDoc = new XmlDocument();
        //    uiResDoc.Load(uiResPath);
        //    uiResRootNode = uiResDoc.SelectSingleNode("TUIResTable_Tab");

        //    // 加载故事事件表
        //    string storyEventPath = Application.dataPath + "/Data/Xml/story_event.xml";
        //    XmlDocument storyEventDoc = new XmlDocument();
        //    storyEventDoc.Load(storyEventPath);
        //    storyEventRootNode = storyEventDoc.SelectSingleNode("TStoryEventTable_Tab");

        //    // 加载道具表
        //    string itemPath = Application.dataPath + "/Data/Xml/item.xml";
        //    XmlDocument itemDoc = new XmlDocument();
        //    itemDoc.Load(itemPath);
        //    itemRootNode = itemDoc.SelectSingleNode("TItemTable_Tab");
        //}
        
    }

    /*******************事件表处理*********************/

    public List<int> GetCommonEventID()
    {
        List<int> comEventList = new List<int>();
        foreach (XmlElement item in eventRootNode)
        {
            int id = int.Parse(item.ChildNodes[GetIDIndex()].InnerText);
            int level = int.Parse(item.ChildNodes[GetLevelIndex()].InnerText);
            if (0 == level)  // 可出现在任意关卡
            {
                if (!comEventList.Contains(id))
                {
                    comEventList.Add(id);
                }  
            }
        }
        return comEventList;
    }

    public int GetChoiceType(int eventID, int eventChoice)
    {
        int choiceType = 0;
        foreach (XmlElement item in eventRootNode)
        {
            int id = int.Parse(item.ChildNodes[GetIDIndex()].InnerText);
            int choice = int.Parse(item.ChildNodes[GetChoiceIndex()].InnerText);
            if (id == eventID && choice == eventChoice)
            {
                choiceType = int.Parse(item.ChildNodes[GetChoiceTypeIndex()].InnerText);
                break;
            }
        }

        return choiceType;
    }

    public int GetPreEventID(int eventID)
    {
        int preEventID = 0;
        foreach (XmlElement item in eventRootNode)
        {
            int id = int.Parse(item.ChildNodes[GetIDIndex()].InnerText);
            if (id == eventID)
            {
                preEventID = int.Parse(item.ChildNodes[GetPreEventIndex()].InnerText);
                break;
            }
        }

        return preEventID;
    }

    public bool IsStroryEvent(int eventID)
    {
        foreach (XmlElement item in eventRootNode)
        {
            int id = int.Parse(item.ChildNodes[GetIDIndex()].InnerText);
            if (id == eventID)
            {
                int ifStory = int.Parse(item.ChildNodes[GetIfStoryIndex()].InnerText);
                if (1 == ifStory)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        return false;
    }

    public int GetRoleCountLimit(int eventID)
    {
        int roleCountLimit = 0;
        foreach (XmlElement item in eventRootNode)
        {
            int id = int.Parse(item.ChildNodes[GetIDIndex()].InnerText);
            if (id == eventID) 
            {
                roleCountLimit = int.Parse(item.ChildNodes[GetCountLimitIndex()].InnerText);
                break;                
            }
        }

        return roleCountLimit;
    }

    public List<int> GetRoleLimit(int eventID)
    {
        List<int> roleLimitList = new List<int>();

        foreach (XmlElement item in eventRootNode)
        {
            int id = int.Parse(item.ChildNodes[GetIDIndex()].InnerText);
            if (id == eventID)
            {
                string roleLimitStr = item.ChildNodes[GetRoleLimitIndex()].InnerText;
                if (roleLimitStr.CompareTo("") != 0)
                {
                    string[] sRoleLimit = roleLimitStr.Split(',');
                    foreach (string roleLimit in sRoleLimit)
                    {
                        if (roleLimit.CompareTo("") != 0)
                        {
                            roleLimitList.Add(int.Parse(roleLimit));
                        }   
                    }
                }
                break;
            }
        }

        return roleLimitList;
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

    public int GetNextStoryEvent(int storyID, int fatherEventID, int eventChoice, int eventChoiceType, List<int> roles)
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
                    if (2 == eventChoiceType)
                    {
                        keyCharacter = 1;
                        break;
                    }

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
                    else  // 通用事件
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

            if (id == storyID && before == fatherEventID)
            {
                if (0 == before)
                {
                    nextStoryEvent = next;
                    break;
                }
                else if (choice == eventChoice && keyCharacter == character)
                {
                    nextStoryEvent = next;
                    break;
                }

            }
        }

        if (0 == nextStoryEvent)
        {
            print("Get Next Stroy Event Error!!!!");

            print("keyCharacter: " + keyCharacter);
            print("storyID: " + storyID + " befor: " + fatherEventID + " eventChoice: " + eventChoice);
            print("select roles: ");
            foreach (int roleId in roles)
            {
                print(roleId);
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
    //    print("eventID: " + eventID + ", Picture: " + uiPath); 
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

    public string GetEventDesc(int eventID)
    {
        string descText = "";
        foreach (XmlElement item in uiResRootNode)
        {
            int id = int.Parse(item.ChildNodes[0].InnerText);
            if (id == eventID)
            {
                descText = item.ChildNodes[2].InnerText;
                break;
            }
        }
        return descText;
    }

    public string GetEventChoiceText(int eventID, int choice)
    {
        string choiceText = "";
        foreach (XmlElement item in uiResRootNode)
        {
            int id = int.Parse(item.ChildNodes[0].InnerText);
            if (id == eventID)
            {
                if (1 == choice)
                {
                    choiceText = item.ChildNodes[3].InnerText;
                }
                else if (2 == choice)
                {
                    choiceText = item.ChildNodes[4].InnerText;
                }
                else
                {
                    print("choice error!!!, choice: " + choice);
                }
                break;
            }
        }
        return choiceText;
    }

    public string GetEventTitle(int eventID)
    {
        string eventTitle = "";
        foreach (XmlElement item in uiResRootNode)
        {
            int id = int.Parse(item.ChildNodes[0].InnerText);
            if (id == eventID)
            {
                eventTitle = item.ChildNodes[5].InnerText;               
                break;
            }
        }

        return eventTitle;
    }
    /***************************************************/



    /**********************道具表***********************/
    string GetItemPicture(int itemID)
    {
        string picture = "";

        foreach (XmlElement item in itemRootNode)
        {
            int id = int.Parse(item.ChildNodes[0].InnerText);
            if (id == itemID)  
            {
                picture = item.ChildNodes[1].InnerText;
                break;
            }
        }
        
        return picture;
    }
            
    /**************************************************/


    // Use this for initialization
    void Start () {

        LoadXml();

        print(GetItemPicture(2));

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

    int GetPreEventIndex()
    {
        return 2;
    }

    int GetIfStoryIndex()
    {
        return 3;
    }

    int GetRoleLimitIndex()
    {
        return 4;
    }

    int GetCountLimitIndex()
    {
        return 5;
    }

    int GetChoiceIndex()
    {
        return 6;
    }

    int GetKey1Index()
    {
        return 7;
    }

    int GetKey2Index()
    {
        return 8;
    }

    int GetChoiceTypeIndex()
    {
        return 9;
    }

    int GetLevelIndex()
    {
        return 10;
    }

    int GetWithKeyMoneyIndex()
    {
        return 11;
    }

    int GetWithKeyReputationIndex()
    {
        return 12;
    }

    int GetWithKeyTeamworkIndex()
    {
        return 13;
    }

    int GetWithoutKeyMoneyIndex()
    {
        return 14;
    }

    int GetWithoutKeyReputationIndex()
    {
        return 15;
    }

    int GetWithoutKeyTeamworkIndex()
    {
        return 16;
    }
}
