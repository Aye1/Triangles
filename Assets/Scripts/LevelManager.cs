using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    public int levelCount = 2;

    private static LevelManager _instance;
    private Dictionary<int, int> _levelsUnlocked;

    public LevelInfo[] levels;

    public static LevelManager Instance {
        get { return _instance; }
    }

	// Use this for initialization
	void Start () {
        if(_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
        DontDestroyOnLoad(this);
        PlayerSettingsManager.Instance.LoadLevelsUnlocked();
	}

    public bool IsLevelAvailable(int level) {
        return PlayerSettingsManager.Instance.IsLevelUnlocked(level);
    }

    private int GetNextLevel() {
        throw new NotImplementedException();
    }

    public Level GetLevelAtIndex(int index) {
        return index < levels.Length ? levels[index].level : null;
    }
}
