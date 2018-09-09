using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnColorChange : EventTrigger {

    private Graphic targetGraphic;
    private bool isSelected = false;

    private Color normalColor = Color.white;
    private Color clickColor = Color.black;
    private Color enterColor = Color.grey;
    private Color selectColor = Color.red;

	// Use this for initialization
	void Start () {
        targetGraphic = this.GetComponent<Button>().targetGraphic;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void OnPointerEnter(PointerEventData eventData)
    {
        //base.OnPointerEnter(eventData);
        targetGraphic.color = enterColor;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        //base.OnPointerExit(eventData);
        if (isSelected)
            targetGraphic.color = selectColor;
        else
            targetGraphic.color = normalColor;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        targetGraphic.color = clickColor;
        isSelected = !isSelected;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (isSelected)
            targetGraphic.color = selectColor;
        else
            targetGraphic.color = normalColor;
    }
}
