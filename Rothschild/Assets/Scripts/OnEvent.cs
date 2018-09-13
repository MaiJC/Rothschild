using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnEvent : MonoBehaviour {

    private Image eventImage;
    private Text eventText;
    private int eventID;
    private LevelManager levelManager;
    private Button choiceOne;
    private Button choiceTwo;
    private Button choiceThree;
 
	// Use this for initialization
	void Start () {
        eventImage = this.transform.GetChild(0).gameObject.GetComponent<Image>();
        eventText = this.transform.GetChild(1).gameObject.GetComponent<Text>();
        levelManager = GameObject.Find("LogicHandler").GetComponent<LevelManager>();
        choiceOne = this.transform.GetChild(2).GetChild(0).gameObject.GetComponent<Button>();
        choiceTwo = this.transform.GetChild(3).GetChild(0).gameObject.GetComponent<Button>();
        choiceThree = this.transform.GetChild(4).GetChild(0).gameObject.GetComponent<Button>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetImage(string path)
    {
        eventImage.sprite = Resources.Load(path, typeof(Sprite)) as Sprite;
    }

    public void SetText(string text)
    {
        eventText.text = text;
    }

    private void SetEventID(int id)
    {
        eventID = id;
    }

    public int GetEventID()
    {
        return eventID; 
    }

    public void SetEventText(List<string> choiceTest)
    {
        eventText.text = choiceTest[0];
        choiceTest.RemoveAt(0);

        if(choiceTest.Count==0)
        {
            choiceOne.enabled = false;
            choiceTwo.enabled = false;
            choiceThree.enabled = false;
        }
        else if(choiceTest.Count==1)
        {
            choiceThree.enabled = true;
            choiceThree.GetComponent<Text>().text = choiceTest[0];
            choiceOne.enabled = false;
            choiceTwo.enabled = false;
        }
        else if(choiceTest.Count==2)
        {
            choiceOne.enabled = true;
            choiceTwo.enabled = true;
            choiceOne.GetComponent<Text>().text = choiceTest[0];
            choiceTwo.GetComponent<Text>().text = choiceTest[1];
            choiceThree.enabled = false;
        }
    }

}
