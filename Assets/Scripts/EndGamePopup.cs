using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndGamePopup : Popup {

    public GameObject highScoreObject;
    public GameObject questsObject;
    private GameManager _gm;

	// Use this for initialization
	void Awake () {
        _gm = FindObjectOfType<GameManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void DisplayHighScoreInfo(bool shouldDisplay) {
        UIHelper.DisplayGameObject(highScoreObject, shouldDisplay);
        highScoreObject.GetComponentInChildren<TextMeshProUGUI>().text = "New high score - " +  _gm.HighScore + "!";
    }

    private void OnEnable()
    {
        if (_gm != null)
        {
            List<Quest> quests = _gm.currentQuests;
            List<Quest> finishedQuests = new List<Quest>();
            uint questPointGain = 0;
            foreach (Quest q in quests)
            {
                if (q.IsQuestCompleted())
                {
                    finishedQuests.Add(q);
                    questPointGain += q.questPointGain;
                }
            }
            
            int finishedQuestsCount = finishedQuests.Count;
            if (finishedQuestsCount == 0)
            {
                questsObject.GetComponentInChildren<TextMeshProUGUI>().text = "No quest completed";
            }
            else if (finishedQuestsCount == 1)
            {
                questsObject.GetComponentInChildren<TextMeshProUGUI>().text = "1 quest completed!";
            }
            else
            {
                questsObject.GetComponentInChildren<TextMeshProUGUI>().text = finishedQuests.ToString() + " quests completed!";
            }
            _gm.ReplaceQuests(finishedQuests);
            PlayerSettingsManager.Instance.QuestsPoints+= (int)questPointGain;
        }
    }
}
