using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour {

    private static SceneController _instance;

    public static SceneController Instance {
        get { return _instance; }
    }

    void Awake()
    {
        if (_instance != null && _instance != this) 
        {
            Destroy(this.gameObject);
        } else 
        {
            _instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    
    public void GoToGameScreen()
    {
        GoToGameScreen(1);
    }

    public void GoToGameScreen(int level) {
        SceneManager.LoadScene(2);
        PlayerSettingsManager.Instance.CurrentLevel = level;
    }

    public void GoToMenuScreen() {
        SceneManager.LoadScene(1);
    }

    public void GoToLevelsScreen() {
        SceneManager.LoadScene(3);
    }

    public void ReloadCurrentScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
