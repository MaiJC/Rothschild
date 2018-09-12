using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnPerson : EventTrigger
{
    private Graphic targetGraphic;
    private bool isSelected = false;

    private ColorState colorState;

    private InterfaceManager interfaceManager;

    //头像
    private Image avatarImage;
    //财富
    private Text wealthText;
    private int wealthNum;
    //名望
    private Text reputationText;
    private int reputationNum;
    //是否死亡
    private bool isDead = false;

    
    // Use this for initialization
    void Start()
    {
        //this.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;

        targetGraphic = this.GetComponent<Button>().targetGraphic;
        colorState = this.GetComponent<ColorState>();

        targetGraphic.color = colorState.normalColor;

        interfaceManager = GameObject.Find("LogicHandler").GetComponent<InterfaceManager>();
        avatarImage = this.transform.GetChild(0).gameObject.GetComponent<Image>();
        wealthText = this.transform.GetChild(1).gameObject.GetComponent<Text>();
        reputationText = this.transform.GetChild(2).gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        //base.OnPointerEnter(eventData);
        targetGraphic.color = colorState.enterColor;
        //image.color = enterColor;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        //base.OnPointerExit(eventData);
        if (isSelected)
        {
            targetGraphic.color = colorState.selectColor;
            //image.color = selectColor;
        }
        else
        {
            targetGraphic.color = colorState.normalColor;
            //image.color = normalColor;
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        targetGraphic.color = colorState.clickColor;
        //image.color = clickColor;
        isSelected = !isSelected;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        //targetGraphic.color = enterColor;
        if (isSelected)
        {
            targetGraphic.color = colorState.selectColor;
            //image.color = selectColor;
        }
        else
        {
            targetGraphic.color = colorState.normalColor;
            //image.color = normalColor;
        }
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    public void Clear()
    {
        isSelected = false;
    }

    public void SetAvator(string avatarName)
    {
        avatarImage.sprite = Resources.Load(avatarName, typeof(Sprite)) as Sprite;
    }

    /*receive the number of wealth change, and return if this person is dead*/
    public bool SetWealthChange(int wealthChange)
    {
        wealthNum += wealthChange;

        isDead = (wealthNum <= 0);
        wealthNum = wealthNum < 0 ? 0 : wealthNum;
        wealthText.text = "财富\n" + wealthNum.ToString() + "/100";

        return isDead;
    }

    public bool SetReputationChange(int reputationChange)
    {
        reputationNum += reputationChange;

        isDead = (reputationNum <= 0);
        reputationNum = reputationNum < 0 ? 0 : wealthNum;
        reputationText.text = "声望\n" + reputationNum.ToString() + "/100";

        return isDead;
    }
}
