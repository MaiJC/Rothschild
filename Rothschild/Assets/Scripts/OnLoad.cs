using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OnLoad : MonoBehaviour {

    double loadTime;
    bool loadWithPic = false;

	// Use this for initialization
	void Start () {
        loadTime = Time.time;
        Resources.UnloadUnusedAssets();
        SceneManager.UnloadSceneAsync("WithPic");
    }
	
	// Update is called once per frame
	void Update () {
        if (Time.time - loadTime < 5 || loadWithPic == true)
            return;

        SceneManager.LoadScene("WithPic", LoadSceneMode.Single);
        loadWithPic = true;
    }
}
