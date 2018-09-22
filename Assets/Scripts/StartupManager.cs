using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartupManager : MonoBehaviour {

    private StringLocalizationManager _locManager;
    private bool startupFinished = false;
    public bool debugStartupFinished = false;
    public bool locFinished;

	// Use this for initialization
	void Start () {
        StartCoroutine(WaitForAllServicesLaunched());
	}
	
	// Update is called once per frame
	void Update () {
        locFinished = _locManager.isReady;
        if (startupFinished){
            SceneController.Instance.GoToMenuScreen();
        }
	}

    private IEnumerator WaitForAllServicesLaunched()
    {
        GetServices();
        bool isEverythingLaunched = debugStartupFinished && _locManager.isReady;
        yield return new WaitUntil(() => isEverythingLaunched);
        startupFinished = true;
    }

    private void GetServices() {
        if(_locManager == null) {
            _locManager = FindObjectOfType<StringLocalizationManager>();
        }
    }
}
