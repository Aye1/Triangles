using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class QuestManager : MonoBehaviour {

    private static QuestManager _instance;
    private Random _rand;
    public List<Quest> quests;
    public ScoreQuest scoreQuest;
    public ComboQuest comboQuest;
    public NumberLineQuest lineQuest;
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
        ScoreQuest sq = Instantiate(scoreQuest);
        sq.targetScore = 10;
        quests.Add(sq);
       
        sq = Instantiate(scoreQuest);
        sq.targetScore = 20;
        quests.Add(sq);
       
        sq = Instantiate(scoreQuest);
        sq.targetScore = 30;
        sq.questPointGain = 2;
        quests.Add(sq);

        ComboQuest cq = Instantiate(comboQuest);
        cq.numberComboTargetScore = 1;
        cq.targetCombo = Combo.Combo2;
        cq.questPointGain = 5;
        quests.Add(cq);

        NumberLineQuest nq = Instantiate(lineQuest);
        nq.numberComboTargetScore = 10;
        nq.questPointGain = 2;
        quests.Add(nq);
    }

    public Quest GetQuest() {
        int id = _rand.Next(0, quests.Count);
        Quest selectedQuest = quests.ToArray()[id];
        return Instantiate(selectedQuest);
    }
}
