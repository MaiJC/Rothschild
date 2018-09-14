using Mono.Xml;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Xml;
using UnityEngine;


public struct PlayerData  //玩家信息
{
    public string name;
    public string password;

    public int level;
    public int section;
    public PlayerAttr[] playerAttr;
}

public struct PlayerAttr
{
    public int money;
    public int reputation;
}

public struct EventLog
{
    public int roleID;
    public int eventID;
    public int choice;
}


public struct RoleEventStat
{
    public int eventID;
    public int choice;
    public List<int> roles;
}

public class PlayerDataProc : MonoBehaviour
{
    // 1是唯key，2是泛key，3是通用
    public const int WEI_KEY_TYPE = 1;
    public const int FAN_KEY_TYPE = 2;
    public const int GENERAL_TYPE = 3;

    public static int teamworkValue = 0;
    public static List<RoleEventStat> roleEventStats = new List<RoleEventStat>();

    string playerDBPath = "/Data/Xml/palyerDB.xml";
    string eventTablePath = "/Data/Xml/event.xml";
    string specialEventTablePath = "/Data/Xml/special_event.xml";

    string storySettleTablePath = "/Data/Xml/story_settle.xml";

    PlayerAttr[] settleResult = new PlayerAttr[4];

    public void CreatePlayerDB()
    {
        string path = Application.dataPath + playerDBPath;
        if (!File.Exists(path))
        {
            //创建最上一层的节点。
            XmlDocument xml = new XmlDocument();
            XmlElement root = xml.CreateElement("Players");
            xml.AppendChild(root);
            //最后保存文件
            xml.Save(path);
        }
    }

    public bool IsJumpStory(int storyID)
    {
        bool jumpStory = false;
        string path = Application.dataPath + storySettleTablePath;
        if (File.Exists(path))
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(path);
            XmlNodeList xmlNodeList = xml.SelectSingleNode("TStorySettleTable_Tab").ChildNodes;
            foreach (XmlElement xl1 in xmlNodeList)
            {
                int iStrotyID = int.Parse(xl1.ChildNodes[0].InnerText);
                int settleType = int.Parse(xl1.ChildNodes[1].InnerText);
                if (iStrotyID == storyID)
                {
                    if (2 == settleType)
                    {
                        jumpStory = true;
                    }
                    break;
                }
            }
        }

