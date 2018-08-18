using System;
using UnityEngine;

public class Shape : MonoBehaviour
{ 
    Vector3Int position;
    private SpriteRenderer _spriteRenderer;
    bool isSelected = false;
    bool isUpsideDown = false;
    Color baseColor = Color.white;
    Color selectedColor = Color.red;

    public EventHandler ShapeClickedHandler;
    public EventHandler ShapeReleasedHandler;

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

    #region Properties
    public bool IsSelected
    {
        get
        {
            return isSelected;
        }

        set
        {
            isSelected = value;
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
    #endregion

    // Use this for initialization
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsSelected)
        {
            ChangeColor(selectedColor);
        }
        else
        {
            ChangeColor(BaseColor);
        }
    }

    void OnMouseDown()
    {
        IsSelected = !IsSelected;

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
