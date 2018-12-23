using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EndGamePopup : Popup {

    public GameObject scoreObject;
    public GameObject questsObject;
    private GameManager _gm;

    private readonly string noQuestLocKey = "no_quest";
    private readonly string questCompletedLocKey = "quest_completed";

	// Use this for initialization
	void Awake () {
        _gm = FindObjectOfType<GameManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void UpdateHighScoreInfo(bool shouldDisplay) {
        //UIHelper.DisplayGameObject(highScoreObject, shouldDisplay);
        UIHelper.DisplayGameObject(scoreObject.GetComponentInChildren<Image>().gameObject, _gm.HighScore == _gm.GlobalScore);
        
        scoreObject.GetComponentInChildren<TextMeshProUGUI>().text = _gm.GlobalScore.ToString();
    }

    private void OnEnable()
    {
        if (_gm != null)
        {    
            int finishedQuestsCount = _gm.finishedQuests.Count;
            if (finishedQuestsCount == 0)
            {
                questsObject.GetComponentInChildren<TextMeshProUGUI>().text = LocalizationManager.Instance.GetLocString(noQuestLocKey, PlayerSettingsManager.Instance.CurrentLocale);
            }
            else if (finishedQuestsCount == 1)
            {
                questsObject.GetComponentInChildren<TextMeshProUGUI>().text = LocalizationManager.Instance.GetLocString(questCompletedLocKey, PlayerSettingsManager.Instance.CurrentLocale);
            }
            else
            {
                //TODO if we can have more than 1 quest
            }
        }
    }
}
