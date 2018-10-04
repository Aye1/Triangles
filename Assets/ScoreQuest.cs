using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreQuest : Quest {

    public uint targetScore;

    public GameManager gameManager;

    public ScoreQuest(uint score) {
        targetScore = score;
        defaultDescription = "_Atteignez " + targetScore.ToString() + " points";
    }

    public override bool IsQuestCompleted() {
        int currentScore = gameManager.globalScore;
        return currentScore >= targetScore;
    }

    public override string SpecificInformation()
    {
        return gameManager.globalScore.ToString() + "/" + targetScore.ToString();
    }
}
