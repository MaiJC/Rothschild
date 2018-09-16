using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnLogInterface : EventTrigger {

    private GameObject logInterface;
    private GameObject logScripts;

	// Use this for initialization
	void Start () {

        logInterface = GameObject.Find("log_canvas");
        logScripts = GameObject.Find("log_scripts");
        //logInterface.

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void OnPointerClick(PointerEventData eventData)
    {
        logScripts.GetComponent<log_interface>().enter_log_interface();
        GameObject.Find("Canvas").SetActive(false);        
    }
}
