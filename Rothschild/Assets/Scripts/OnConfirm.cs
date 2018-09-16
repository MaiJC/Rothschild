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
    private Graphic targetGraphic;
    private ColorState colorState;
    private int choiceType;
    private Image teamWorkBar;
    private OnEvent onEvent;
    private bool hasInitalize = false;

    // Use this for initialization
    void Start()
    {
        //onPerson.Add(GameObject.Find("PersonPanelA").GetComponent<OnPerson>());
        //onPerson.Add(GameObject.Find("PersonPanelB").GetComponent<OnPerson>());
        //onPerson.Add(GameObject.Find("PersonPanelC").GetComponent<OnPerson>());
        //onPerson.Add(GameObject.Find("PersonPanelD").GetComponent<OnPerson>());
        //playerDataProc = GameObject.Find("LogicHandler").GetComponent<PlayerDataProc>();
        //levelManager = GameObject.Find("LogicHandler").GetComponent<LevelManager>();
        //colorState = GameObject.Find("ColorState").GetComponent<ColorState>();
        //targetGraphic = this.GetComponent<Button>().targetGraphic;
        //teamWorkBar = GameObject.Find("Fill").GetComponent<Image>();
        //float teamworkPercent = (float)playerDataProc.GetTeamworkValue() / 200.0f;
        //teamWorkBar.transform.localScale = new Vector3(teamworkPercent, 1.0f, 1.0f);
        //onEvent = GameObject.Find("EventSlot").GetComponent<OnEvent>();

        //choiceID = this.tag == "ChoiceOne" ? 1 : 2;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (hasInitalize == false && Time.fixedTime > 2)
        {
            onPerson.Add(GameObject.Find("PersonPanelA").GetComponent<OnPerson>());
            onPerson.Add(GameObject.Find("PersonPanelB").GetComponent<OnPerson>());
            onPerson.Add(GameObject.Find("PersonPanelC").GetComponent<OnPerson>());
            onPerson.Add(GameObject.Find("PersonPanelD").GetComponent<OnPerson>());
            playerDataProc = GameObject.Find("LogicHandler").GetComponent<PlayerDataProc>();
            levelManager = GameObject.Find("LogicHandler").GetComponent<LevelManager>();
            colorState = GameObject.Find("ColorState").GetComponent<ColorState>();
            targetGraphic = this.GetComponent<Button>().targetGraphic;
            teamWorkBar = GameObject.Find("Fill").GetComponent<Image>();
            float teamworkPercent = (float)playerDataProc.GetTeamworkValue() / 200.0f;
            teamWorkBar.transform.localScale = new Vector3(teamworkPercent, 1.0f, 1.0f);
            onEvent = GameObject.Find("EventSlot").GetComponent<OnEvent>();

            choiceID = this.tag == "ChoiceOne" ? 1 : 2;

            hasInitalize = true;
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        //若没有选择任何人
        int selectedCount = 0;
        List<bool> selectRole = new List<bool>();
        foreach (OnPerson person in onPerson)
        {
            selectRole.Add(person.IsSelected());
            if (person.IsSelected()) selectedCount++;
        }
        if (selectedCount == 0 && choiceType != 2)
        {
            return;
        }

        int choice = tag == "ChoiceOne" ? 1 : 2;

        PrcData();
        //获得下一关
        levelManager.Confirm(choice, selectRole);
        foreach (OnPerson personTmp in onPerson)
        {
            personTmp.Clear();
        }
        levelManager.HandleDead();
        RefreshTeamwork();
    }

    void PrcData()
    {
        int eventID = onEvent.GetEventID();
        List<int> selectPerson = new List<int>();

        for (int i = 0; i < onPerson.Count; i++)
        {
            if (onPerson[i].IsSelected())
                selectPerson.Add(i + 1);
        }

        PlayerAttr[] playerAttrs = playerDataProc.SettlePlayer(eventID, choiceID, selectPerson);

        for (int i = 0; i < 4; i++)
        {
            onPerson[i].SetReputation(playerAttrs[i].reputation);
            onPerson[i].SetWealth(playerAttrs[i].money);
        }

        RefreshTeamwork();
    }

    private void RefreshTeamwork()
    {
        int teamWork = playerDataProc.GetTeamworkValue();
        float teamWorkPercent = (float)teamWork / 200.0f;
        teamWorkBar.transform.localScale = new Vector3(teamWorkPercent, 1, 1);
    }

    public void SetUnselectable()
    {
        if (choiceType == 2)
        {
            this.targetGraphic.color = colorState.buttonNormalColor;
            this.enabled = true;
            return;
        }

        this.targetGraphic.color = colorState.deadColor;
        this.enabled = false;
    }

    public void SetSelectable()
    {
        this.targetGraphic.color = colorState.buttonNormalColor;
        this.enabled = true;
    }

    public void SetChoiceType(int ct)
    {
        choiceType = ct;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
    }
}
