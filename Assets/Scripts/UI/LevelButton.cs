using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButton : MonoBehaviour {

    public int id;
    public int pointsNeeded;
    public Button goToLevelButton;
    public Button unlockButton;
    //public LevelInfo level;

	// Use this for initialization
	void Start () {
        goToLevelButton.onClick.AddListener(LoadLevel);
        unlockButton.onClick.AddListener(UnlockLevel);
        unlockButton.GetComponentInChildren<Text>().text = pointsNeeded.ToString();
	}
	
	// Update is called once per frame
	void Update () {
        bool unlocked = LevelManager.Instance.IsLevelAvailable(id);
        goToLevelButton.interactable = unlocked;
        unlockButton.gameObject.SetActive(!unlocked);
        unlockButton.interactable = PlayerSettingsManager.Instance.QuestsPoints >= pointsNeeded;
	}

    private void LoadLevel(){
        FindObjectOfType<SceneController>().GoToGameScreen(id);
    }

    private void UnlockLevel() {
        PlayerSettingsManager.Instance.UnlockLevel(id, pointsNeeded);
    }
}
