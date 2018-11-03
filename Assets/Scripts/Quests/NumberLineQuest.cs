public class NumberLineQuest : ComboQuest {

    private void Awake()
    {
        targetCombo = Combo.Combo1;
    }
    private void Update()
    {
        defaultDescription = "_Faites " + numberComboTargetScore.ToString() + " lignes";
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
