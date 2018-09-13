using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnEvent : MonoBehaviour {

    private Image eventImage;
    private Text eventText;
    private int eventID;
    private LevelManager levelManager;

	// Use this for initialization
	void Start () {
        eventImage = this.transform.GetChild(0).gameObject.GetComponent<Image>();
        eventText = this.transform.GetChild(1).gameObject.GetComponent<Text>();
        levelManager = GameObject.Find("LogicHandler").GetComponent<LevelManager>();
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

    private void SetID(int id)
    {
        eventID = id;
    }

}
