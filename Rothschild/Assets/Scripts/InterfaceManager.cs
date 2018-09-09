using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterfaceManager : MonoBehaviour {

    public GameObject myEvent;

    private ArrayList currentEventList = new ArrayList();

	// Use this for initialization
	void Start () {
        //GameObject tmp = (GameObject)Instantiate(myEvent, new Vector3(0, 0, 0), Quaternion.identity);
        //GameObject canvas = GameObject.Find("Canvas");
        //tmp.transform.SetParent(canvas.transform);

        //currentEventList.Add(tmp);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
