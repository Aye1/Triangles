using System.Collections;
using System.IO;
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
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
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
        /*_allLocalizedStrings = new Dictionary<string, Dictionary<string, string>>();
        foreach(string locale in GetLocalesList()) {
            _allLocalizedStrings.Add(locale, LoadOneLocale(locale));
        }*/
        _allLocalizedStrings = LoadDicoFromCSV();
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
        if(!isReady) {
            return null;
        }
        if(_allLocalizedStrings.ContainsKey(locale)) {
            if(_allLocalizedStrings[locale].ContainsKey(key)) {
                return _allLocalizedStrings[locale][key];
            }
        }
        return _stringNotFound;
    }

    private Dictionary<string, Dictionary<string, string>> LoadDicoFromCSV() {
        Dictionary<string, Dictionary<string, string>> res = new Dictionary<string, Dictionary<string, string>>();
        string[] rawLines = GetRawCSVLines();
        string[][] splitLines = CleanedLines(RawToSplitLines(rawLines));
        string[] keys = splitLines[0];
        for (int i = 1; i < keys.Length; i++)
        {
            if (keys[i] != "")
            {
                Dictionary<string, string> newLocaleDic = new Dictionary<string, string>();
                for (int j = 1; j < splitLines.Length;j++) {
                    newLocaleDic.Add(splitLines[j][0], splitLines[j][i]);
                }
                res.Add(keys[i], newLocaleDic);
            }
        }
        return res;
    }

    private string[] GetRawCSVLines() {
        string basePath = Application.dataPath;
        string completePath = basePath + "/Localization/Resources/Localizations.csv";
        if (File.Exists(completePath))
        {
            string[] rawText = File.ReadAllLines(completePath);
            return rawText;
        }
        return new string[]{};
    }

    private string[][] RawToSplitLines(string[] rawLines) {
        char separator = ';';
        string[][] splitLines = new string[rawLines.Length][];

        for (int i = 0; i < rawLines.Length; i++) {
            splitLines[i] = rawLines[i].Split(separator);
        }
        return splitLines;
    }

    private string[][] CleanedLines(string[][]splitLines) {
        List<string[]> listLines = new List<string[]>();
        foreach (string[] splitLine in splitLines) {
            if(splitLine[0] != "") {
                listLines.Add(splitLine);
            }
        }
        return listLines.ToArray();
    }
    
}
