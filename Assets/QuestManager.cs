using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class QuestManager : MonoBehaviour {

    private static QuestManager _instance;
    private Random _rand;
    public List<Quest> quests;

    public static QuestManager Instance {
        get { return _instance; }
    }

	// Use this for initialization
	void Start () {
        if(_instance != null && _instance != this) {
            Destroy(gameObject);
        } else {
            _instance = this;
        }
        DontDestroyOnLoad(gameObject);
        quests = new List<Quest>();
        PopulateQuests();
        _rand = new Random();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void PopulateQuests() {
        GameManager gm = FindObjectOfType<GameManager>();
        ScoreQuest sq = new ScoreQuest(100);
        sq.gameManager = gm;
        quests.Add(sq);

        ScoreQuest sq2 = new ScoreQuest(50);
        sq.gameManager = gm;
        quests.Add(sq2);
    }

    public Quest GetQuest() {
        int id = _rand.Next(0, quests.Count-1);
        Quest selectedQuest = quests.ToArray()[id];
        return selectedQuest;
    }
}
