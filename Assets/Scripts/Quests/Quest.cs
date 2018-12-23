using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Quest : MonoBehaviour {

    public string descriptionLocKey;
    public string defaultDescription;
    public uint questPointGain = 1;
    protected GameManager _gameManager;
    private bool isFinished = false;

    public bool IsFinished
    {
        get
        {
            return isFinished;
        }

        set
        {
            isFinished = value;
        }
    }

    public virtual bool IsQuestCompleted() {
        throw new NotImplementedException();
    }

    public virtual string SpecificInformation() {
        throw new NotImplementedException();
    }

    public virtual string GetDescription()
    {
        throw new NotImplementedException();
    }
    public void LinkGameManager () {
        _gameManager = FindObjectOfType<GameManager>();
	}

}
