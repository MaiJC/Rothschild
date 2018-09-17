using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnIntroduction : MonoBehaviour
{

    int introductionNum = 3;
    List<GameObject> introduction = new List<GameObject>();
    ColorState colorState;

    // Use this for initialization
    void Start()
    {
        colorState = GameObject.Find("ColorState").GetComponent<ColorState>();
        GameObject.Find("NextPage").GetComponent<Button>().onClick.AddListener(NextPage);
        GameObject.Find("ForePage").GetComponent<Button>().onClick.AddListener(ForePage);

        GameObject.Find("ForePage").GetComponent<Button>().targetGraphic.color = colorState.unselectableColor;

        introduction.Add(GameObject.Find("IntroductionD"));
        introduction.Add(GameObject.Find("IntroductionC"));
        introduction.Add(GameObject.Find("IntroductionB"));
        introduction.Add(GameObject.Find("IntroductionA"));
    }

    // Update is called once per frame
    void Update()
    {

    }

    void ForePage()
    {
        if (introductionNum == 3)
        {
            return;
        }
        introduction[++introductionNum].SetActive(true);
        if (introductionNum == 3)
        {
            GameObject.Find("ForePage").GetComponent<Button>().targetGraphic.color = colorState.unselectableColor;
        }
    }

    void NextPage()
    {
        GameObject.Find("ForePage").GetComponent<Button>().targetGraphic.color = colorState.buttonNormalColor;
        if (introductionNum == 0)
            this.gameObject.SetActive(false);
        else
            introduction[introductionNum--].SetActive(false);
    }
}
