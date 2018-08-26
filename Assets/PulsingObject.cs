using UnityEngine;

public class PulsingObject : MonoBehaviour {

    public float scalingSpeed = 0.01f;
    public float maxScale = 1.2f;
    public float minScale = 0.8f;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        transform.localScale += new Vector3(scalingSpeed*Time.deltaTime, scalingSpeed*Time.deltaTime, 0.0f);
        if (transform.localScale.x >= maxScale || transform.localScale.x <= minScale)
        {
            scalingSpeed *= -1;
        }
	}
}
