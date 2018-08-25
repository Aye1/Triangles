using System;
using UnityEngine;

public class Shape : MonoBehaviour
{ 
    Vector3Int position;
    Vector2 posXY;
    private SpriteRenderer _spriteRenderer;
    bool isUpsideDown = false;
    Color baseColor = new Color(0.87f, 0.87f, 0.90f);
    Color selectedColor = Color.red;
    Color filledColor;
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
            Color alphaColor = filledColor;
            alphaColor.a = 0.7f;
            ChangeColor(alphaColor);
        }
        if (tmpBool)
        {
            ChangeColor(Color.cyan);
            Debug.Log("Tmp position " + transform.position);
        } 
        if (isFilled)
        {
            ChangeColor(FilledColor);
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
