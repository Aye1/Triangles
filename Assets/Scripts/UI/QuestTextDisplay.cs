using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestTextDisplay : TextAnimation {

    protected override void InternalStart()
    {
        FindObjectOfType<GameManager>().IsCurrentQuestFinishedEvent += VariableChangeHandler;
    }
    private void VariableChangeHandler(bool newVal)
    {
        if(newVal)
        {
            PlayAnimation();
        }
    }
}
