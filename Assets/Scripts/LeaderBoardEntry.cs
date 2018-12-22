using UnityEngine;
using TMPro;

public class LeaderBoardEntry : MonoBehaviour {

    public Score score;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI scoreText;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        nameText.text = score.name;
        scoreText.text = score.score.ToString();
	}
}
