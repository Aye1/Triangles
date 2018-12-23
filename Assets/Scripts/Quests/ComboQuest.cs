using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboQuest : Quest {

    public uint numberComboTargetScore;
    public Combo targetCombo = Combo.Combo1;
    public override string GetDescription()
    {
        defaultDescription = "_Faites " + numberComboTargetScore.ToString() + " fois un combo de "+ ((int)targetCombo + 1) + " lignes ou plus";
        return defaultDescription;
    }

    public override bool IsQuestCompleted()
    {
        uint currentScore = ComputeCurrentScore();
        return currentScore >= numberComboTargetScore;
    }

    public override string SpecificInformation()
    {
        uint currentScore = ComputeCurrentScore();
        uint displayedScore = currentScore < numberComboTargetScore ? currentScore : numberComboTargetScore;
        return displayedScore.ToString() + "/" + numberComboTargetScore.ToString();
    }
    protected virtual uint ComputeCurrentScore()
    {
        uint score = 0;
        for(Combo combo = targetCombo; combo < Combo.Max; combo ++)
        {
            score += (uint)_gameManager.comboArray[(int)combo];
        }
        return score;
    }
}
