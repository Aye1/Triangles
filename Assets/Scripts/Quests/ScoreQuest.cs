public class ScoreQuest : Quest {

    public uint targetScore;

    public override string GetDescription()
    {
        defaultDescription = "_Atteignez " + targetScore.ToString() + " points";
        return defaultDescription;
    }

    public override bool IsQuestCompleted() {
        int currentScore = _gameManager.GlobalScore;
        return currentScore >= targetScore;
    }

    public override string SpecificInformation()
    {
        uint currentScore = (uint)_gameManager.GlobalScore;
        uint displayedScore = currentScore < targetScore ? currentScore : targetScore;
        return displayedScore.ToString() + "/" + targetScore.ToString();
    }
}
