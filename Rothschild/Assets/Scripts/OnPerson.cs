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

    private bool hasInitalize = false;


    // Use this for initialization
    void Start()
    {
        //targetGraphic = this.GetComponent<Button>().targetGraphic;
        ////colorState = this.GetComponent<ColorState>();
        //colorState = GameObject.Find("ColorState").GetComponent<ColorState>();

        //targetGraphic.color = colorState.personNormalColor;

        //levelManager = GameObject.Find("LogicHandler").GetComponent<LevelManager>();
        //avatarImage = this.transform.GetChild(0).gameObject.GetComponent<Image>();
        //wealthText = this.transform.GetChild(1).gameObject.GetComponent<Text>();
        //reputationText = this.transform.GetChild(2).gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (hasInitalize == false && Time.fixedTime > 2)
        {
            targetGraphic = this.GetComponent<Button>().targetGraphic;
            //colorState = this.GetComponent<ColorState>();
            colorState = GameObject.Find("ColorState").GetComponent<ColorState>();

            targetGraphic.color = colorState.personNormalColor;

            levelManager = GameObject.Find("LogicHandler").GetComponent<LevelManager>();
            avatarImage = this.transform.GetChild(0).gameObject.GetComponent<Image>();
            wealthText = this.transform.GetChild(1).gameObject.GetComponent<Text>();
            reputationText = this.transform.GetChild(2).gameObject.GetComponent<Text>();

            hasInitalize = true;
        }
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
            targetGraphic.color = colorState.personNormalColor;
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
            levelManager.RemoveSelect(this.tag);
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
            targetGraphic.color = colorState.personNormalColor;
        }
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    public void Clear()
    {
        if (isDead)
            return;
        isSelected = false;
        targetGraphic.color = colorState.personNormalColor;
        this.enabled = true;
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
        wealthText.text = wealthNum.ToString() + "/100";
        if (isDead)
            SetDead();

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
        reputationText.text = reputationNum.ToString() + "/100";
        if (isDead)
            SetDead();

        return isDead;
    }

    private void SetDead()
    {
        targetGraphic.color = colorState.deadColor;
        this.isSelected = false;
        this.enabled = false;
    }

    public void SetUnselectable()
    {
        targetGraphic.color = colorState.unselectableColor;
        this.isSelected = false;
        this.enabled = false;
    }

    public void SetSelected()
    {
        targetGraphic.color = colorState.selectColor;
        this.isSelected = true;
        this.enabled = false;
    }

    public bool IsDead()
    {
        return isDead;
    }

    public void SetAlive()
    {
        isDead = false;
        this.enabled = true;
        targetGraphic.color = colorState.personNormalColor;
    }
}
