using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboQuest : Quest {

    public uint numberComboTargetScore;

    private void Update()
    {
        defaultDescription = "_Faites " + numberComboTargetScore.ToString() + " Combos";
    }

    public override bool IsQuestCompleted()
    {
        int currentScore = _gameManager.comboScore;
        return currentScore >= numberComboTargetScore;
    }

    public override string SpecificInformation()
    {
        uint currentScore = (uint)_gameManager.comboScore;
        uint displayedScore = currentScore < numberComboTargetScore ? currentScore : numberComboTargetScore;
        return displayedScore.ToString() + "/" + numberComboTargetScore.ToString();
    }
}
