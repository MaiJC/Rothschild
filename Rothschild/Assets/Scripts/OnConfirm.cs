using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OnConfirm : EventTrigger
{
    private List<OnPerson> onPerson = new List<OnPerson>();
    private LevelManager levelManager;
    private PlayerDataProc playerDataProc;
    private int choiceID;

    // Use this for initialization
    void Start()
    {
        onPerson.Add(GameObject.Find("PersonPanelA").GetComponent<OnPerson>());
        onPerson.Add(GameObject.Find("PersonPanelB").GetComponent<OnPerson>());
        onPerson.Add(GameObject.Find("PersonPanelC").GetComponent<OnPerson>());
        onPerson.Add(GameObject.Find("PersonPanelD").GetComponent<OnPerson>());
        playerDataProc = GameObject.Find("LogicHandler").GetComponent<PlayerDataProc>();
        levelManager = GameObject.Find("LogicHandler").GetComponent<LevelManager>();

        choiceID = this.tag == "ChoiceOne" ? 1 : 2;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        PrcData();
        levelManager.Confirm();
    }

    void PrcData()
    {
        int eventID = this.transform.parent.gameObject.GetComponent<OnEvent>().GetEventID();
        List<int> selectPerson = new List<int>();

        for (int i = 0; i < onPerson.Count; i++)
        {
            if (onPerson[i].IsSelected())
                selectPerson.Add(i + 1);
        }

        if (selectPerson.Count == 0)
            return;

        PlayerAttr[] playerAttrs = playerDataProc.SettlePlayer(eventID, choiceID, selectPerson);

        for (int i = 0; i < 4; i++)
        {
            onPerson[i].SetReputation(playerAttrs[i].reputation);
            onPerson[i].SetWealth(playerAttrs[i].money);
        }
    }
}
