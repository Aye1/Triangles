﻿using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : MonoBehaviour {


    private Dictionary<string, Dictionary<string, string>> _allLocalizedStrings;
    private readonly string _stringNotFound = "Unknown string";
    private static LocalizationManager _instance;

    public bool isReady = false;

    public static LocalizationManager Instance 
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
        _allLocalizedStrings = LoadDicoFromCSV();
        isReady = true;
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

    public string GetLocString(string key, SystemLanguage language) {
        return GetLocString(key, SystemLanguageToString(language));
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
                if (res.ContainsKey(keys[i]))
                {
                    Debug.LogWarning("Key " + keys[i] + " already loaded");
                }
                else
                {
                    Dictionary<string, string> newLocaleDic = new Dictionary<string, string>();
                    for (int j = 1; j < splitLines.Length; j++)
                    {
                        newLocaleDic.Add(splitLines[j][0], splitLines[j][i]);
                    }
                    res.Add(keys[i], newLocaleDic);
                }
            }
        }
        return res;
    }

    private string[] GetRawCSVLines() {
        string basePath = Application.dataPath;
        TextAsset resObj = Resources.Load<TextAsset>("Localizations");
        if(resObj != null) {
            return RawToLines(resObj.text);
        }  else {
            Debug.Log("Can't find localization file");
        }
        return new string[]{};
    }

    private string[] RawToLines(string rawText) {
        string[] separators = { "\n" };
        return rawText.Split(separators, System.StringSplitOptions.None);
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

    public static string SystemLanguageToString(SystemLanguage language)
    {
        switch (language)
        {
            case SystemLanguage.French:
                return Locales.fr_FR;
            case SystemLanguage.English:
                return Locales.en_GB;
        }
        return Locales.en_GB;
    }

    public static SystemLanguage StringToSystemLanguage(string language) {
        if(language == Locales.fr_FR) {
            return SystemLanguage.French;
        } 
        if (language == Locales.en_GB) {
            return SystemLanguage.English;
        }
        return SystemLanguage.English;
    }

}