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
    private GameObject wealthArror;
    //名望
    private Text reputationText;
    private int reputationNum;
    private GameObject reputationArror;
    //是否死亡
    private bool isDead = false;

    private bool hasInitalize = false;

    private GameObject frame;

    private List<double> monkeyInput = new List<double>();

    bool isMonkey = false;

    double loadTime;


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

        loadTime = Time.time;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (hasInitalize == false && Time.fixedTime - loadTime > 2)
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
            StartFrame();
            DeactiveFrame();
        }
    }

    private void Update()
    {
        for (int i = 0; i < monkeyInput.Count; i++)
        {
            if (Time.time - monkeyInput[i] > 2)
            {
                monkeyInput.RemoveAt(i);
                i--;
            }
        }
        if (monkeyInput.Count > 12)
        {
            if (!isMonkey)
            {
                transform.GetChild(0).gameObject.GetComponent<Image>().overrideSprite
                    = Resources.Load("monkey", typeof(Sprite)) as Sprite;
                isMonkey = true;
                monkeyInput.Clear();
            }
            else
            {
                transform.GetChild(0).gameObject.GetComponent<Image>().overrideSprite = null;
                isMonkey = false;
                monkeyInput.Clear();
            }
        }
<<<<<<< HEAD
        if (hasInitalize == false && Time.fixedTime - loadTime > 1.5)
        {
            targetGraphic = this.transform.GetChild(0).GetComponent<Image>();
            //targetGraphic = this.GetComponent<Button>().targetGraphic;
            //colorState = this.GetComponent<ColorState>();
            colorState = GameObject.Find("ColorState").GetComponent<ColorState>();

            targetGraphic.color = colorState.personNormalColor;

            levelManager = GameObject.Find("LogicHandler").GetComponent<LevelManager>();
            avatarImage = this.transform.GetChild(0).gameObject.GetComponent<Image>();
            wealthText = this.transform.GetChild(1).gameObject.GetComponent<Text>();
            reputationText = this.transform.GetChild(2).gameObject.GetComponent<Text>();
            wealthArror = this.transform.GetChild(3).gameObject;
            reputationArror = this.transform.GetChild(4).gameObject;
            wealthArror.SetActive(false);
            reputationArror.SetActive(false);

            hasInitalize = true;
            StartFrame();
            DeactiveFrame();
        }

=======
>>>>>>> b9a6213160e25931c6abc9ddbf15241def09ce79
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        //targetGraphic.color = colorState.enterColor;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (isSelected)
        {
            //targetGraphic.color = colorState.selectColor;
        }
        else
        {
            //targetGraphic.color = colorState.personNormalColor;
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        //targetGraphic.color = colorState.clickColor;
        //isSelected = !isSelected;
        //判断是否能够选择
        if (isSelected == false)
        {
            if (levelManager.AddSelect(this.tag))
            {
                isSelected = true;
                ActiveFrame();
            }
        }
        else
        {
            levelManager.RemoveSelect(this.tag);
            DeactiveFrame();
            isSelected = false;
        }
        monkeyInput.Add(Time.time);

    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        //if (isSelected)
        //{
        //    //targetGraphic.color = colorState.selectColor;
        //}
        //else
        //{
        //    //targetGraphic.color = colorState.personNormalColor;
        //}
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
        //targetGraphic.color = colorState.personNormalColor;
        this.enabled = true;
        DeactiveFrame();
    }

    public void SetAvator(string avatarName)
    {
        avatarImage.sprite = Resources.Load(avatarName, typeof(Sprite)) as Sprite;
    }

    private bool hasSetWealth = false;
    /*接收值为财富值的改变，返回角色是否死亡，返回true则为死亡*/
    public bool SetWealth(int wealth)
    {
        if (wealthNum != wealth && hasSetWealth == true)
        {
            wealthArror.SetActive(true);
            int direction = wealth > wealthNum ? 1 : -1;
            wealthArror.transform.localScale = new Vector3(Mathf.Abs(wealthArror.transform.localScale.x) * direction
                , wealthArror.transform.localScale.y, 1);
            if (direction == 1)
                wealthArror.GetComponent<Image>().color = Color.red;
            else
                wealthArror.GetComponent<Image>().color = Color.green;
        }
        else if (hasSetWealth == true)
        {
            wealthArror.SetActive(false);
        }
        hasSetWealth = true;

        wealthNum = wealth;
        isDead = (wealthNum <= 0 || wealthNum >= 100);
        wealthNum = Mathf.Clamp(wealthNum, 0, 100);
        wealthText.text = wealthNum.ToString() + "/100";
        if (isDead)
            SetDead();

        return isDead;
    }
    private bool hasSetReputation = false;
    /*接收值为声望的改变，返回角色是否死亡，返回true则为死亡*/
    public bool SetReputation(int reputation)
    {
        if (reputationNum != reputation && hasSetReputation == true)
        {
            reputationArror.SetActive(true);
            int direction = reputation > reputationNum ? 1 : -1;
            reputationArror.transform.localScale = new Vector3(Mathf.Abs(reputationArror.transform.localScale.x) * direction
                , reputationArror.transform.localScale.y, 1);

            if (direction == 1)
                reputationArror.GetComponent<Image>().color = Color.red;
            else
                reputationArror.GetComponent<Image>().color = Color.green;
        }
        else if (hasSetReputation == true)
        {
            reputationArror.SetActive(false);
        }
        hasSetReputation = true;

        reputationNum = reputation;
        isDead = (reputationNum <= 0 || reputationNum >= 100);
        reputationNum = Mathf.Clamp(reputation, 0, 100);
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
        //targetGraphic.color = colorState.unselectableColor;
        this.isSelected = false;
        this.enabled = false;
    }

    public void SetSelected()
    {
        //targetGraphic.color = colorState.selectColor;
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
        //targetGraphic.color = colorState.personNormalColor;
    }

    private void ActiveFrame()
    {
        frame.SetActive(true);
    }

    private void DeactiveFrame()
    {
        frame.SetActive(false);
    }

    private void StartFrame()
    {
        switch (tag)
        {
            case "PersonOne":
                frame = GameObject.Find("SelectA");
                break;
            case "PersonTwo":
                frame = GameObject.Find("SelectB");
                break;
            case "PersonThree":
                frame = GameObject.Find("SelectC");
                break;
            case "PersonFour":
                frame = GameObject.Find("SelectD");
                break;
        }
    }
}
