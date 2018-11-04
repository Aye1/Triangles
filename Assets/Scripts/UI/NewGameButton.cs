using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class NewGameButton : MonoBehaviour {

    private Button _thisButton;
    private GameManager _gameManager;

	// Use this for initialization
	void Start () {
        _thisButton = GetComponent<Button>();
        _thisButton.onClick.AddListener(OnButtonClicked);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnButtonClicked() {
        SceneController.Instance.GoToGameScreen(PlayerSettingsManager.Instance.CurrentLevel);
        /*_gameManager = FindObjectOfType<GameManager>();
        if(_gameManager != null) {
            _gameManager.Restart();
        }*/
    }
}
