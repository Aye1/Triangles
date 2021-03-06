﻿using UnityEngine;
using TMPro;

public class LeaderboardPopup : MonoBehaviour {

    public int entryCount;
    public Transform contentPanel;
    public SimpleObjectPool entriesPool;
    public TMP_InputField pseudoInputField;

    private int maxEntries = 50;

	// Use this for initialization
	void OnEnable () {
        RefreshDisplay();
        SetPseudonym();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void RefreshDisplay()
    {
        RemoveEntries();
        PopulateEntries();
    }

    private void RemoveEntries() {
        int loopCount = 0;
        int maxLoopCount = 1000;
        while(contentPanel.childCount > 0 && loopCount <= maxLoopCount) {
            GameObject toRemove = contentPanel.transform.GetChild(0).gameObject;
            entriesPool.ReturnObject(toRemove);
            loopCount++;
        }
        if(loopCount == maxLoopCount)
        {
            Debug.Log("Max loop iterations reached");
        }
    }

    private void PopulateEntries() {
        if (LeaderboardManager.Instance.CurrentScores != null)
        {
            int count = 0;
            foreach (Score s in LeaderboardManager.Instance.CurrentScores)
            {
                CreateLeaderboardEntry(s);
                count++;
                if(count == maxEntries) {
                    return;
                }
            }
        }
    }

    private void FakePopulateEntries() {
        for (int i = 0; i < entryCount; i++) {
            Score s = new Score();
            s.name = "Player " + i.ToString();
            s.score = i * 100;
            CreateLeaderboardEntry(s);
        }
    }

    private void CreateLeaderboardEntry(Score s) {
        GameObject newEntry = entriesPool.GetObject();
        LeaderBoardEntry entry = newEntry.GetComponent<LeaderBoardEntry>();
        entry.enabled = true;

        entry.score.name = s.name;
        entry.score.score = s.score;

        newEntry.transform.SetParent(contentPanel);
        newEntry.transform.position = new Vector3(newEntry.transform.position.x, newEntry.transform.position.y, -1.0f);
    }

    public void OnPseudoTextFielEndEdit(string value) {
        PlayerSettingsManager.Instance.Name = value;
    }

    private void SetPseudonym() {
        pseudoInputField.text = PlayerSettingsManager.Instance.Name;
    }
}
