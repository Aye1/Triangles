using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ShuffleButton : MonoBehaviour {

    RewardedVideoManager _rewardedVideoManager;
    GameManager _gameManager;
    Button _thisButton;

    private bool _isWaitingForVideo = false;

    // Use this for initialization
    void Start () {
        _rewardedVideoManager = FindObjectOfType<RewardedVideoManager>();
        _gameManager = FindObjectOfType<GameManager>();
        _thisButton = GetComponent<Button>();
        _thisButton.onClick.AddListener(OnShuffleButtonClick);
        _rewardedVideoManager.OnVideoCompleted += OnVideoSuccessful;
    }

    // Update is called once per frame
    void Update () {
        ManageInteractable();
	}

    private void ManageInteractable()
    {
        _thisButton.interactable = (_gameManager != null && _gameManager.ShuffleCount > 0) || _rewardedVideoManager.IsRewardedVideoAvailable;
    }

    public void OnShuffleButtonClick()
    {
        if(_gameManager != null)
        {
            if (_gameManager.ShuffleCount > 0)
            {
                _gameManager.ShuffleShapesInPopup();
                _gameManager.ShuffleCount--;
            } else if (_rewardedVideoManager.IsRewardedVideoAvailable)
            {
                _isWaitingForVideo = true;
                _rewardedVideoManager.DisplayRewardedVideo();
            }
        }
    }

    private void OnVideoSuccessful(object sender, EventArgs e)
    {
        if (_isWaitingForVideo)
        {
            _isWaitingForVideo = false;
            _gameManager.ShuffleShapesInPopup();
        }
    }
}
