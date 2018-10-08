using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LanguagePicker : MonoBehaviour {

    //public TextMeshProUGUI instructionsText;

	// Use this for initialization
	void Start () {
        BindButtons();
	}

    private void BindButtons() {
        foreach(LanguageButton b in GetComponentsInChildren<LanguageButton>()){
            b.onClick.AddListener(delegate { OnButtonClicked(b); });
        } 
    }

    private void OnButtonClicked(LanguageButton b){
        string newLocale = LocalizationManager.SystemLanguageToString(b.language);
        PlayerSettingsManager.Instance.ChangeLocale(newLocale);
        //instructionsText.GetComponent<TextMeshProLocalizator>().UpdateLocale();
        SceneController.Instance.ReloadCurrentScene();
    }
}
