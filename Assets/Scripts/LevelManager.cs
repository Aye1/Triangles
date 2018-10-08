using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    private int _maxLevel;

    private static LevelManager _instance;

    public static LevelManager Instance {
        get { return _instance; }
    }

    public int MaxLevel {
        get { return _maxLevel; }
    }

	// Use this for initialization
	void Start () {
        if(_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
        DontDestroyOnLoad(this);
	}

    public bool IsLevelAvailable(int level) {
        return level <= _maxLevel;
    }

    private int GetNextLevel() {
        throw new NotImplementedException();
    }
}
