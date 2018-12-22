using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LevelScreenButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Button>().onClick.AddListener(GoToLevelsScreen);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void GoToLevelsScreen(){
        SceneController.Instance.GoToLevelsScreen();
    }
}
