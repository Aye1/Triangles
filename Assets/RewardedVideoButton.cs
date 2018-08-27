
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class RewardedVideoButton : MonoBehaviour {

    RewardedVideoManager _rewardedVideoManager;
    Button _thisButton;

	// Use this for initialization
	void Start () {
        _rewardedVideoManager = FindObjectOfType<RewardedVideoManager>();
        _thisButton = GetComponent<Button>();
        _thisButton.interactable = false;
        BindAction();
	}

    private void BindAction()
    {
        _thisButton.onClick.AddListener(_rewardedVideoManager.DisplayRewardedVideo);
    }
	
	// Update is called once per frame
	void Update () {
		if(_rewardedVideoManager != null)
        {
            _thisButton.interactable = _rewardedVideoManager.IsRewardedVideoAvailable;
        } 
	}
}
