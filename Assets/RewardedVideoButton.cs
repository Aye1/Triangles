
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class RewardedVideoButton : MonoBehaviour {

    RewardedVideoManager _rewardedVideoManager;
    Button _thisButton;
    public string noMoreShufflesText = "No More Shuffle available";


// Use this for initialization
void Start () {
        _rewardedVideoManager = RewardedVideoManager.Instance;
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
        ManageInteractability();
    }

    private void ManageInteractability() {
        bool isInteractable = _rewardedVideoManager != null && _rewardedVideoManager.IsVideoAvailable();
        _thisButton.interactable = isInteractable;
        if (!isInteractable)
        {
            GetComponentInChildren<Text>().text = noMoreShufflesText;
        }
    }
}
