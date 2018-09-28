using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSettingsManager : MonoBehaviour {

    private static PlayerSettingsManager _instance;
    private string _currentLocale;

    public static string localeKey = "locale";

    public static PlayerSettingsManager Instance {
        get { return _instance; }
    }

    public string CurrentLocale {
        get { return _currentLocale; }
    }

    // Use this for initialization
    void Awake () {
        if(_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
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
    }

    private void GetLocaleFromDevice() {
        _currentLocale = LocalizationManager.SystemLanguageToString(Application.systemLanguage);
    }


    public void ChangeLocale(string locale) {
        _currentLocale = locale;
        PlayerPrefs.SetString(localeKey, locale);
    }
}
