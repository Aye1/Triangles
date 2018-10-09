using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class PlayerSettingsManager : MonoBehaviour {

    private static PlayerSettingsManager _instance;
    private string _currentLocale;
    private int _maxLevel;
    private int _currentLevel;
    private string _name;
    private int _questsPoints;
    private Random _rand;
    private int _highScore;
    private Dictionary<int, bool> _levelsUnlocked;

    public static string localeKey = "locale";
    public static string levelKey = "level";
    public static string nameKey = "name";
    public static string questPointsKey = "questPoints";
    public static string highScoreKey = "highScore";
    public static string levelUnlockedBaseKey = "levelUnlocked";


    #region Properties
    public static PlayerSettingsManager Instance {
        get { return _instance; }
    }

    public string CurrentLocale {
        get { return _currentLocale; }
    }

    public int MaxLevel {
        get { return _maxLevel; }
    }

    public string Name {
        get { return _name; }
    }

    public int QuestsPoints {
        get { return _questsPoints; }
        set 
        { 
            if(value != _questsPoints) {
                _questsPoints = value;
                PlayerPrefs.SetInt(questPointsKey, _questsPoints);
            }
        }
    }

    public int CurrentLevel {
        get { 
            if(_currentLevel == 0) {
                return _maxLevel;
            }
            return _currentLevel; 
        }
        set {
            if(value != _currentLevel) {
                _currentLevel = value;
            }
        }
    }

    public int HighScore {
        get {
            return _highScore;
        }
        set {
            if(value != _highScore) {
                _highScore = value;
                PlayerPrefs.SetInt(highScoreKey, _highScore);
            }
        }
    }
    #endregion

    // Use this for initialization
    void Awake () {
        if(_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
        _rand = new Random();
        DontDestroyOnLoad(gameObject);
        LoadPlayerSettings();
    }

    private void LoadPlayerSettings() {
        if (PlayerPrefs.HasKey(localeKey))
        {
            _currentLocale = PlayerPrefs.GetString(localeKey);
        } else {
            GetLocaleFromDevice();
        }

        if(PlayerPrefs.HasKey(levelKey)) {
            _maxLevel = PlayerPrefs.GetInt(levelKey);
        } else {
            _maxLevel = 1;
            PlayerPrefs.SetInt(levelKey, _maxLevel);
        }

        if(PlayerPrefs.HasKey(nameKey)) {
            _name = PlayerPrefs.GetString(nameKey);
        } else {
            _name = GenerateRandomName();
            PlayerPrefs.SetString(nameKey, _name);
        }

        if(PlayerPrefs.HasKey(questPointsKey)) {
            _questsPoints = PlayerPrefs.GetInt(questPointsKey);
        } else {
            _questsPoints = 0;
            PlayerPrefs.SetInt(questPointsKey, 0);
        }

        if (PlayerPrefs.HasKey(highScoreKey))
        {
            _highScore = PlayerPrefs.GetInt(highScoreKey);
        } else {
            _highScore = 0;
            PlayerPrefs.SetInt(highScoreKey, 0);
        }
    }

    private void GetLocaleFromDevice() {
        _currentLocale = LocalizationManager.SystemLanguageToString(Application.systemLanguage);
    }

    private string GenerateRandomName() {
        return "Unicorn"+_rand.Next().ToString();
    }


    public void ChangeLocale(string locale) {
        _currentLocale = locale;
        PlayerPrefs.SetString(localeKey, locale);
    }

    private void LoadLevelsUnlocked() {

    }

    private string GenerateKeyForLevelUnlocked(int id) {
        return levelUnlockedBaseKey + id.ToString();
    }
}
