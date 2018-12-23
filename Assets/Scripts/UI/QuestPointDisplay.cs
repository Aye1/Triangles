using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestPointDisplay : TextAnimation {
    protected override void InternalStart()
    {
       PlayerSettingsManager.Instance.OnVariableChange += VariableChangeHandler;
    }

    private void VariableChangeHandler(int newVal)
    {
        PlayAnimation();
    }
}
