using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EndGamePopup : Popup {

    public GameObject scoreObject;
    public GameObject questsObject;
    public Image highScoreImage;
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
        bool isNewHighScore = (_gm.HighScore == _gm.GlobalScore);
        UIHelper.DisplayGameObject(highScoreImage.gameObject, isNewHighScore);
        
        scoreObject.GetComponentInChildren<TextMeshProUGUI>().text = _gm.GlobalScore.ToString();
    }

    private void OnEnable()
    {
        if (_gm != null)
        {    
            int finishedQuestsCount = _gm.finishedQuests.Count;
            if (finishedQuestsCount == 0)
            {
                questsObject.GetComponentInChildren<TextMeshProUGUI>().text = LocalizationManager.Instance.GetLocString(noQuestLocKey);
            }
            else if (finishedQuestsCount == 1)
            {
                questsObject.GetComponentInChildren<TextMeshProUGUI>().text = LocalizationManager.Instance.GetLocString(questCompletedLocKey);
            }
            else
            {
                //TODO if we can have more than 1 quest
            }
        }
    }
}
