public class NumberLineQuest : ComboQuest {

    private void Awake()
    {
        targetCombo = Combo.Combo1;
    }
    public override string GetDescription()
    {
        defaultDescription = "_Faites " + numberComboTargetScore.ToString() + " lignes";
        return defaultDescription;
    }
    protected override uint ComputeCurrentScore()
    {
        uint score = 0;
        for (Combo combo = targetCombo; combo < Combo.Max; combo++)
        {
            uint comboNumber = (uint)combo;
            score += (uint)_gameManager.comboArray[comboNumber] * comboNumber;
        }
        return score;
    }
}
