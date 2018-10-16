using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LevelButton : MonoBehaviour {

    public int id;
    //public LevelInfo level;

	// Use this for initialization
	void Start () {
        this.GetComponent<Button>().onClick.AddListener(LoadLevel);
	}
	
	// Update is called once per frame
	void Update () {
        bool unlocked = LevelManager.Instance.IsLevelAvailable(id);
        this.GetComponent<Button>().interactable = unlocked;
	}

    private void LoadLevel(){
        FindObjectOfType<SceneController>().GoToGameScreen(id);
    }
}
