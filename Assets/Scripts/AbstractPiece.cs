using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
abstract public class AbstractPiece : MonoBehaviour {
    public List<Vector4> positionsXY;
    public Shape refShape;
    public List<Shape> pieceShapes;
    public Shape firstShape;
    public int poolIndex;

    public float dragPositionOffset = 1.5f;
    //public float dragPositionOffset = 0.0f;
    public bool debugPrintPosition;

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
            Vector3 mousePos = Input.mousePosition;
            Vector3 shapeOffset = new Vector3(-firstShape.PosXY.x * Config.paddingX, firstShape.PosXY.y * Config.paddingY, -2.0f);
            Vector3 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(newPos.x, newPos.y + dragPositionOffset, -1) + shapeOffset;
            if(debugPrintPosition) {
                Debug.Log("Piece position: " + transform.position);
            }
        }
    }

    private void GeneratePiece()
    {
        pieceShapes = new List<Shape>();
        float ratio = FindObjectOfType<Board>().CurrentLevel.Ratio;
        foreach (Vector4 pos in positionsXY)
        {
            Vector3 position = transform.position + new Vector3(pos.x * Config.paddingX*ratio, -pos.y * Config.paddingY*ratio, pos.z);
            Shape newShape = Instantiate(refShape, position, Quaternion.identity);

            newShape.IsUpsideDown = pos.w == 1.0f;
            newShape.transform.parent = transform;
            newShape.transform.localScale = Vector3.Scale(newShape.transform.localScale, new Vector3(ratio, ratio, 1.0f));

            newShape.BaseColor = Color.blue;
            RegisterCallback(newShape);
            newShape.PosXY = pos;
            newShape.gameObject.layer = Constants.pieceLayerId;
            pieceShapes.Add(newShape);
        }
        firstShape = pieceShapes.ToArray()[0];
        firstShape.debugIsFirstShape = true;
        this.gameObject.layer = Constants.pieceLayerId;
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
            ((Shape.CollisionEventArgs)e).CurrentPiece = GetComponent<AbstractPiece>();
            PieceCollidingHandler(this, e);
        }
    }

    protected void OnShapeExitColliding(object sender, EventArgs e) {
        Shape movedShape = (Shape)sender;
        // We only want the first shape to collide
        if (PieceExitCollisionHandler != null && movedShape == firstShape)
        {
            ((Shape.CollisionEventArgs)e).CurrentPiece = GetComponent<AbstractPiece>();
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
