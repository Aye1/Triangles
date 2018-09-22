using UnityEngine;

public class LeaderboardPopup : MonoBehaviour {

    public int entryCount;
    public Transform contentPanel;
    private LeaderboardManager _lbManager;
    public SimpleObjectPool entriesPool;

	// Use this for initialization
	void Start () {
        _lbManager = FindObjectOfType<LeaderboardManager>();
        RefreshDisplay();
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
        while(contentPanel.childCount > 0) {
            GameObject toRemove = transform.GetChild(0).gameObject;
            entriesPool.ReturnObject(toRemove);
        }
    }

    private void PopulateEntries() {
        for (int i = 0; i < entryCount; i++) {
            GameObject newEntry = entriesPool.GetObject();
            LeaderBoardEntry entry = newEntry.GetComponent<LeaderBoardEntry>();
            entry.enabled = true;

            entry.score.name = "Player " + i.ToString();
            entry.score.score = i * 100;

            newEntry.transform.SetParent(contentPanel);
        }
    }
}
