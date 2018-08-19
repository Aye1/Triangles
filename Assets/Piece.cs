using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Piece : MonoBehaviour {

    public List<Vector4> positionsXY;
    public Shape refShape;
    public List<Shape> pieceShapes;

    private bool isDragged = false;
    public EventHandler PieceDraggedHandler;
    public EventHandler PieceReleasedHandler;

	// Use this for initialization
	void Awake () {
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
        pieceShapes = new List<Shape>();
        foreach (Vector4 pos in positionsXY)
        {
            Shape newShape = Instantiate(refShape);
            newShape.transform.position = transform.position + new Vector3(pos.x*Config.paddingX, -pos.y*Config.paddingY, pos.z);
            newShape.IsUpsideDown = pos.w == 1.0f;
            newShape.transform.parent = transform;

            newShape.BaseColor = Color.blue;
            newShape.ShapeClickedHandler += OnShapeClicked;
            newShape.ShapeReleasedHandler += OnShapeReleased;
            newShape.PosXY = pos;
            pieceShapes.Add(newShape);
        }
    }

    private void OnShapeClicked(object sender, EventArgs e)
    {
        isDragged = true;
        if (PieceDraggedHandler != null)
        {
            PieceDraggedHandler(this, new EventArgs());
        } 
    }

    private void OnShapeReleased(object sender, EventArgs e)
    {
        isDragged = false;
        if (PieceReleasedHandler != null)
        {
            PieceReleasedHandler(this, new EventArgs());
        }
    }

    public void DestroyPiece()
    {
        foreach(Shape s in pieceShapes)
        {
            Destroy(s.gameObject);
        }
        Destroy(this.gameObject);
    }
}
