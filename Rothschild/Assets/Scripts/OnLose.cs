using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OnLose : MonoBehaviour
{
    GameObject loseInterface;
    GameObject winInterface;
    List<OnPerson> person = new List<OnPerson>();
    bool isLose = false;
    bool isFinish = false;
    double loadTime;
    GameObject teamwork;
    LevelManager levelManager;

    // Use this for initialization
    void Start()
    {

        levelManager = GameObject.Find("LogicHandler").GetComponent<LevelManager>();
        winInterface = GameObject.Find("WinInterface");
        loseInterface = GameObject.Find("LoseInterface");
        GameObject.Find("Restart").GetComponent<Button>().onClick.AddListener(Retry);
        GameObject.Find("Exit").GetComponent<Button>().onClick.AddListener(Quit);
        GameObject.Find("WinRestart").GetComponent<Button>().onClick.AddListener(Retry);
        GameObject.Find("WinExit").GetComponent<Button>().onClick.AddListener(Quit);
        //GameObject.Find("WTF").GetComponent<Button>().onClick.AddListener(Retry);
        loseInterface.SetActive(false);
        winInterface.SetActive(false);

        person.Add(GameObject.Find("PersonPanelA").GetComponent<OnPerson>());
        person.Add(GameObject.Find("PersonPanelB").GetComponent<OnPerson>());
        person.Add(GameObject.Find("PersonPanelC").GetComponent<OnPerson>());
        person.Add(GameObject.Find("PersonPanelD").GetComponent<OnPerson>());

        teamwork = GameObject.Find("Fill");

        loadTime = Time.fixedTime;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Time.fixedTime - loadTime < 3 || isLose)
            return;
        ListenLose();
        ListenWin();
    }

    void Initalize()
    {

    }

    void Retry()
    {
        //GameObject[] allGameObject = Transform.FindObjectsOfType<GameObject>();
        //foreach (GameObject go in allGameObject)
        //{
        //    if (go != this.gameObject)
        //    {
        //        go.SetActive(true);
        //        Resources.UnloadAsset(go);
        //    }     
        //}

        SceneManager.LoadScene("WithPic", LoadSceneMode.Single);
    }

    void Quit()
    {
        Application.Quit();
    }

    void ListenLose()
    {
        bool allDead = true;
        for (int i = 0; i < person.Count; i++)
        {
            if (person[i].IsDead() == false)
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

        if (teamwork.gameObject.transform.localScale.x < 0.02)
        {
            isLose = true;
            loseInterface.SetActive(true);
            return;
        }
    }

    void ListenWin()
    {
        if(isFinish==false && levelManager.IsFinish())
        {
            winInterface.SetActive(true);
            isFinish = true;
        }
    }
}
