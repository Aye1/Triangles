using TMPro;
using UnityEngine;

public class ScoreDisplay : TextAnimation
{
    protected override void InternalStart()
    {
        FindObjectOfType<GameManager>().OnScoreChange += VariableChangeHandler;
    }

    private void VariableChangeHandler(int newVal)
    {
        PlayAnimation();
    }
}
