using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OnConfirm : EventTrigger {

    private List<OnPerson> onPerson;
    private LevelManager levelManager;
    private PlayerDataProc playerDataProc;

	// Use this for initialization
	void Start () {
        onPerson.Add(GameObject.Find("PersonPanelA").GetComponent<OnPerson>());
        onPerson.Add(GameObject.Find("PersonPanelB").GetComponent<OnPerson>());
        onPerson.Add(GameObject.Find("PersonPanelC").GetComponent<OnPerson>());
        onPerson.Add(GameObject.Find("PersonPanelD").GetComponent<OnPerson>());
        playerDataProc = GameObject.Find("LogicHandler").GetComponent<PlayerDataProc>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void OnPointerUp(PointerEventData eventData)
    {
        int eventID = this.transform.parent.gameObject.GetComponent<OnEvent>().GetEventID();
    }
}
