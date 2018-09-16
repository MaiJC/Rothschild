using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OnChecckIsDead : MonoBehaviour
{

    private List<OnPerson> onPeople = new List<OnPerson>();
    private Image teamWorkBar;
    private GameObject loseInterface;
    private bool hasLose = false;



    // Use this for initialization
    void Start()
    {
        onPeople.Add(GameObject.Find("PersonPanelA").gameObject.GetComponent<OnPerson>());
        onPeople.Add(GameObject.Find("PersonPanelB").gameObject.GetComponent<OnPerson>());
        onPeople.Add(GameObject.Find("PersonPanelC").gameObject.GetComponent<OnPerson>());
        onPeople.Add(GameObject.Find("PersonPanelD").gameObject.GetComponent<OnPerson>());
        teamWorkBar = GameObject.Find("Fill").GetComponent<Image>();
        loseInterface = GameObject.Find("LoseInterface").gameObject;
        loseInterface.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(hasLose==false)
        {
            if (teamWorkBar.transform.localScale.x < 0.02)
                loseInterface.SetActive(true);
            else
            {
                bool lose = true;
                foreach (OnPerson p in onPeople)
                    if (p.IsDead() == false)
                        lose = true;
                if (lose)
                    loseInterface.SetActive(true);
            }
            hasLose = true;
        }
    }

    void Restart()
    {
        SceneManager.LoadScene(0);
    }

    void Exist()
    {
        Application.Quit();
    }

}
