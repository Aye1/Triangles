using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestsPopup : Popup {

    List<Quest> _currentQuests;
    public QuestEntry questPlaceHolder;
    public GameObject questContainer;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnEnable()
    {
        List<Quest> newQuests = FindObjectOfType<GameManager>().currentQuests;
        bool shouldPopulate = newQuests != _currentQuests;
        _currentQuests = newQuests;
        if (shouldPopulate)
        {
            Debug.Log("Quests changed, populating");
            PopulatePopup();
        }
    }

    private void PopulatePopup() {
        foreach(Quest q in _currentQuests) {
            QuestEntry newEntry = Instantiate(questPlaceHolder).GetComponentInChildren<QuestEntry>();
            newEntry.quest = q;
            newEntry.gameObject.SetActive(true);
            newEntry.transform.parent = questContainer.transform;
        }
    }

    /*private void UpdateQuestsInfos() {
        foreach(GameObject q in questContainer.GetComponentInChildren<GameObject>()) {
            TextMeshProUGUI[] texts = newEntry.GetComponentsInChildren<TextMeshProUGUI>();
            texts[1].text = q.SpecificInformation();
        }
    }*/
}
