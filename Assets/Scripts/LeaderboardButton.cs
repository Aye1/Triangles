using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LeaderboardButton : MonoBehaviour
{
    public LeaderboardPopup leaderBoardUI;

    // Use this for initialization
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(DisplayLeaderboard);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DisplayLeaderboard()
    {
        leaderBoardUI.RefreshDisplay();
        UIHelper.DisplayGameObject(leaderBoardUI.gameObject);
    }

    public void CloseLeaderboard()
    {
        UIHelper.HideGameObject(leaderBoardUI.gameObject);
    }
}