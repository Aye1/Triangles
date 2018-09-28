using System.Collections;
using System.Threading;
using UnityEngine;

public class StartupManager : MonoBehaviour {

    private StringLocalizationManager _locManager;
    private bool startupFinished = false;
    public bool debugStartupFinished = true;
    public bool locFinished;
    private bool _forceMenuLoading;

	// Use this for initialization
	void Start () {
        Timer _timeoutTimer = new Timer(OnTimeoutTimerFinished, null, 5000, 0);
        StartCoroutine(WaitForAllServicesLaunched());
	}
	
	// Update is called once per frame
	void Update () {
        locFinished = _locManager.isReady;
        if (startupFinished || _forceMenuLoading){
            StopAllCoroutines();
            SceneController.Instance.GoToMenuScreen();
        }
	}

    private IEnumerator WaitForAllServicesLaunched()
    {
        GetServices();
        bool isEverythingLaunched = debugStartupFinished && _locManager.isReady;
        while(!isEverythingLaunched) {
            isEverythingLaunched = debugStartupFinished && _locManager.isReady;
            yield return null;
        }
        //yield return new WaitUntil(() => isEverythingLaunched);
        startupFinished = true;
    }

    private void GetServices() {
        if(_locManager == null) {
            _locManager = FindObjectOfType<StringLocalizationManager>();
        }
    }

    private void OnTimeoutTimerFinished(object state) {
        Debug.Log("Startup took too long, forced skipping");
        Debug.Log("Localization manager status: " + _locManager.isReady);
        Debug.Log("Debug variable status: " + debugStartupFinished);
        _forceMenuLoading = true;
    }
}
