using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : MonoBehaviour {

    Vector3 position;
    private SpriteRenderer _spriteRenderer;

    public Vector3 Position
    {
        get
        {
            return position;
        }

        set
        {
            position = value;
        }
    }

    // Use this for initialization
    void Start() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update() {
        ChangeColor();
	}

    private void ChangeColor()
    {
        _spriteRenderer.color = new Color(Mathf.Abs(Position.x)/10.0f, Mathf.Abs(Position.y) / 10.0f, Mathf.Abs(Position.z)/10.0f);
    }
}
