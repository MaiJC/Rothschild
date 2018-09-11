using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnCardClicked : EventTrigger
{
    private Graphic targetGraphic;
    //private Image image;
    private bool isSelected = false;

    private Color normalColor;
    private Color clickColor;
    private Color enterColor;
    private Color selectColor;

    private InterfaceManager interfaceManager;

    // Use this for initialization
    void Start()
    {
        this.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;


        interfaceManager = GameObject.Find("LogicHandler").GetComponent<InterfaceManager>();
        targetGraphic = this.GetComponent<Button>().targetGraphic;
        ColorState colorState = this.GetComponent<ColorState>();
        normalColor = colorState.normalColor;
        clickColor = colorState.clickColor;
        enterColor = colorState.enterColor;
        selectColor = colorState.selectColor;

        targetGraphic.color = normalColor;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        //base.OnPointerEnter(eventData);
        targetGraphic.color = enterColor;
        //image.color = enterColor;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        //base.OnPointerExit(eventData);
        if (isSelected)
        {
            targetGraphic.color = selectColor;
            //image.color = selectColor;
        }
        else
        {
            targetGraphic.color = normalColor;
            //image.color = normalColor;
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        targetGraphic.color = clickColor;
        //image.color = clickColor;
        isSelected = !isSelected;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        //targetGraphic.color = enterColor;
        if (isSelected)
        {
            targetGraphic.color = selectColor;
            //image.color = selectColor;
        }
        else
        {
            targetGraphic.color = normalColor;
            //image.color = normalColor;
        }
        if (tag == "ConfirmButton")
        {
            interfaceManager.OnConfirm();
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

}
