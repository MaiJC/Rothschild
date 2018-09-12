using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceManager : MonoBehaviour
{
    LoadRes loadRes;
    private List<int> commonEventID = new List<int>();
    private int levelCount;
    private List<List<int>> levelEventID = new List<List<int>>();
    private List<List<int>> levelStoryID = new List<List<int>>();
    private int currentEventID;
    private int currentLevel = 1;
    private Dictionary<int, string> eventUIPath = new Dictionary<int, string>();
    private Dictionary<int, string> eventText = new Dictionary<int, string>();

    /*this is just use for monkeys*/
    private List<string> cardPath = new List<string>();
    private List<Image> personImage = new List<Image>();
    private List<OnPerson> person = new List<OnPerson>();

    // Use this for initialization
    void Start()
    {
        Initialize();
        InitializeMonkey();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnConfirm()
    {

    }

    void Initialize()
    {
        loadRes = GameObject.Find("DataHandler").GetComponent<LoadRes>();

        levelCount = loadRes.GetLevelCount();
        commonEventID = loadRes.GetCommonEventID();
        for (int i = 1 ; i <= levelCount; i++)
        {
            levelEventID.Add(loadRes.GetLevelEventID(i));
            levelStoryID.Add(loadRes.GetLevelStoryID(i));
        }
        for (int i = 0; i < commonEventID.Count; i++)
        {
            int eventID = commonEventID[i];
            string path = loadRes.GetEventUIPath(eventID);
            string text = loadRes.GetEventText(eventID);
            eventUIPath.Add(eventID, path);
            eventText.Add(eventID, text);
        }
        for (int i = 0; i < levelCount; i++)
        {
            for (int j = 0; j < levelEventID[i].Count; j++)
            {
                int eventID = levelEventID[i][j];
                string path = loadRes.GetEventUIPath(eventID);
                string text = loadRes.GetEventText(eventID);
                eventUIPath.Add(eventID, path);
                eventText.Add(eventID, text);
            }
        }
    }

    void InitializeMonkey()
    {
        cardPath.Add("banana");
        cardPath.Add("monkey");
        cardPath.Add("banana_alfa");

        personImage.Add(GameObject.Find("PersonImageA").GetComponent<Image>());
        personImage.Add(GameObject.Find("PersonImageB").GetComponent<Image>());
        personImage.Add(GameObject.Find("PersonImageC").GetComponent<Image>());
        personImage.Add(GameObject.Find("PersonImageD").GetComponent<Image>());

        person.Add(GameObject.Find("PersonPanelA").GetComponent<OnPerson>());
        person.Add(GameObject.Find("PersonPanelB").GetComponent<OnPerson>());
        person.Add(GameObject.Find("PersonPanelC").GetComponent<OnPerson>());
        person.Add(GameObject.Find("PersonPanelD").GetComponent<OnPerson>());

        //foreach (Image i in personImage)
        //{
        //    i.overrideSprite = Resources.Load("monkey", typeof(Sprite)) as Sprite;
        //}
    }

    void GenerateCurrentEvent()
    {

    }

}
