using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestPointDisplay : MonoBehaviour {
    Animation gainAnimation;
    private void Start()
    {
        gainAnimation = FindObjectOfType<Animation>();
       PlayerSettingsManager.Instance.OnVariableChange += VariableChangeHandler;
    }

    private void VariableChangeHandler(int newVal)
    {
        gainAnimation.Play();
    }
}
