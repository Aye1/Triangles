using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class NewGameButton : MonoBehaviour {

    private Button _thisButton;

	// Use this for initialization
	void Start () {
        _thisButton = GetComponent<Button>();
        _thisButton.onClick.AddListener(OnButtonClicked);
        _thisButton.GetComponentInChildren<Text>().text = StringLocalizationManager.Instance.GetLocString("test", Locales.en_GB);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnButtonClicked() {
        SceneController.Instance.GoToGameScreen();
    }
}
