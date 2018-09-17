using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnIntroduction : MonoBehaviour
{

    int introductionNum = 7;
    List<GameObject> introduction = new List<GameObject>();

    // Use this for initialization
    void Start()
    {
        GameObject.Find("NextPage").GetComponent<Button>().onClick.AddListener(NextPage);
        GameObject.Find("ForePage").GetComponent<Button>().onClick.AddListener(ForePage);

        introduction.Add(GameObject.Find("IntroductionD2"));
        introduction.Add(GameObject.Find("IntroductionD"));
        introduction.Add(GameObject.Find("IntroductionC2"));
        introduction.Add(GameObject.Find("IntroductionC"));
        introduction.Add(GameObject.Find("IntroductionB2"));
        introduction.Add(GameObject.Find("IntroductionB"));
        introduction.Add(GameObject.Find("IntroductionA2"));
        introduction.Add(GameObject.Find("IntroductionA"));
    }

    // Update is called once per frame
    void Update()
    {

    }

    void ForePage()
    {
        if (introductionNum == 7)
            return;
        introduction[++introductionNum].SetActive(true);
    }

    void NextPage()
    {
        if (introductionNum == 0)
            this.gameObject.SetActive(false);
        else
            introduction[introductionNum--].SetActive(false);
    }
}
