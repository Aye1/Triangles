using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BackButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
        this.GetComponent<Button>().onClick.AddListener(GoBackToMenu);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void GoBackToMenu() {
        SceneController.Instance.GoToMenuScreen();
    }
}
