using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterfaceManager : MonoBehaviour {

    public Canvas threeEvent;
    public Canvas twoEvent;

    private ArrayList currentEventList = new ArrayList();

    private OnCardClicked cardOne;
    private OnCardClicked cardFour;

    // Use this for initialization
    void Start () {
        threeEvent.enabled = true;
        twoEvent.enabled = false;

        cardOne = GameObject.Find("Event1").GetComponent<OnCardClicked>();
        cardFour = GameObject.Find("Event4").GetComponent<OnCardClicked>();
    }
	
	// Update is called once per frame
	void Update () {
        //if (!Input.GetMouseButtonUp(0))
        //    return;

        //if (cardOne.IsSelected())
        //{
        //    threeEvent.enabled = false;
        //    cardOne.Clear();
        //    twoEvent.enabled = true;
        //}
        //else if (cardFour.IsSelected())
        //{
        //    threeEvent.enabled = true;
        //    twoEvent.enabled = false;
        //    cardFour.Clear();
        //}
    }
}
