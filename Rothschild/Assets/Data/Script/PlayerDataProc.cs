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
    public int money;
    public int reputation;  
}

public class PlayerDataProc : MonoBehaviour {
    string playerDBPath = "/Data/Xml/palyerDB.xml";

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

    void addPlayerData(PlayerData playerInfo)
    {
        string path = Application.dataPath + playerDBPath;
        if (File.Exists(path))
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(path);
            XmlNode root = xml.SelectSingleNode("Players");
            
            XmlElement element = xml.CreateElement("Player");
           
            XmlElement elementName = xml.CreateElement("Name");
            elementName.InnerText = playerInfo.name;

            XmlElement elementPassword = xml.CreateElement("Password");
            elementPassword.InnerText = playerInfo.password;

            XmlElement elementLevel = xml.CreateElement("Level");
            elementLevel.InnerText = playerInfo.level.ToString();

            XmlElement elementMoney = xml.CreateElement("Money");
            elementMoney.InnerText = playerInfo.money.ToString();

            XmlElement elementReputation = xml.CreateElement("Reputation");
            elementReputation.InnerText = playerInfo.reputation.ToString();

            // 新增玩家的初始已选择的事件集合为空
            XmlElement elementEventLog = xml.CreateElement("EventLog");
            elementEventLog.InnerText = "";

            //把节点一层一层的添加至xml中，注意他们之间的先后顺序，这是生成XML文件的顺序
            element.AppendChild(elementName);
            element.AppendChild(elementPassword);
            element.AppendChild(elementLevel);
            element.AppendChild(elementMoney);
            element.AppendChild(elementReputation);
            element.AppendChild(elementEventLog);

            root.AppendChild(element);

            xml.AppendChild(root);
            //最后保存文件
            xml.Save(path);
        }
    }

    // 更新玩家数据
    void updatePlayerData(PlayerData playerInfo)
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
                    xl1.ChildNodes[3].InnerText = playerInfo.money.ToString();
                    xl1.ChildNodes[4].InnerText = playerInfo.reputation.ToString();

                    break;
                }
            }
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
            PlayerData playerInfo;

            playerInfo.name = name;
            playerInfo.password = password;
            playerInfo.level = 1;
            playerInfo.money = 0;
            playerInfo.reputation = 0;

            addPlayerData(playerInfo);
        }
        return 0;
    }

    List<int> GetEventLog(string name)
    {
        string path = Application.dataPath + playerDBPath;

        List<int> eventList = new List<int>();
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
                    eventStr = xl1.ChildNodes[5].InnerText;
                    break;
                }
            }
        }
        print(eventStr);
        string[] sEvent = eventStr.Split(',');
        foreach (string eventID in sEvent)
        {
            print(eventID);
            eventList.Add(int.Parse(eventID));
        }

        return eventList;
    }

    int AddEventLog(string name, int eventID)
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
                    string eventStr = xl1.ChildNodes[5].InnerText;
                    if (eventStr.CompareTo("") != 0)
                    {
                        eventStr += ',';
                    }
                    eventStr += eventID;
                    
                    xl1.ChildNodes[5].InnerText = eventStr;
                    xml.Save(path);
                    return 0;
                }
            }
        }
        return 1;
    }

    int AddEvenLogList(string name, List<int> eventLogList)
    {
        foreach (int eventID in eventLogList)
        {
            if (AddEventLog(name, eventID) != 0)
            {
                return 1;
            }
        }
        return 0;
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
                    playerInfo.money = int.Parse(xl1.ChildNodes[3].InnerText);
                    playerInfo.reputation = int.Parse(xl1.ChildNodes[4].InnerText);
             
                    return 0;
                }
            }
        }

        print("用户名或密码错误！！！");
        return 1;
    }

    int Logout(PlayerData playerInfo)
    {
        updatePlayerData(playerInfo);
        return 0;
    }

    // Use this for initialization
    void Start () {
        
        //CreatePlayerDB();
        regPlayerData("neilxkchen", "43216");
        PlayerData myInfo;
        myInfo.name = "neilxkchen";
        myInfo.password = "43216";
        myInfo.level = 0;
        myInfo.money = 0;
        myInfo.reputation = 0;

        Login(ref myInfo);

        myInfo.level = 8888;

        Logout(myInfo);

        //      print("My Level: " + myInfo.level);

        //     AddEventLog("neilxkchen", 666);
        //    AddEventLog("neilxkchen", 777);
        //    AddEventLog("neilxkchen", 888);
        List<int> eventLogList = new List<int>();
        eventLogList.Add(101);
        eventLogList.Add(102);
        eventLogList.Add(103);
        AddEvenLogList("neilxkchen", eventLogList);
        List<int> eventLog = GetEventLog("neilxkchen");
        
    //    foreach (int eventID in eventLog)
    //       print(eventID);

    }

    // Update is called once per frame
    void Update () {
		
	}
}
