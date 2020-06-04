using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LocalizationManager : MonoBehaviour
{
    private Dictionary<string, Dictionary<string, string>> _allLocalizedStrings;
    private readonly string _stringNotFound = "Unknown string";

    public bool isReady = false;
    public SystemLanguage locale;

    public static LocalizationManager Instance { get; private set; }

    public Dictionary<string, Dictionary<string, string>> AllLocalizedStrings {
        get {
            return _allLocalizedStrings;
        }
    }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
        LoadLocalizedStrings();
    }

    private void LoadLocalizedStrings()
    {
        _allLocalizedStrings = LoadDicoFromCSV();
        isReady = true;
    }

    public string GetLocString(string key)
    {
        if (!isReady)
        {
            return null;
        }
        string localeString = SystemLanguageToString(locale);
        if (_allLocalizedStrings.ContainsKey(localeString))
        {
            if (_allLocalizedStrings[localeString].ContainsKey(key))
            {
                return _allLocalizedStrings[localeString][key];
            }
        }
        return _stringNotFound;
    }

    private Dictionary<string, Dictionary<string, string>> LoadDicoFromCSV()
    {
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

    private string[] GetRawCSVLines()
    {
        string basePath = Application.dataPath;
        TextAsset resObj = Resources.Load<TextAsset>("Localizations");
        if (resObj != null)
        {
            return RawToLines(resObj.text);
        }
        else
        {
            Debug.Log("Can't find localization file");
        }
        return new string[] { };
    }

    private string[] RawToLines(string rawText)
    {
        string[] separators = { "\n" };
        return rawText.Split(separators, System.StringSplitOptions.None);
    }

    private string[][] RawToSplitLines(string[] rawLines)
    {
        char separator = ';';
        string[][] splitLines = new string[rawLines.Length][];

        for (int i = 0; i < rawLines.Length; i++)
        {
            splitLines[i] = rawLines[i].Split(separator);
        }
        return splitLines;
    }

    private string[][] CleanedLines(string[][] splitLines)
    {
        List<string[]> listLines = new List<string[]>();
        foreach (string[] splitLine in splitLines)
        {
            if (splitLine[0] != "")
            {
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

    public static SystemLanguage StringToSystemLanguage(string language)
    {
        if (language == Locales.fr_FR)
        {
            return SystemLanguage.French;
        }
        if (language == Locales.en_GB)
        {
            return SystemLanguage.English;
        }
        return SystemLanguage.English;
    }

#if UNITY_EDITOR
    [MenuItem("Debug/Dump available localizations")]
    public static void DisplayAvailableLoc() {

        foreach(Dictionary<string, string> locale in LocalizationManager.Instance.AllLocalizedStrings.Values) {
            foreach(string value in locale.Values) {
                Debug.Log(value);
            }
        }
    }
#endif

}
