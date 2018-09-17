using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OnLose : MonoBehaviour
{
    GameObject loseInterface;
    List<OnPerson> person = new List<OnPerson>();
    bool isLose = false;
    double loadTime;
    GameObject teamwork;

    // Use this for initialization
    void Start()
    {
<<<<<<< HEAD
        levelManager = GameObject.Find("LogicHandler").GetComponent<LevelManager>();
        winInterface = GameObject.Find("WinInterface");
=======
>>>>>>> b9a6213160e25931c6abc9ddbf15241def09ce79
        loseInterface = GameObject.Find("LoseInterface");
        GameObject.Find("Restart").GetComponent<Button>().onClick.AddListener(Retry);
        GameObject.Find("Exit").GetComponent<Button>().onClick.AddListener(Quit);
        loseInterface.SetActive(false);

        person.Add(GameObject.Find("PersonPanelA").GetComponent<OnPerson>());
        person.Add(GameObject.Find("PersonPanelB").GetComponent<OnPerson>());
        person.Add(GameObject.Find("PersonPanelC").GetComponent<OnPerson>());
        person.Add(GameObject.Find("PersonPanelD").GetComponent<OnPerson>());

        teamwork = GameObject.Find("Fill");

        loadTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time -loadTime < 2 || isLose)
            return;

        bool allDead = true;
        for(int i=0;i<person.Count;i++)
        {
            if(person[i].IsDead()==false)
            {
                allDead = false;
                break;
            }
        }
        if (allDead)
        {
            isLose = true;
            loseInterface.SetActive(true);
            return;
        }

        if(teamwork.gameObject.transform.localScale.x<0.02)
        {
            isLose = true;
            loseInterface.SetActive(true);
            return;
        }
    }

    void Retry()
    {
        SceneManager.LoadScene("Reload");
    }

    void Quit()
    {
        Application.Quit();
    }
}
