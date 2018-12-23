using System;
using UnityEngine.UI;
using TMPro;
public class ShuffleButtonWithRewarded : ShuffleButton
{

    RewardedVideoManager _rewardedVideoManager;
    private bool _isWaitingForVideo = false;
    private readonly string noShuffleLocKey = "no_shuffle";
    private readonly string shuffleLocKey = "shuffle";
    private readonly string videoLocKey = "show_video";

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
        string locKey = noShuffleLocKey;
        if (base.IsInteractable())
        {
            locKey = shuffleLocKey;
        }
        else if (_rewardedVideoManager.IsVideoAvailable())
        {
            locKey = videoLocKey;
        }
        GetComponentInChildren<TextMeshProUGUI>().text = LocalizationManager.Instance.GetLocString(locKey, PlayerSettingsManager.Instance.CurrentLocale);
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
            _gameManager.ShuffleShapesInPopup(true);
        }
    }
}
