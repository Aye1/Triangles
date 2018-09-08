using System;
using UnityEngine.UI;

public class ShuffleButtonWithRewarded : ShuffleButton
{

    RewardedVideoManager _rewardedVideoManager;
    private bool _isWaitingForVideo = false;

    // Use this for initialization
    new void Start()
    {
        base.Start();
        _rewardedVideoManager = FindObjectOfType<RewardedVideoManager>();
        _rewardedVideoManager.OnVideoCompleted += OnVideoSuccessful;

    }

    override protected bool IsInteractable()
    {
        bool interactable = base.IsInteractable() || _rewardedVideoManager.IsVideoAvailable();
        return interactable;
    }

    override protected void ManageText()
    {
        if (base.IsInteractable())
        {
            GetComponentInChildren<Text>().text = "Shuffle";
        }
        else if (_rewardedVideoManager.IsVideoAvailable())
        {
            GetComponentInChildren<Text>().text = "Show video";
        }
        else
        {
            GetComponentInChildren<Text>().text = "No More Shuffle";
        }

    }

    override public void OnShuffleButtonClick()
    {
        if (_gameManager != null)
        {
            if (_gameManager.ShuffleCount > 0)
            {
                _gameManager.ShuffleShapesInPopup();
            }
            else if (_rewardedVideoManager.IsRewardedVideoAvailable)
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
            _gameManager.ShuffleCount++;
            _gameManager.ShuffleShapesInPopup();
        }
    }
}
