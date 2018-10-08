using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusView : MonoBehaviour {

    public GameObject emptyPart;
    public bool isOut = false;
    private Vector3 _translation = new Vector3(0.0f, 100.0f, 0.0f);
    private Vector3 _targetPosition;
    private Vector3 _upPosition;
    private Vector3 _downPosition;
    private float _moveSpeed = 200.0f;
    public GameObject bonusViewButton;

	// Use this for initialization
	void Start () {
        emptyPart.SetActive(false);
        _upPosition = transform.position + _translation;
        _downPosition = transform.position;
        _targetPosition = _downPosition;
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _moveSpeed * Time.deltaTime);
	}

    public void DisplayView() {
        if (!isOut)
        {
            UIHelper.HideGameObject(bonusViewButton);
            Vector3 pos = transform.position;
            _targetPosition = _upPosition;
            emptyPart.SetActive(true);
            isOut = true;
        }
    }

    public void HideView() {
        if (isOut)
        {
            Vector3 pos = transform.position;
            _targetPosition = _downPosition;
            emptyPart.SetActive(false);
            isOut = false;
            UIHelper.DisplayGameObject(bonusViewButton);
        }
    }
}
