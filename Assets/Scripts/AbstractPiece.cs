using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
abstract public class AbstractPiece : MonoBehaviour {
    public List<Vector4> positionsXY;
    public Shape refShape;
    public List<Shape> pieceShapes;
    public Shape firstShape;

    //public float dragPositionOffset = 1.5f;
    public float dragPositionOffset = 0.0f;

    public EventHandler PieceDraggedHandler;
    public EventHandler PieceReleasedHandler;
    public EventHandler PieceCollidingHandler;
    public EventHandler PieceExitCollisionHandler;

    protected Color pieceColor;
    protected bool isDragged = false;
    protected bool _isHoverPiece = false;

    public Color PieceColor
    {
        get
        {
            return pieceColor;
        }

        set
        {
            if (pieceColor != value)
            {
                pieceColor = value;
                if(IsHoverPiece) {
                    pieceColor.a = 0.5f;
                }
                SetShapesColor();
            }
        }
    }

    public bool IsHoverPiece {
        get {
            return _isHoverPiece;
        }
        set {
            _isHoverPiece = value;
        }
    }

    // Use this for initialization
    void Awake()
    {
        GeneratePiece();
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePosition();
    }

    void UpdatePosition()
    {
        if (isDragged)
        {
            Vector3 shapeSize = firstShape.GetComponent<Renderer>().bounds.size;
            Vector3 pos = new Vector3(firstShape.PosXY.x * shapeSize.x, firstShape.PosXY.y * shapeSize.y, 0.0f) + Input.mousePosition;
            Vector3 newPos = Camera.main.ScreenToWorldPoint(pos);
            transform.position = new Vector3(newPos.x, newPos.y + dragPositionOffset, -1);
        }
    }

    private void GeneratePiece()
    {
        pieceShapes = new List<Shape>();
        foreach (Vector4 pos in positionsXY)
        {
            Shape newShape = Instantiate(refShape);
            newShape.transform.position = transform.position + new Vector3(pos.x * Config.paddingX, -pos.y * Config.paddingY, pos.z);
            newShape.IsUpsideDown = pos.w == 1.0f;
            newShape.transform.parent = transform;

            newShape.BaseColor = Color.blue;
            RegisterCallback(newShape);
            newShape.PosXY = pos;
            pieceShapes.Add(newShape);
        }
        firstShape = pieceShapes.ToArray()[0];
    }


    private void RegisterCallback(Shape shape)
    {
        shape.ShapeClickedHandler += OnShapeClicked;
        shape.ShapeReleasedHandler += OnShapeReleased;
        shape.ShapeCollidingHandler += OnShapeColliding;
        shape.ShapeExitCollisionHandler += OnShapeExitColliding;
    }

    private void OnShapeClicked(object sender, EventArgs e)
    {
        isDragged = true;
        if (PieceDraggedHandler != null)
        {
            PieceDraggedHandler(this, new EventArgs());
        }
    }

    protected void OnShapeReleased(object sender, EventArgs e)
    {
        isDragged = false;
        if (PieceReleasedHandler != null)
        {
            PieceReleasedHandler(this, new EventArgs());
        }
    }

    protected void OnShapeColliding(object sender, EventArgs e) {
        if (PieceCollidingHandler != null && (Shape)sender == firstShape) {
            ((Shape.CollisionEventArgs)e).CurrentPiece = GetComponent<Piece>();
            PieceCollidingHandler(this, e);
        }
    }

    protected void OnShapeExitColliding(object sender, EventArgs e) {
        if (PieceExitCollisionHandler != null && (Shape)sender == firstShape)
        {
            ((Shape.CollisionEventArgs)e).CurrentPiece = GetComponent<Piece>();
            PieceExitCollisionHandler(this, e);
        }
    }

    public void DestroyPiece()
    {
        foreach (Shape s in pieceShapes)
        {
            Destroy(s.gameObject);
        }
        Destroy(this.gameObject);
    }

    private void SetShapesColor()
    {
        foreach (Shape s in pieceShapes)
        {
            s.BaseColor = pieceColor;
        }
    }

    public void Highlight(bool highlight)
    {
        foreach (Shape s in pieceShapes)
        {
            s.IsHighlighted = highlight;
        }
    }
}
