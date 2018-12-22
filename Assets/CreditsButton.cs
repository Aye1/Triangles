using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CreditsButton : MonoBehaviour {


	public Popup creditsPopup;

	//use this for initialization
	void Start () {
        GetComponent<Button>().onClick.AddListener(OpenPopup);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OpenPopup() {
        UIHelper.DisplayGameObject(creditsPopup.gameObject);
	}
}
