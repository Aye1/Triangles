using System;
using UnityEngine;

[Serializable]
public class Shape : MonoBehaviour
{ 
    Vector3Int position;
    Vector2 posXY;
    private SpriteRenderer _spriteRenderer;
    bool isUpsideDown = false;
    Color baseColor = new Color(0.87f, 0.87f, 0.90f);
    Color selectedColor = Color.red;
    Color hoveredColor;
    Color filledColor;
    public bool isFilled = false;
    private bool isVisuallyFilled = false;
    private bool isHighlighted = false;
    public bool isPlayable = false;
    public bool tmpBool = false;
    public SpriteRenderer halo;

    public EventHandler ShapeClickedHandler;
    public EventHandler ShapeReleasedHandler;
    public EventHandler ShapeCollidingHandler;
    public EventHandler ShapeExitCollisionHandler;

    #region Properties
    public Vector3Int PositionABC
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

    public bool IsUpsideDown
    {
        get
        {
            return isUpsideDown;
        }

        set
        {
            if (isUpsideDown != value)
            {
                isUpsideDown = value;
                transform.Rotate(0.0f, 0.0f, 180.0f);
            }
        }
    }

    public Color BaseColor
    {
        get
        {
            return baseColor;
        }

        set
        {
            if (baseColor != value)
            {
                baseColor = value;
            }
        }
    }

    public Vector2 PosXY
    {
        get
        {
            return posXY;
        }

        set
        {
            posXY = value;
        }
    }

    public Color FilledColor
    {
        get
        {
            return filledColor;
        }

        set
        {
            filledColor = value;
        }
    }

    public bool IsVisuallyFilled
    {
        get
        {
            return isVisuallyFilled;
        }

        set
        {
            isVisuallyFilled = value;
        }
    }

    public bool IsHighlighted
    {
        get
        {
            return isHighlighted;
        }

        set
        {
            isHighlighted = value;
        }
    }

    public Color HoveredColor
    {
        get
        {
            return hoveredColor;
        }

        set
        {
            hoveredColor = value;
        }
    }
    #endregion

    // Use this for initialization
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        ChangeColor(BaseColor);
        if (isPlayable)
        {
            Color alphaColor = HoveredColor;
            alphaColor.a = 0.7f;
            ChangeColor(alphaColor);
        }
        else if (IsVisuallyFilled)
        {
            ChangeColor(FilledColor);
        }
        halo.enabled = IsHighlighted;
        tmpBool = false;
        isPlayable = false;
    }

    void OnMouseDown()
    {
        if (ShapeClickedHandler != null)
        {
            ShapeClickedHandler(this, new EventArgs());
        }
    }

    private void OnMouseUp()
    {
        if (ShapeReleasedHandler != null)
        {
            ShapeReleasedHandler(this, new EventArgs());
        }
    }

    private void ChangeColor(Color color)
    {
        _spriteRenderer.color = color;
    }

    public class CollisionEventArgs : EventArgs {
        public GameObject OtherObject { get; set; }
        public Piece CurrentPiece { get; set; }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(ShapeCollidingHandler != null) {
            CollisionEventArgs args = new CollisionEventArgs();
            args.OtherObject = collision.gameObject;
            ShapeCollidingHandler(this, args);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(ShapeExitCollisionHandler != null) {
            CollisionEventArgs args = new CollisionEventArgs();
            args.OtherObject = collision.gameObject;
            ShapeExitCollisionHandler(this, args);
        }
    }

}
