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
	void Awake () {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;

            DontDestroyOnLoad(gameObject);
            quests = new List<Quest>();
            PopulateQuests();
            _rand = new Random();
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void PopulateQuests() {
        ScoreQuest sq = Instantiate(scoreQuest);
        sq.targetScore = 50;
        sq.questPointGain = 1;
        AddQuest(sq);

        ComboQuest cq = Instantiate(comboQuest);
        cq.numberComboTargetScore = 1;
        cq.targetCombo = Combo.Combo3;
        cq.questPointGain = 5;
        AddQuest(cq);

        cq.numberComboTargetScore = 2;
        cq.targetCombo = Combo.Combo2;
        cq.questPointGain = 5;
        AddQuest(cq);

        NumberLineQuest nq = Instantiate(lineQuest);
        nq.numberComboTargetScore = 10;
        nq.questPointGain = 2;
        AddQuest(nq);
    }

    private void AddQuest(Quest q)
    {
        quests.Add(q);
        q.transform.SetParent(transform);
    }

    public Quest GetQuest() {
        int id = _rand.Next(0, quests.Count);
        Quest selectedQuest = quests.ToArray()[id];
        return Instantiate(selectedQuest);
    }
}
