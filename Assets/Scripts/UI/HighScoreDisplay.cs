using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighScoreDisplay : TextAnimation {
    protected override void InternalStart()
    {
        FindObjectOfType<GameManager>().OnHighScoreChange += VariableChangeHandler;
    }

    private void VariableChangeHandler(int newVal)
    {
        PlayAnimation();
    }
}
