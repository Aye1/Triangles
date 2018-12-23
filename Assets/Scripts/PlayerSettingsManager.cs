using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Random = System.Random;
using UnityEngine.Analytics;

public class PlayerSettingsManager : MonoBehaviour
{

    private static PlayerSettingsManager _instance;
    private string _currentLocale;
    private int _maxLevel;
    private int _currentLevel;
    private string _name;
    private int _questsPoints;
    private Random _rand;
    private int _highScore;
    private Dictionary<int, int> _levelsUnlocked;

    public static string localeKey = "locale";
    public static string levelKey = "level";
    public static string nameKey = "name";
    public static string questPointsKey = "questPoints";
    public static string highScoreKey = "highScore";
    public static string levelUnlockedBaseKey = "levelUnlocked";


    #region Properties
    public static PlayerSettingsManager Instance
    {
        get { return _instance; }
    }

    public string CurrentLocale
    {
        get { return _currentLocale; }
    }

    public int MaxLevel
    {
        get { return _maxLevel; }
    }

    public string Name
    {
        get { return _name; }
        set {
            if (value != _name){
                _name = value;
                PlayerPrefs.SetString(nameKey, _name);
            } 
        }
    }

    public int QuestsPoints
    {
        get { return _questsPoints; }
        set
        {
            if (value != _questsPoints)
            {
                _questsPoints = value;
                PlayerPrefs.SetInt(questPointsKey, _questsPoints);
                OnVariableChange(value);
            }
        }
    }

    public delegate void OnVariableChangeDelegate(int newVal);
    public event OnVariableChangeDelegate OnVariableChange;

    public int CurrentLevel
    {
        get
        {
            if (_currentLevel == -1)
            {
                return _maxLevel;
            }
            return _currentLevel;
        }
        set
        {
            if (value != _currentLevel)
            {
                _currentLevel = value;
            }
        }
    }

    public int HighScore
    {
        get
        {
            return _highScore;
        }
        set
        {
            if (value != _highScore)
            {
                _highScore = value;
                PlayerPrefs.SetInt(highScoreKey, _highScore);
            }
        }
    }

    public Dictionary<int, int> LevelsUnlocked
    {
        get
        {
            return _levelsUnlocked;
        }
    }
    #endregion

    // Use this for initialization
    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        _rand = new Random();
        DontDestroyOnLoad(gameObject);
        LoadPlayerSettings();
    }

    private void LoadPlayerSettings()
    {
        if (PlayerPrefs.HasKey(localeKey))
        {
            _currentLocale = PlayerPrefs.GetString(localeKey);
        }
        else
        {
            GetLocaleFromDevice();
        }

        if (PlayerPrefs.HasKey(levelKey))
        {
            _maxLevel = PlayerPrefs.GetInt(levelKey);
        }
        else
        {
            _maxLevel = 1;
            PlayerPrefs.SetInt(levelKey, _maxLevel);
        }

        if (PlayerPrefs.HasKey(nameKey))
        {
            _name = PlayerPrefs.GetString(nameKey);
        }
        else
        {
            _name = GenerateRandomName();
            PlayerPrefs.SetString(nameKey, _name);
        }

        if (PlayerPrefs.HasKey(questPointsKey))
        {
            _questsPoints = PlayerPrefs.GetInt(questPointsKey);
        }
        else
        {
            _questsPoints = 0;
            PlayerPrefs.SetInt(questPointsKey, 0);
        }

        if (PlayerPrefs.HasKey(highScoreKey))
        {
            _highScore = PlayerPrefs.GetInt(highScoreKey);
        }
        else
        {
            _highScore = 0;
            PlayerPrefs.SetInt(highScoreKey, 0);
        }
    }

    private void GetLocaleFromDevice()
    {
        _currentLocale = LocalizationManager.SystemLanguageToString(Application.systemLanguage);
    }

    private string GenerateRandomName()
    {
        return "Unicorn" + _rand.Next().ToString();
    }


    public void ChangeLocale(string locale)
    {
        _currentLocale = locale;
        PlayerPrefs.SetString(localeKey, locale);
    }

    public void LoadLevelsUnlocked()
    {
        _levelsUnlocked = new Dictionary<int, int>();
        for (int i = 0; i < LevelManager.Instance.levelCount; i++)
        {
            string key = GenerateKeyForLevelUnlocked(i);
            int value = 0;
            if (PlayerPrefs.HasKey(key))
            {
                value = PlayerPrefs.GetInt(key);
            }
            else
            {
                // Init player pref

                // First level is always unlocked
                if (i == 0)
                {
                    PlayerPrefs.SetInt(key, 1);
                }
                // Other levels start locked
                else
                {
                    PlayerPrefs.SetInt(key, 0);
                }
            }
            _levelsUnlocked.Add(i, value);
        }
    }

    private static string GenerateKeyForLevelUnlocked(int id)
    {
        return levelUnlockedBaseKey + id.ToString();
    }

    public bool IsLevelUnlocked(int index)
    {
        int value;
        _levelsUnlocked.TryGetValue(index, out value);
        return value == 1;
    }

    public void UnlockLevel(int index, int cost)
    {
        string key = GenerateKeyForLevelUnlocked(index);
        _levelsUnlocked[index] = 1;
        PlayerPrefs.SetInt(key, 1);
        QuestsPoints = QuestsPoints - cost;
        AnalyticsEvent.ItemAcquired(AcquisitionType.Soft, "levels", 1, key);
    }

#if UNITY_EDITOR
    [MenuItem("Debug/Reset levels unlocked")]
    public static void ResetLevelsUnlocked()
    {
        for (int i = 0; i < LevelManager.Instance.levelCount; i++)
        {
            string key = GenerateKeyForLevelUnlocked(i);
            PlayerPrefs.SetInt(key, 0);
            PlayerSettingsManager.Instance.LevelsUnlocked[i] = 0;
        }
    }

    [MenuItem("Debug/Reset quest points")]
    public static void ResetQuestPoints()
    {
        PlayerSettingsManager.Instance.QuestsPoints = 0;
    }
#endif

}
