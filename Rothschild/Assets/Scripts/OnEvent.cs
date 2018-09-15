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

    // Use this for initialization
    void Start()
    {
        eventImage = this.transform.GetChild(0).gameObject.GetComponent<Image>();
        eventText = this.transform.GetChild(1).gameObject.GetComponent<Text>();
        eventTitle = GameObject.Find("EventTitle").GetComponent<Text>();
        levelManager = GameObject.Find("LogicHandler").GetComponent<LevelManager>();
        choiceOne = this.transform.GetChild(2).gameObject.GetComponent<Button>();
        choiceTwo = this.transform.GetChild(3).gameObject.GetComponent<Button>();
        choiceThree = this.transform.GetChild(4).gameObject.GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {

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
    }

    public void SetEventTitle(string title)
    {

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
    }

    public void SetRoleCountLimit(int countLimit)
    {

    }

    public void SetChoiceType(int typeOne, int typeTwo)
    {
        switch(choiceCount)
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
