using System;
using UnityEngine;

public class Shape : MonoBehaviour
{ 
    Vector3Int position;
    Vector2 posXY;
    private SpriteRenderer _spriteRenderer;
    bool isUpsideDown = false;
    Color baseColor = Color.white;
    Color selectedColor = Color.red;
    public bool isFilled = false;
    public bool isPlayable = false;
    public bool tmpBool = false;

    public EventHandler ShapeClickedHandler;
    public EventHandler ShapeReleasedHandler;

    #region Properties
    public Vector3Int Position
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
            baseColor = value;
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
    #endregion

    // Use this for initialization
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        ChangeColor(Color.white);
        if (isPlayable)
        {
            ChangeColor(Color.green);
        }
        /*if (tmpBool)
        {
            ChangeColor(Color.cyan);
        } */
        if (isFilled)
        {
            ChangeColor(Color.red);
        }
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
}
