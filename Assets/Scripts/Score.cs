[System.Serializable]
public class Score
{
    public string name;
    public int score;

    public static Score FromDreamLoScore(DreamloLeaderBoard.Score dlScore) {
        Score sc = new Score();
        sc.name = dlScore.playerName;
        sc.score = dlScore.score;
        return sc;
    }
}