        return jumpStory;
    }

    public bool IsPreStroy(int storyID, ref int preEvent, ref int roleID)
    {
        bool preStroy = false;

        string path = Application.dataPath + storySettleTablePath;
        if (File.Exists(path))
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(path);
            XmlNodeList xmlNodeList = xml.SelectSingleNode("TStorySettleTable_Tab").ChildNodes;
            foreach (XmlElement xl1 in xmlNodeList)
            {
                int iStrotyID = int.Parse(xl1.ChildNodes[0].InnerText);
                int settleType = int.Parse(xl1.ChildNodes[1].InnerText);
                if (iStrotyID == storyID)
                {
                    if (1 == settleType)
                    {
                        preStroy = true;
                        preEvent = int.Parse(xl1.ChildNodes[2].InnerText);
                        roleID = int.Parse(xl1.ChildNodes[3].InnerText);
                    }
                    break;
                }
            }
        }

        return preStroy;
    }

    public bool FindPlayer(string name)
    {
        string path = Application.dataPath + playerDBPath;
        if (File.Exists(path))
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(path);
            XmlNodeList xmlNodeList = xml.SelectSingleNode("Players").ChildNodes;
            foreach (XmlElement xl1 in xmlNodeList)
            {
                if (0 == name.CompareTo(xl1.ChildNodes[0].InnerText))
                {
                    return true;
                }
            }
        }
        return false;
    }


    public int regPlayerData(string name, string password)
    {
        if (FindPlayer(name))
        {
            print("玩家已存在！！！");
            return 1;
        }
        else
        {
            string path = Application.dataPath + playerDBPath;
            if (File.Exists(path))
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(path);
                XmlNode root = xml.SelectSingleNode("Players");

                XmlElement element = xml.CreateElement("Player");

                XmlElement elementName = xml.CreateElement("Name");
                elementName.InnerText = name;

                XmlElement elementPassword = xml.CreateElement("Password");
                elementPassword.InnerText = password;

                XmlElement elementLevel = xml.CreateElement("Level");
                elementLevel.InnerText = "1";

                XmlElement elementSection = xml.CreateElement("Section");
                elementSection.InnerText = "1";

                XmlElement elementTeamwork = xml.CreateElement("Teamwork");
                elementTeamwork.InnerText = "0";

                XmlElement elementMoney = xml.CreateElement("Money");
                elementMoney.InnerText = "";

                XmlElement elementReputation = xml.CreateElement("Reputation");
                elementReputation.InnerText = "";

                // 新增玩家的初始已选择的事件集合为空
                XmlElement elementEventLog = xml.CreateElement("EventLog");
                elementEventLog.InnerText = "";

                //把节点一层一层的添加至xml中，注意他们之间的先后顺序，这是生成XML文件的顺序
                element.AppendChild(elementName);
                element.AppendChild(elementPassword);
                element.AppendChild(elementLevel);
                element.AppendChild(elementSection);
                element.AppendChild(elementTeamwork);
                element.AppendChild(elementMoney);
                element.AppendChild(elementReputation);
                element.AppendChild(elementEventLog);

                root.AppendChild(element);

                xml.AppendChild(root);
                //最后保存文件
                xml.Save(path);
            }
        }
        return 0;
    }

    public PlayerAttr[] GetPlayerAttr()
    {
        PlayerAttr[] playerAttrs = new PlayerAttr[4];

        for (int i = 0; i < 4; i++)
        {
            playerAttrs[i].money = settleResult[i].money;
            playerAttrs[i].reputation = settleResult[i].reputation;
        }

        return playerAttrs;
    }

    public bool EventHaveOccur(int eventID)
    {
        bool occurFlag = false;

        foreach (RoleEventStat eventStat in roleEventStats)
        {
            if (eventID == eventStat.eventID)
            {
                occurFlag = true;
                break;
            }
        }

        return occurFlag;
    }

    public List<EventLog> GetEventLog(string name)
    {
        string path = Application.dataPath + playerDBPath;

        List<EventLog> eventList = new List<EventLog>();
        EventLog eventLogResult = new EventLog();
        string eventStr = "";

        if (File.Exists(path))
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(path);
            XmlNodeList xmlNodeList = xml.SelectSingleNode("Players").ChildNodes;
            foreach (XmlElement xl1 in xmlNodeList)
            {
                if (0 == name.CompareTo(xl1.ChildNodes[0].InnerText))
                {
                    eventStr = xl1.ChildNodes[7].InnerText;
                    break;
                }
            }
        }
        print(eventStr);

        if (eventStr.CompareTo("") != 0)
        {
            string[] sEvent = eventStr.Split(',');
            foreach (string eventLog in sEvent)
            {
                print(eventLog);
                if (eventLog.CompareTo("") != 0)
                {
                    string[] sEventlog = eventLog.Split('/');

                    int flag = 0;
                    foreach (string eventLogItem in sEventlog)
                    {
                        if (0 == flag)
                            eventLogResult.roleID = int.Parse(eventLogItem);
                        else if (1 == flag)
                            eventLogResult.eventID = int.Parse(eventLogItem);
                        else if (2 == flag)
                            eventLogResult.choice = int.Parse(eventLogItem);

                        flag++;
                    }

                    eventList.Add(eventLogResult);
                }
            }
        }

        return eventList;
    }

    public int AddEventLog(string name, EventLog eventlog)
    {
        string path = Application.dataPath + playerDBPath;

        if (File.Exists(path))
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(path);
            XmlNodeList xmlNodeList = xml.SelectSingleNode("Players").ChildNodes;
            foreach (XmlElement xl1 in xmlNodeList)
            {
                if (0 == name.CompareTo(xl1.ChildNodes[0].InnerText))
                {
                    string eventStr = xl1.ChildNodes[7].InnerText;
                    if (eventStr.CompareTo("") != 0)
                    {
                        eventStr += ",";
                    }
                    print(eventStr);
                    eventStr += eventlog.roleID.ToString() + "/" + eventlog.eventID.ToString() + "/" + eventlog.choice.ToString();
                    print(eventlog.roleID);
                    print(eventlog.eventID);
                    print(eventlog.choice);
                    print(eventStr);

                    xl1.ChildNodes[7].InnerText = eventStr;
                    xml.Save(path);
                    return 0;
                }
            }
        }
        return 1;
    }

    public int AddEvenLogList(string name, List<EventLog> eventLogList)
    {
        foreach (EventLog eventLog in eventLogList)
        {
            if (AddEventLog(name, eventLog) != 0)
            {
                return 1;
            }
        }
        return 0;
    }

    public int GetTeamworkValue()
    {
        return teamworkValue;
    }

    public bool KeyMatch(int key1, int key2, int type, List<int> selectRoles)
    {
        bool keyFlag = false;
        int roleNum = selectRoles.Count;

        if (WEI_KEY_TYPE == type) // 唯key事件
        {
            if (key2 != 0)   // 双key
            {
                if (roleNum == 2 && selectRoles.Contains(key1) && selectRoles.Contains(key2))
                {
                    keyFlag = true;
                }
            }
            else    // 单key
            {
                if (roleNum == 1 && selectRoles[0] == key1)
                {
                    keyFlag = true;
                }
            }
        }
        else if (FAN_KEY_TYPE == type) // 范key事件
        {
            if (key2 != 0)  // 双key
            {
                if (selectRoles.Contains(key1) && selectRoles.Contains(key2))
                {
                    keyFlag = true;
                }
            }
            else    // 单key
            {
                if (selectRoles.Contains(key1))
                {
                    keyFlag = true;
                }
            }
        }

        return keyFlag;
        
    }

    public void AddEventLogStat(int eventID, int eventChoice, List<int> selectRoles)
    {
        RoleEventStat eventStat = new RoleEventStat();
        eventStat.eventID = eventID;
        eventStat.choice = eventChoice;


        eventStat.roles = new List<int>();
        foreach (int roleID in selectRoles)
        {
            eventStat.roles.Add(roleID);
        }

        roleEventStats.Add(eventStat);
    }

    public RoleEventStat GetEventLogStat(int eventID)
    {
        RoleEventStat eventStat = new RoleEventStat();
        eventStat.eventID = 0;

        foreach (RoleEventStat stat in roleEventStats)
        {
            if (eventID == stat.eventID)
            {
                eventStat.eventID = stat.eventID;
                eventStat.choice = stat.choice;
                eventStat.roles = new List<int>();
                foreach (int role in stat.roles)
                {
                    eventStat.roles.Add(role);
                }
            }
        }

        return eventStat;
    }

    void SettleType1(int eventID, int eventChoice, int specialRoleID)
    {
        string path = Application.dataPath + eventTablePath;
        if (File.Exists(path))
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(path);
            XmlNodeList xmlNodeList = xml.SelectSingleNode("TEventTable_Tab").ChildNodes;
            foreach (XmlElement xl1 in xmlNodeList)
            {
                int id = int.Parse(xl1.ChildNodes[GetIDIndex()].InnerText);
                int choice = int.Parse(xl1.ChildNodes[GetChoiceIndex()].InnerText);

                if (id == eventID && choice == eventChoice)
                {
                    int money = int.Parse(xl1.ChildNodes[GetWithKeyMoneyIndex()].InnerText);
                    int reputation = int.Parse(xl1.ChildNodes[GetWithKeyReputationIndex()].InnerText);
                    int teamWork = int.Parse(xl1.ChildNodes[GetWithKeyTeamworkIndex()].InnerText);

                    settleResult[specialRoleID - 1].money += money;
                    settleResult[specialRoleID - 1].reputation += reputation;
                    teamworkValue += teamWork;

                    break;
                }
            }
        }
    }
    void SettleType2(int eventID, int eventChoice, int roleNumThre, List<int> selectRoles)
    {
        string path = Application.dataPath + eventTablePath;
        if (File.Exists(path))
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(path);
            XmlNodeList xmlNodeList = xml.SelectSingleNode("TEventTable_Tab").ChildNodes;
            foreach (XmlElement xl1 in xmlNodeList)
            {
                int id = int.Parse(xl1.ChildNodes[GetIDIndex()].InnerText);
                int choice = int.Parse(xl1.ChildNodes[GetChoiceIndex()].InnerText);

                if (id == eventID && choice == eventChoice)
                {
                    int money = int.Parse(xl1.ChildNodes[GetWithKeyMoneyIndex()].InnerText);
                    int reputation = int.Parse(xl1.ChildNodes[GetWithKeyReputationIndex()].InnerText);
                    int teamWork = int.Parse(xl1.ChildNodes[GetWithKeyTeamworkIndex()].InnerText);

                    foreach (int roleID in selectRoles)
                    {
                        if (roleID < 1 || roleID > 4)
                        {
                            print("SettleType2: roleID error, roleID: " + roleID);
                            return;
                        }
                        settleResult[roleID - 1].money += money;
                        settleResult[roleID - 1].reputation += reputation;
                    }

                    if (selectRoles.Count >= roleNumThre)
                    {
                        teamworkValue += teamWork;
                    }
                 
                    break;
                }
            }
        }
    }

    void SettleType3(int eventID, int eventChoice, int preEvent)
    {
        RoleEventStat preEventStat = GetEventLogStat(preEvent);
        if (0 == preEventStat.eventID)
        {
            print("error!!!! event: " + eventID + " it's pre event: " + preEvent + "not occurr!!!");
            return;
        }

        List<int> preEventRoles = preEventStat.roles;

        string path = Application.dataPath + eventTablePath;
        if (File.Exists(path))
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(path);
            XmlNodeList xmlNodeList = xml.SelectSingleNode("TEventTable_Tab").ChildNodes;
            foreach (XmlElement xl1 in xmlNodeList)
            {
                int id = int.Parse(xl1.ChildNodes[GetIDIndex()].InnerText);
                int choice = int.Parse(xl1.ChildNodes[GetChoiceIndex()].InnerText);

                if (id == eventID && choice == eventChoice)
                {
                    int money = int.Parse(xl1.ChildNodes[GetWithKeyMoneyIndex()].InnerText);
                    int reputation = int.Parse(xl1.ChildNodes[GetWithKeyReputationIndex()].InnerText);
                    int teamwork = int.Parse(xl1.ChildNodes[GetWithKeyTeamworkIndex()].InnerText);

                    foreach (int roleID in preEventRoles)
                    {
                        if (roleID < 1 || roleID > 4)
                        {
                            print("SettleType3: roleID error, roleID: " + roleID);
                            return;
                        }

                        settleResult[roleID - 1].money += money;
                        settleResult[roleID - 1].reputation += reputation;
                    }
                    teamworkValue += teamwork;
                    break;
                }
            }
        }
    }

    void SettleType4(int eventID, int eventChoice, List<int> selectRoles, int preEvent, int preChoice)
    {
        RoleEventStat preEventStat = GetEventLogStat(preEvent);
        if (0 == preEventStat.eventID)
        {
            print("error!!!! event: " + eventID + " it's pre event: " + preEvent + "not occurr!!!");
            return;
        }

        string path = Application.dataPath + eventTablePath;
        if (File.Exists(path))
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(path);
            XmlNodeList xmlNodeList = xml.SelectSingleNode("TEventTable_Tab").ChildNodes;
            foreach (XmlElement xl1 in xmlNodeList)
            {
                int id = int.Parse(xl1.ChildNodes[GetIDIndex()].InnerText);
                int choice = int.Parse(xl1.ChildNodes[GetChoiceIndex()].InnerText);

                if (id == eventID && choice == eventChoice)
                {
                    int money = 0;
                    int reputation = 0;
                    int teamwork = 0;
                    if (eventChoice == preChoice)
                    {
                        money = int.Parse(xl1.ChildNodes[GetWithKeyMoneyIndex()].InnerText);
                        reputation = int.Parse(xl1.ChildNodes[GetWithKeyReputationIndex()].InnerText);
                        teamwork = int.Parse(xl1.ChildNodes[GetWithKeyTeamworkIndex()].InnerText);
                    }
                    else
                    {
                        money = int.Parse(xl1.ChildNodes[GetWithoutKeyMoneyIndex()].InnerText);
                        reputation = int.Parse(xl1.ChildNodes[GetWithoutKeyReputationIndex()].InnerText);
                        teamwork = int.Parse(xl1.ChildNodes[GetWithoutKeyTeamworkIndex()].InnerText);
                    }
                
                    foreach (int roleID in selectRoles)
                    {
                        if (roleID < 1 || roleID > 4)
                        {
                            print("SettleType4: roleID error, roleID: " + roleID);
                            return;
                        }

                        settleResult[roleID - 1].money += money;
                        settleResult[roleID - 1].reputation += reputation;
                    }
                    teamworkValue += teamwork;
                    break;
                }
            }
        }

    }
    void SettleType5(int eventID, int eventChoice, int preEvent, int specialRoleID)
    {
        RoleEventStat preEventStat = GetEventLogStat(preEvent);
        if (0 == preEventStat.eventID)
        {
            print("error!!!! event: " + eventID + " it's pre event: " + preEvent + "not occurr!!!");
            return;
        }

        List<int> preEventRoles = preEventStat.roles;

        string path = Application.dataPath + eventTablePath;
        if (File.Exists(path))
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(path);
            XmlNodeList xmlNodeList = xml.SelectSingleNode("TEventTable_Tab").ChildNodes;
            foreach (XmlElement xl1 in xmlNodeList)
            {
                int id = int.Parse(xl1.ChildNodes[GetIDIndex()].InnerText);
                int choice = int.Parse(xl1.ChildNodes[GetChoiceIndex()].InnerText);

                if (id == eventID && choice == eventChoice)
                {
                    int money = int.Parse(xl1.ChildNodes[GetWithKeyMoneyIndex()].InnerText);
                    int reputation = int.Parse(xl1.ChildNodes[GetWithKeyReputationIndex()].InnerText);
                    int teamwork = int.Parse(xl1.ChildNodes[GetWithKeyTeamworkIndex()].InnerText);

                    foreach (int roleID in preEventRoles)
                    {
                        if (roleID < 1 || roleID > 4)
                        {
                            print("SettleType5: roleID error, roleID: " + roleID);
                            return;
                        }

                        settleResult[roleID - 1].money += (int)(money * 0.7);
                        settleResult[roleID - 1].reputation += (int)(reputation * 0.7);
                    }

                    if (specialRoleID < 1 || specialRoleID > 4)
                    {
                        print("SettleType5: specialRoleID error, specialRoleID: " + specialRoleID);
                        return;
                    }
                    settleResult[specialRoleID - 1].money += (int)(money * 0.7);
                    settleResult[specialRoleID - 1].reputation += (int)(reputation * 0.7);

                    teamworkValue += teamwork;
                    break;
                }
            }
        }

    }

    void SettleType6(int eventID, int eventChoice)
    {
        string path = Application.dataPath + eventTablePath;
        if (File.Exists(path))
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(path);
            XmlNodeList xmlNodeList = xml.SelectSingleNode("TEventTable_Tab").ChildNodes;
            foreach (XmlElement xl1 in xmlNodeList)
            {
                int id = int.Parse(xl1.ChildNodes[GetIDIndex()].InnerText);
                int choice = int.Parse(xl1.ChildNodes[GetChoiceIndex()].InnerText);

                if (id == eventID && choice == eventChoice)
                {
                    int money = int.Parse(xl1.ChildNodes[GetWithKeyMoneyIndex()].InnerText);
                    int reputation = int.Parse(xl1.ChildNodes[GetWithKeyReputationIndex()].InnerText);
                    int teamwork = int.Parse(xl1.ChildNodes[GetWithKeyTeamworkIndex()].InnerText);

                    for (int i = 0; i < 4; i++)
                    {
                        settleResult[i].money += money;
                        settleResult[i].reputation += reputation;
                    }

                    teamworkValue += teamwork;
                    break;
                }
            }
        }
    }

    bool SpeicialSettle(int eventID, int eventChoice, List<int> selectRoles)
    {
        bool isSpeicial = false;

        string path = Application.dataPath + specialEventTablePath;
        if (File.Exists(path))
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(path);
            XmlNodeList xmlNodeList = xml.SelectSingleNode("TSpecialEventTable_Tab").ChildNodes;
            foreach (XmlElement xl1 in xmlNodeList)
            {
                int id = int.Parse(xl1.ChildNodes[0].InnerText);
                int settleType = int.Parse(xl1.ChildNodes[1].InnerText);
                if (id == eventID)
                {
                    switch (settleType)
                    {
                        case 1:
                            int specialRoleID = int.Parse(xl1.ChildNodes[2].InnerText);
                            SettleType1(eventID, eventChoice, specialRoleID);
                            break;
                        case 2:
                            int roleNumThre = int.Parse(xl1.ChildNodes[2].InnerText);
                            SettleType2(eventID, eventChoice, roleNumThre, selectRoles);
                            break;
                        case 3:
                            int preEvent = int.Parse(xl1.ChildNodes[2].InnerText);
                            SettleType3(eventID, eventChoice, preEvent);
                            break;
                        case 4:
                            int preEvent1 = int.Parse(xl1.ChildNodes[2].InnerText);
                            int preChoice = int.Parse(xl1.ChildNodes[3].InnerText);
                            SettleType4(eventID, eventChoice, selectRoles, preEvent1, preChoice);
                            break;
                        case 5:
                            int preEvent2 = int.Parse(xl1.ChildNodes[2].InnerText);
                            int specialRoleID1 = int.Parse(xl1.ChildNodes[3].InnerText);
                            SettleType5(eventID, eventChoice, preEvent2, specialRoleID1);
                            break;
                        case 6:
                            SettleType6(eventID, eventChoice);
                            break;
                    }
                    return true;
                }
            }
        }

        return isSpeicial;
    }

    public PlayerAttr[] SettlePlayer(int eventID, int eventChoice, List<int> selectRoles) 
    {
        string path = Application.dataPath + eventTablePath;

        bool keyFlag = false;

        int money = 0;
        int reputation = 0;
        int teamWork = 0;

        AddEventLogStat(eventID, eventChoice, selectRoles);

        if (SpeicialSettle(eventID, eventChoice, selectRoles))
        {
            return settleResult;
        }

        if (File.Exists(path))
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(path);
            XmlNodeList xmlNodeList = xml.SelectSingleNode("TEventTable_Tab").ChildNodes;
            foreach (XmlElement xl1 in xmlNodeList)
            {
                int id = int.Parse(xl1.ChildNodes[GetIDIndex()].InnerText);
                int type = int.Parse(xl1.ChildNodes[GetTypeIndex()].InnerText);
                int choice = int.Parse(xl1.ChildNodes[GetChoiceIndex()].InnerText);
                int key1 = int.Parse(xl1.ChildNodes[GetKey1Index()].InnerText);
                int key2 = int.Parse(xl1.ChildNodes[GetKey2Index()].InnerText);

                if (eventID == id && eventChoice == choice)
                {
                    if (key1 != 0)  // 有key事件
                    {
                        keyFlag = KeyMatch(key1, key2, type, selectRoles);
                        if (keyFlag)    // key成立，结算
                        {
                            money = int.Parse(xl1.ChildNodes[GetWithKeyMoneyIndex()].InnerText);
                            reputation = int.Parse(xl1.ChildNodes[GetWithKeyReputationIndex()].InnerText);
                            teamWork = int.Parse(xl1.ChildNodes[GetWithKeyTeamworkIndex()].InnerText);
                        }
                        else   // key不成立，结算
                        {
                            money = int.Parse(xl1.ChildNodes[GetWithoutKeyMoneyIndex()].InnerText);
                            reputation = int.Parse(xl1.ChildNodes[GetWithoutKeyReputationIndex()].InnerText);
                            teamWork = int.Parse(xl1.ChildNodes[GetWithoutKeyTeamworkIndex()].InnerText);
                        }

                        if (WEI_KEY_TYPE == type)
                        {
                            if (1 == selectRoles.Count)
                            {
                                settleResult[selectRoles[0] - 1].money += money;
                                settleResult[selectRoles[0] - 1].reputation += reputation;
                            }
                            else if (2 == selectRoles.Count)
                            {
                                settleResult[selectRoles[0] - 1].money += (int)(money * 0.7);
                                settleResult[selectRoles[1] - 1].money += (int)(money * 0.7);
                              
                                settleResult[selectRoles[0] - 1].reputation += (int)(reputation * 0.7);
                                settleResult[selectRoles[1] - 1].reputation += (int)(reputation * 0.7);
                            }
                        }
                        else if (FAN_KEY_TYPE == type)
                        {
                            if (0 == key2)  // 事件是单key
                            {
                                if (1 == selectRoles.Count)
                                {
                                    settleResult[selectRoles[0] - 1].money += money;
                                    settleResult[selectRoles[0] - 1].reputation += reputation;
                                }
                                else if (2 == selectRoles.Count)
                                {
                                    if (keyFlag)    // 选对了
                                    {
                                        foreach (int roleID in selectRoles)
                                        {
                                            if (roleID == key1)     // 对的人80%
                                            {
                                                settleResult[roleID - 1].money += (int)(money * 0.8);
                                                settleResult[roleID - 1].reputation += (int)(reputation * 0.8);
                                            }
                                            else  // 错的人20%
                                            {
                                                settleResult[roleID - 1].money += (int)(money * 0.2);
                                                settleResult[roleID - 1].reputation += (int)(reputation * 0.2);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        settleResult[selectRoles[0] - 1].money += (int)(money * 0.7);
                                        settleResult[selectRoles[1] - 1].money += (int)(money * 0.7);

                                        settleResult[selectRoles[0] - 1].reputation += (int)(reputation * 0.7);
                                        settleResult[selectRoles[1] - 1].reputation += (int)(reputation * 0.7);
                                    }
                                }
                            }
                            else     // 双key
                            {
                                if (1 == selectRoles.Count)
                                {
                                    settleResult[selectRoles[0] - 1].money += money;
                                    settleResult[selectRoles[0] - 1].reputation += reputation;
                                }
                                else if (2 == selectRoles.Count)
                                {
                                    settleResult[selectRoles[0] - 1].money += (int)(money * 0.7);
                                    settleResult[selectRoles[1] - 1].money += (int)(money * 0.7);

                                    settleResult[selectRoles[0] - 1].reputation += (int)(reputation * 0.7);
                                    settleResult[selectRoles[1] - 1].reputation += (int)(reputation * 0.7);
                                }
                            }
                        }
                        else
                        {
                            print("SettlePlayer: event type error!!!!!, type: " + type);
                        }
                    }
                    else   // 无key事件
                    {
                        money = int.Parse(xl1.ChildNodes[GetWithKeyMoneyIndex()].InnerText);
                        reputation = int.Parse(xl1.ChildNodes[GetWithKeyReputationIndex()].InnerText);
                        teamWork = int.Parse(xl1.ChildNodes[GetWithKeyTeamworkIndex()].InnerText);

                        if (1 == selectRoles.Count)
                        {
                            settleResult[selectRoles[0] - 1].money += money;
                            settleResult[selectRoles[0] - 1].reputation += reputation;
                        }
                        else if (2 == selectRoles.Count)
                        {
                            if (money > 0)
                            {
                                settleResult[selectRoles[0] - 1].money += (int)(money * 0.4);
                                settleResult[selectRoles[1] - 1].money += (int)(money * 0.4);
                            }
                            else
                            {
                                settleResult[selectRoles[0] - 1].money += (int)(money * 0.7);
                                settleResult[selectRoles[1] - 1].money += (int)(money * 0.7);
                            }

                            if (reputation > 0)
                            {
                                settleResult[selectRoles[0] - 1].reputation += (int)(reputation * 0.4);
                                settleResult[selectRoles[1] - 1].reputation += (int)(reputation * 0.4);
                            }
                            else
                            {
                                settleResult[selectRoles[0] - 1].reputation += (int)(reputation * 0.7);
                                settleResult[selectRoles[1] - 1].reputation += (int)(reputation * 0.7);
                            }
                        }
                        
                    }

                    // 结算teamwork
                    if (2 == selectRoles.Count)
                    {
                        teamworkValue += (teamWork + Random.Range(2, 5));
                    }
                    else
                    {
                        teamworkValue += teamWork;
                    }
                                        
                    break;
                }

            }
        }

        return settleResult;
    }

    public int Login(ref PlayerData playerInfo)
    {
        string path = Application.dataPath + playerDBPath;
        string name = playerInfo.name;
        string password = playerInfo.password;

        if (File.Exists(path))
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(path);
            XmlNodeList xmlNodeList = xml.SelectSingleNode("Players").ChildNodes;
            foreach (XmlElement xl1 in xmlNodeList)
            {
                if (0 == name.CompareTo(xl1.ChildNodes[0].InnerText) &&
                    0 == password.CompareTo(xl1.ChildNodes[1].InnerText))
                {
                    playerInfo.level = int.Parse(xl1.ChildNodes[2].InnerText);
                    playerInfo.section = int.Parse(xl1.ChildNodes[3].InnerText);
                    teamworkValue = int.Parse(xl1.ChildNodes[4].InnerText);

                    string moneyStr = xl1.ChildNodes[5].InnerText;
                    string reputationStr = xl1.ChildNodes[6].InnerText;

                    int i = 0;
                    if (moneyStr.CompareTo("") != 0)
                    {
                        string[] sMoney = moneyStr.Split(',');
                        foreach (string money in sMoney)
                        {
                            playerInfo.playerAttr[i].money = int.Parse(money);
                            settleResult[i].money = int.Parse(money);
                            i++;
                        }
                    }
                    if (reputationStr.CompareTo("") != 0)
                    {
                        i = 0;
                        string[] sReputation = reputationStr.Split(',');
                        foreach (string reputation in sReputation)
                        {
                            playerInfo.playerAttr[i].reputation = int.Parse(reputation);
                            settleResult[i].reputation = int.Parse(reputation);
                            i++;
                        }
                    }

                    return 0;
                }
            }
        }

        print("用户名或密码错误！！！");
        return 1;
    }

    public int Logout(PlayerData playerInfo)
    {
        name = playerInfo.name;
        string path = Application.dataPath + playerDBPath;
        if (File.Exists(path))
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(path);
            XmlNodeList xmlNodeList = xml.SelectSingleNode("Players").ChildNodes;
            foreach (XmlElement xl1 in xmlNodeList)
            {
                if (0 == name.CompareTo(xl1.ChildNodes[0].InnerText))
                {
                    xl1.ChildNodes[1].InnerText = playerInfo.password;
                    xl1.ChildNodes[2].InnerText = playerInfo.level.ToString();
                    xl1.ChildNodes[3].InnerText = playerInfo.section.ToString();
                    xl1.ChildNodes[4].InnerText = teamworkValue.ToString();

                    string money = "";
                    string reputation = "";
                    for (int i = 0; i < 4; i++)
                    {
                        if (i > 0)
                        {
                            money += ",";
                            reputation += ",";
                        }
                        money += playerInfo.playerAttr[i].money.ToString();
                        reputation += playerInfo.playerAttr[i].reputation.ToString();
                    }
                    xl1.ChildNodes[5].InnerText = money;
                    xl1.ChildNodes[6].InnerText = reputation;

                    break;
                }
            }
            xml.Save(path);
        }

        return 0;
    }

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            settleResult[i].money = 50;
            settleResult[i].reputation = 50;
        }
/*
        if (IsJumpStory(51))
            print("跳跳跳！！！");

        int preEvent = 0;
        int roleID = 0;

        if (IsPreStroy(9, ref preEvent, ref roleID))
        {
            print("preEvent: " + preEvent + "roleID: " + roleID);
        }
*/
    }

    // Update is called once per frame
    void Update()
    {

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
