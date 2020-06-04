using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextMeshProLocalizator : MonoBehaviour {

    public string localizationKey;

	// Use this for initialization
	void Start () {
        UpdateLocale();
	}

    public void UpdateLocale() {
        GetComponent<TextMeshProUGUI>().text = LocalizationManager.Instance.GetLocString(localizationKey);
    }
}
