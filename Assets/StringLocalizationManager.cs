using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringLocalizationManager : MonoBehaviour {


    private Dictionary<string, Dictionary<string, string>> _allLocalizedStrings;
    private readonly string _stringNotFound = "Unknown string";
    private static StringLocalizationManager _instance;

    public bool isReady = false;

    public static StringLocalizationManager Instance 
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if(_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
        DontDestroyOnLoad(gameObject);
        LoadLocalizedStrings();
    }

    private IEnumerable<string> GetLocalesList() 
    {
        List<string> locales = new List<string>();
        locales.Add(Locales.en_GB);
        locales.Add(Locales.fr_FR);
        return locales;
    }

    private void LoadLocalizedStrings() {
        _allLocalizedStrings = new Dictionary<string, Dictionary<string, string>>();
        foreach(string locale in GetLocalesList()) {
            _allLocalizedStrings.Add(locale, LoadOneLocale(locale));
        }
        isReady = true;
    }

    private Dictionary<string, string> LoadOneLocale(string locale) {
        Dictionary<string, string> currentLocaleDico = new Dictionary<string, string>();
        if (Locales.fr_FR.Equals(locale))
        {
            currentLocaleDico.Add("test", "Test Français");
        } else if (Locales.en_GB.Equals(locale)) {
            currentLocaleDico.Add("test", "English Test");
        }
        return currentLocaleDico;
    }

    public string GetLocString(string key, string locale) 
    {
        if(_allLocalizedStrings.ContainsKey(locale)) {
            if(_allLocalizedStrings[locale].ContainsKey(key)) {
                return _allLocalizedStrings[locale][key];
            }
        }
        return _stringNotFound;
    }

    private void ReadDataFromCSV() {

    }
}
