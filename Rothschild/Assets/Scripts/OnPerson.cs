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

    private LevelManager levelManager;

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
        //colorState = this.GetComponent<ColorState>();
        colorState = GameObject.Find("ColorState").GetComponent<ColorState>();

        targetGraphic.color = colorState.normalColor;

        levelManager = GameObject.Find("LogicHandler").GetComponent<LevelManager>();
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
        targetGraphic.color = colorState.enterColor;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (isSelected)
        {
            targetGraphic.color = colorState.selectColor;
        }
        else
        {
            targetGraphic.color = colorState.normalColor;
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        targetGraphic.color = colorState.clickColor;
        //isSelected = !isSelected;
        //判断是否能够选择
        if (isSelected == false)
        {
            if (levelManager.AddSelect(this.tag))
            {
                isSelected = true;
            }
        }
        else
        {
            levelManager.RemoveSelect();
            isSelected = false;
        }

    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (isSelected)
        {
            targetGraphic.color = colorState.selectColor;
        }
        else
        {
            targetGraphic.color = colorState.normalColor;
        }
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    public void Clear()
    {
        isSelected = false;
        targetGraphic.color = colorState.normalColor;
    }

    public void SetAvator(string avatarName)
    {
        avatarImage.sprite = Resources.Load(avatarName, typeof(Sprite)) as Sprite;
    }

    /*接收值为财富值的改变，返回角色是否死亡，返回true则为死亡*/
    public bool SetWealth(int wealth)
    {
        //wealthNum += wealthChange;

        //isDead = (wealthNum <= 0);
        //wealthNum = wealthNum < 0 ? 0 : wealthNum;
        wealthNum = wealth;
        isDead = (wealthNum <= 0 || wealthNum >= 100);
        wealthNum = Mathf.Clamp(wealthNum, 0, 100);
        wealthText.text = "财富\n" + wealthNum.ToString() + "/100";

        return isDead;
    }

    /*接收值为声望的改变，返回角色是否死亡，返回true则为死亡*/
    public bool SetReputation(int reputation)
    {
        //reputationNum += reputationChange;

        //isDead = (reputationNum <= 0);
        //reputationNum = reputationNum < 0 ? 0 : wealthNum;
        reputationNum = reputation;
        isDead = (reputationNum <= 0 || reputationNum >= 100);
        wealthNum = Mathf.Clamp(reputation, 0, 100);
        reputationText.text = "声望\n" + reputationNum.ToString() + "/100";

        return isDead;
    }

    private void SetDead()
    {
        targetGraphic.color = colorState.deadColor;
        this.enabled = false;
    }
}
