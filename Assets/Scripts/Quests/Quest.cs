using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Quest : MonoBehaviour {

    public string descriptionLocKey;
    public string defaultDescription;
    public uint questPointGain = 1;
    protected GameManager _gameManager;

    public virtual bool IsQuestCompleted() {
        throw new NotImplementedException();
    }

    public virtual string SpecificInformation() {
        throw new NotImplementedException();
    }

    // Use this for initialization
    void Start () {
        _gameManager = FindObjectOfType<GameManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
