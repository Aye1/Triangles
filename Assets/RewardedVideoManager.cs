using System;
using UnityEngine;
using UnityEngine.Advertisements;


public class RewardedVideoManager : MonoBehaviour {

    private bool _isRewardedVideoAvailable = false;
    private uint _numberRewardedVideo = 0;
    public static uint NumberMaxRewardedVideoInGame = 1;

    public EventHandler OnVideoCompleted;

    public bool isTestMode;

    public bool IsRewardedVideoAvailable
    {
        get
        {
            return _isRewardedVideoAvailable;
        }

        set
        {
            if (_isRewardedVideoAvailable != value)
            {
                _isRewardedVideoAvailable = value;
                if (_isRewardedVideoAvailable)
                {
                    Debug.Log("Rewarded video available");
                }
            }
        }
    }

    // Use this for initialization
    void Start () {
        DontDestroyOnLoad(this);
	}
	
	// Update is called once per frame
	void Update () {
        IsRewardedVideoAvailable = Advertisement.IsReady();
	}

    public void DisplayRewardedVideo()
    {
        if (_numberRewardedVideo <= NumberMaxRewardedVideoInGame)
        {
            ShowOptions options = new ShowOptions();
            options.resultCallback = HandleDisplayResult;
            Advertisement.Show("rewardedVideo", options);
            _numberRewardedVideo++;
        }
    }

    public bool IsVideoAvailable()
    {
        return _isRewardedVideoAvailable && (_numberRewardedVideo <= NumberMaxRewardedVideoInGame);
    }

    private void HandleDisplayResult(ShowResult result)
    {
        switch(result)
        {
            case ShowResult.Finished:
                Debug.Log("Video completed");
                if (OnVideoCompleted != null)
                {
                    OnVideoCompleted(this, new EventArgs());
                }
                break;
            case ShowResult.Skipped:
                Debug.Log("Video skipped");
                break;
            case ShowResult.Failed:
                Debug.Log("Error during video");
                break;
        }
    }
}
