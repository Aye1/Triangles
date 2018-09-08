using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ShuffleButton : MonoBehaviour {

   protected GameManager _gameManager;
    protected Button _thisButton;


    // Use this for initialization
   protected void Start () {
        _gameManager = FindObjectOfType<GameManager>();
        _thisButton = GetComponent<Button>();
        _thisButton.onClick.AddListener(OnShuffleButtonClick);
    }

    // Update is called once per frame
    void Update () {
        _thisButton.interactable = IsInteractable();
        ManageText();
	}

    virtual protected bool IsInteractable()
    {
        bool interactable = (_gameManager != null && _gameManager.ShuffleCount > 0);
        return interactable;
    }

    virtual public void OnShuffleButtonClick()
    {
        if(_gameManager != null)
        {
            if (_gameManager.ShuffleCount > 0)
            {
                _gameManager.ShuffleShapesInPopup();
            } 
        }
    }

    virtual protected void ManageText()
    {
    }
    
}
