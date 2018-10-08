public class ScoreQuest : Quest {

    public uint targetScore;

    private void Update()
    {
        defaultDescription = "_Atteignez " + targetScore.ToString() + " points";
    }

    public override bool IsQuestCompleted() {
        int currentScore = _gameManager.globalScore;
        return currentScore >= targetScore;
    }

    public override string SpecificInformation()
    {
        uint currentScore = (uint)_gameManager.globalScore;
        uint displayedScore = currentScore < targetScore ? currentScore : targetScore;
        return displayedScore.ToString() + "/" + targetScore.ToString();
    }
}
