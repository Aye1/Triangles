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
        SceneManager.LoadScene(2);
    }

    public void GoToMenuScreen() {
        SceneManager.LoadScene(1);
    }
}
