using Mono.Xml;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Xml;
using UnityEngine;


struct PlayerData  //玩家信息
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

struct EventLog
{
    public int roleID;
    public int eventID;
    public int choice;
}

public class PlayerDataProc : MonoBehaviour
{
    // 1是唯key，2是泛key，3是通用
    public const int WEI_KEY_TYPE = 1;
    public const int FAN_KEY_TYPE = 2;
    public const int GENERAL_TYPE = 3;

    string playerDBPath = "/Data/Xml/palyerDB.xml";
    string eventTablePath = "/Data/Xml/event.xml";

    PlayerAttr[] settleResult = new PlayerAttr[4];

    void CreatePlayerDB()
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

    bool FindPlayer(string name)
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


    int regPlayerData(string name, string password)
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

    List<EventLog> GetEventLog(string name)
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
                    eventStr = xl1.ChildNodes[6].InnerText;
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

    int AddEventLog(string name, EventLog eventlog)
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
                    string eventStr = xl1.ChildNodes[6].InnerText;
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

                    xl1.ChildNodes[6].InnerText = eventStr;
                    xml.Save(path);
                    return 0;
                }
            }
        }
        return 1;
    }

    int AddEvenLogList(string name, List<EventLog> eventLogList)
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

    bool KeyMatch(int key1, int key2, int type, List<int> selectRoles)
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

    public PlayerAttr[] SettlePlayer(int eventID, int eventChoice, List<int> selectRoles) 
    {
        string path = Application.dataPath + eventTablePath;

        int roleNum = selectRoles.Count;
        bool keyFlag = false;

        int money = 0;
        int reputation = 0;
        int teamWork = 0;
        int settleValue = 0;

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

                        }
                    }
                    else   // 无key事件
                    {

                    }
                                        
                    break;
                }

            }
        }

        return settleResult;
    }

    int Login(ref PlayerData playerInfo)
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

                    string moneyStr = xl1.ChildNodes[4].InnerText;
                    string reputationStr = xl1.ChildNodes[5].InnerText;

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

    int Logout(PlayerData playerInfo)
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
                    xl1.ChildNodes[4].InnerText = money;
                    xl1.ChildNodes[5].InnerText = reputation;

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

        /*    PlayerAttr[] result = SettlePlayer();

            print(result[0].money);
            print(result[0].reputation);
            print(result[1].money);
            print(result[1].reputation);*/

        //CreatePlayerDB();
        //       regPlayerData("neilxkchen", "43216");
        /*        PlayerData myInfo;
                myInfo.name = "neilxkchen";
                myInfo.password = "43216";
                myInfo.level = 3;
                myInfo.section = 3;

                myInfo.playerAttr = new PlayerAttr[4];

                for (int i = 0; i < 4; i++)
                {
                    myInfo.playerAttr[i].money = i + 1;
                    myInfo.playerAttr[i].reputation = i + 1;
                }

                Login(ref myInfo);

                print("level: " + myInfo.level);
                print("section: " + myInfo.section);
                for (int i = 0; i < 4; i++)
                {
                    print("money: " + myInfo.playerAttr[i].money);
                    print("reputation: " + myInfo.playerAttr[i].reputation);
                }

                //        myInfo.level = 8888;

                //       Logout(myInfo);

                //      print("My Level: " + myInfo.level);

                //     AddEventLog("neilxkchen", 666);
                //    AddEventLog("neilxkchen", 777);
                //    AddEventLog("neilxkchen", 888);
                /*        List<int> eventLogList = new List<int>();
                        eventLogList.Add(101);
                        eventLogList.Add(102);
                        eventLogList.Add(103);
                        AddEvenLogList("neilxkchen", eventLogList);
                        List<int> eventLog = GetEventLog("neilxkchen");*/

        //    foreach (int eventID in eventLog)
        //       print(eventID);*/
        /*    EventLog log1 = new EventLog();
            log1.roleID = 1;
            log1.eventID = 1001;
            log1.choice = 1;

            EventLog log2 = new EventLog();
            log2.roleID = 2;
            log2.eventID = 1002;
            log2.choice = 2;

            EventLog log3 = new EventLog();
            log3.roleID = 3;
            log3.eventID = 1003;
            log3.choice = 3;

            List<EventLog> eventLogList = new List<EventLog>();

            eventLogList.Add(log1);
            eventLogList.Add(log2);
            eventLogList.Add(log3);


            AddEvenLogList("neilxkchen", eventLogList);
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

    int GetWithKeyMoneyIndex()
    {
        return 8;
    }

    int GetWithKeyReputationIndex()
    {
        return 9;
    }

    int GetWithKeyTeamworkIndex()
    {
        return 10;
    }

    int GetWithKeyCd()
    {
        return 11;
    }

    int GetWithoutKeyMoneyIndex()
    {
        return 12;
    }

    int GetWithoutKeyReputation()
    {
        return 13;
    }

    int GetWithoutKeyTeamwork()
    {
        return 14;
    }

    int GetWithoutKeyCd()
    {
        return 15;
    }
}
