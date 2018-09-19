using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnEvent : MonoBehaviour
{

    private Image eventImage;
    private Text eventText;
    private Text eventTitle;
    private int eventID;
    private LevelManager levelManager;
    private Button choiceOne;
    private Button choiceTwo;
    private Button choiceThree;
    private int choiceCount = 2;

    private OnConfirm save;

    private bool hasInitalize = false;
    double loadTime;

    // Use this for initialization
    void Start()
    {
        //eventImage = this.transform.GetChild(0).gameObject.GetComponent<Image>();
        //eventText = this.transform.GetChild(1).gameObject.GetComponent<Text>();
        //eventTitle = GameObject.Find("EventTitle").GetComponent<Text>();
        //levelManager = GameObject.Find("LogicHandler").GetComponent<LevelManager>();
        //choiceOne = this.transform.GetChild(2).gameObject.GetComponent<Button>();
        //choiceTwo = this.transform.GetChild(3).gameObject.GetComponent<Button>();
        //choiceThree = this.transform.GetChild(4).gameObject.GetComponent<Button>();
        loadTime = Time.fixedTime;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (hasInitalize == false && Time.fixedTime - loadTime > 2)
        {
            eventImage = this.transform.GetChild(0).gameObject.GetComponent<Image>();
            eventText = this.transform.GetChild(1).gameObject.GetComponent<Text>();
            eventTitle = GameObject.Find("EventTitle").GetComponent<Text>();
            levelManager = GameObject.Find("LogicHandler").GetComponent<LevelManager>();
            choiceOne = this.transform.GetChild(2).gameObject.GetComponent<Button>();
            choiceTwo = this.transform.GetChild(3).gameObject.GetComponent<Button>();
            choiceThree = this.transform.GetChild(4).gameObject.GetComponent<Button>();

            //GameObject.Find("Save").gameObject.GetComponent<OnConfirm>().SetChoiceType(1);
            GameObject.Find("NotSave").gameObject.GetComponent<OnConfirm>().SetChoiceType(2);
            save = GameObject.Find("Save").gameObject.GetComponent<OnConfirm>();
            save.SetChoiceType(1);

            hasInitalize = true;
        }
    }

    public void SetImage(string path)
    {
        eventImage.sprite = Resources.Load(path, typeof(Sprite)) as Sprite;
    }

    public void SetText(string text)
    {
        eventText.text = text;
    }

    public void SetEventID(int id)
    {
        eventID = id;
    }

    public int GetEventID()
    {
        return eventID;
    }

    public void SetEventText(List<string> choiceTest)
    {
        //choiceThree.transform.GetChild(0).GetComponent<Text>().text = "fuck you bitch";

        eventText.text = choiceTest[0];
        choiceTest.RemoveAt(0);

        Debug.Log("choice count" + choiceTest.Count.ToString());

        choiceCount = choiceTest.Count;
        if (choiceTest.Count == 0)
        {
            choiceOne.gameObject.SetActive(false);
            choiceTwo.gameObject.SetActive(false);
            choiceThree.gameObject.SetActive(false);
        }
        else if (choiceTest.Count == 1)
        {
            choiceOne.gameObject.SetActive(false);
            choiceTwo.gameObject.SetActive(false);
            choiceThree.gameObject.SetActive(true);

            choiceThree.transform.GetChild(0).gameObject.GetComponent<Text>().text = choiceTest[0];
        }
        else if (choiceTest.Count == 2)
        {
            choiceOne.gameObject.SetActive(true);
            choiceTwo.gameObject.SetActive(true);
            choiceThree.gameObject.SetActive(false);

            choiceOne.transform.GetChild(0).gameObject.GetComponent<Text>().text = choiceTest[0];
            choiceTwo.transform.GetChild(0).gameObject.GetComponent<Text>().text = choiceTest[1];
        }
    }

    public void SetUnselectable()
    {
        if (levelManager.currentMaxSelectedPersonCount == 0)
        {
            SetSelectable();
            return;
        }

        switch (choiceCount)
        {
            case 1:
                choiceThree.GetComponent<OnConfirm>().SetUnselectable();
                break;
            case 2:
                choiceOne.GetComponent<OnConfirm>().SetUnselectable();
                choiceTwo.GetComponent<OnConfirm>().SetUnselectable();
                break;
        }
        save.SetUnselectable();
    }

    public void SetEventTitle(string title)
    {
        eventTitle.text = title;
    }

    public void SetSelectable()
    {
        switch (choiceCount)
        {
            case 1:
                choiceThree.GetComponent<OnConfirm>().SetSelectable();
                break;
            case 2:
                choiceOne.GetComponent<OnConfirm>().SetSelectable();
                choiceTwo.GetComponent<OnConfirm>().SetSelectable();
                break;
        }
        save.SetSelectable();

    }

    public void SetChoiceType(int typeOne, int typeTwo)
    {
        Debug.Log("SetChoiceType: choiceCount:" + choiceCount.ToString()
            + ", type1:" + typeOne.ToString() + ", type2:" + typeTwo.ToString());
        switch (choiceCount)
        {
            case 1:
                choiceThree.GetComponent<OnConfirm>().SetChoiceType(typeOne);
                break;
            case 2:
                choiceOne.GetComponent<OnConfirm>().SetChoiceType(typeOne);
                choiceTwo.GetComponent<OnConfirm>().SetChoiceType(typeTwo);
                break;
        }
    }

    public void SetJumpStoryRole(int role)
    {

    }

    public void SetPersonUnselectable(int unRole)
    {

    }

    private void KeepPersonSelected(int role)
    {

    }

}
