using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour {

    public List<Vector4> positionsXY;
    public Shape refShape;

    private bool isDragged = false;

	// Use this for initialization
	void Start () {
        GeneratePiece();
	}
	
	// Update is called once per frame
	void Update () {
        UpdatePosition();
	}

    void UpdatePosition()
    {
        if (isDragged)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePos.x, mousePos.y, -1);
        }
    }

    private void GeneratePiece()
    {
        foreach(Vector4 pos in positionsXY)
        {
            Shape newShape = Instantiate(refShape);
            newShape.transform.position = transform.position + new Vector3(pos.x / 2.0f, pos.y, pos.z);
            newShape.IsUpsideDown = pos.w == 1.0f;
            newShape.transform.parent = transform;

            newShape.BaseColor = Color.blue;
            newShape.ShapeClickedHandler += OnShapeClicked;
            newShape.ShapeReleasedHandler += OnShapeReleased;
        }
    }

    private void OnShapeClicked(object sender, EventArgs e)
    {
        isDragged = true;
    }

    private void OnShapeReleased(object sender, EventArgs e)
    {
        isDragged = false;
    }
}
