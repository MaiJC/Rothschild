using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceManager : MonoBehaviour {

    private Image imageOfConvesOne;
    private List<string> cardPath = new List<string>();
    private int currentIndex;

    // Use this for initialization
    void Start () {

        cardPath.Add("banana");
        cardPath.Add("monkey");
        cardPath.Add("banana_alfa");
        currentIndex = Random.Range(0, 3);

        imageOfConvesOne = GameObject.Find("imageForOneEvent").GetComponent<Image>();

        imageOfConvesOne.sprite = Resources.Load(cardPath[currentIndex], typeof(Sprite)) as Sprite;
    }
	
	// Update is called once per frame
	void Update () {

    }

    public void OnConfirm()
    {
        int i = Random.Range(0, 3);
        while(i==currentIndex)
        {
            i = Random.Range(0, 3);
        }
        currentIndex = i;
        imageOfConvesOne.sprite = Resources.Load(cardPath[i], typeof(Sprite)) as Sprite;
        Debug.Log(currentIndex);
    }

}
