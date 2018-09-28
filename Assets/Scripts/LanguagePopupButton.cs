using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LanguagePopupButton : MonoBehaviour {

    public Popup languagePopup;

    private void Start()
    {
        //GetComponent<Button>().onClick.AddListener(OnButtonClicked);
        Button _thisButton = GetComponent<Button>();
        _thisButton.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked() {
        UIHelper.DisplayGameObject(languagePopup.gameObject);
    }
}
