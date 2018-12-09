using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighScoreDisplay : MonoBehaviour {
    Animation gainAnimation;
    private void Start()
    {
        gainAnimation = FindObjectOfType<Animation>();
        FindObjectOfType<GameManager>().OnHighScoreChange += VariableChangeHandler;
    }

    private void VariableChangeHandler(int newVal)
    {
        gainAnimation.Play();
    }
}
