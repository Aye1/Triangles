using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{

    // Leaderboard managed by Dreamlo
    // Private url
    // http://dreamlo.com/lb/ainU-KkNx0Wl5Fki7YfFKwlbJ0wzSPpEqF8EH2jjth-A

    private DreamloLeaderBoard _dreamLoLB;
    private List<Score> _currentScores;

    public GameObject leaderBoardUI;

    public List<Score> CurrentScores
    {
        get
        {
            return _currentScores;
        }

        set
        {
            _currentScores = value;
        }
    }

    // Use this for initialization
    void Start()
    {
        _dreamLoLB = GetComponent<DreamloLeaderBoard>();
        _dreamLoLB.LoadScores();
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SendHighScore(string name, int score)
    {
        _dreamLoLB.AddScore(name, score);
    }

    public void LoadScores()
    {
        List<Score> scores = new List<Score>();
        foreach (DreamloLeaderBoard.Score score in _dreamLoLB.ToListHighToLow())
        {
            Score simpleScore = Score.FromDreamLoScore(score);
            scores.Add(simpleScore);
        }
        CurrentScores = scores;
    }

    public void DisplayLeaderboard() {
        UIHelper.DisplayGameObject(leaderBoardUI);
    }

    public void CloseLeaderboard() {
        UIHelper.HideGameObject(leaderBoardUI);
    }
}
