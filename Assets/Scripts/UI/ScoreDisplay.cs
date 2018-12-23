using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour {
    [Header("UI objects")]
    public TextMeshProUGUI textScore;
    // Use this for initialization
    private void Update()
    {
        textScore.text = PlayerSettingsManager.Instance.QuestsPoints.ToString();
    }
}
