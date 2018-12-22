using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public abstract class Level : MonoBehaviour {

    protected List<Shape> _shapes;
    private float _width;
    private float _height;
    private float _ratio;
    protected int _visualWidth;
    protected int _visualHeight;
    private readonly float _layoutSizeRatio = 0.9f;
    public Shape basicShape;

    public List<Shape> Shapes {
        get { return _shapes; }
    }

    public float Ratio {
        get { return _ratio; }
    }

    /// <summary>
    /// Method generating the board, depends on each level
    /// </summary>
    public void GenerateBoard() 
    {
        _shapes = new List<Shape>();
        GenerateShapes();
    }

    public abstract void GenerateShapes();

    /// <summary>
    /// Creates the board shape.
    /// </summary>
    protected void CreateBoardShape(int i, int j)
    {
        Vector3 pos = new Vector3(i * Config.paddingX * _ratio,
                    -j * Config.paddingY * _ratio,
                    0);
        Vector3 finalPos = Vector3.Scale(pos, transform.lossyScale);
        Shape newShape = Instantiate(basicShape, finalPos, Quaternion.identity);
        newShape.gameObject.tag = Constants.boardTag;
        newShape.gameObject.layer = Constants.boardLayerId;
        newShape.PositionABC = PosABCfromIJ(i, j);
        newShape.PosXY = new Vector2(i, j);
        newShape.IsUpsideDown = (i + j) % 2 ==0;
        newShape.transform.localScale = Vector3.Scale(transform.lossyScale, newShape.transform.localScale);
        newShape.transform.localScale = Vector3.Scale(newShape.transform.localScale, new Vector3(_ratio, _ratio, 1.0f));
        newShape.transform.parent = transform;
        _shapes.Add(newShape);
    }

    protected Vector3Int PosABCfromIJ(int i, int j)
    {
        int A = j;
        int B = (int)Mathf.Ceil((-i - j) / 2.0f);
        int C = (int)Mathf.Floor(-(i - j) / 2.0f);
        return new Vector3Int(A, B, C);
    }

    protected void ComputeBoardDimensions()
    {
        _width = Config.paddingX * (_visualWidth + 1);
        _height = Config.paddingY * (_visualHeight + 1);
        ComputeShapeRatio();
    }

    /// <summary>
    /// Computes the ratio to be used to create the shapes, scaling with the screen size
    /// </summary>
    protected void ComputeShapeRatio() {
        Vector3 camPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0.0f));
        Vector3 camOrigin = Camera.main.ScreenToWorldPoint(Vector3.zero);
        Vector3 diff = camPos - camOrigin;

        if (diff.x < diff.y)
        {
            // Portrait
            _ratio = diff.x / _width;
        }
        else
        {
            // Landscape
            _ratio = diff.y / _height;
        }
        _ratio *= _layoutSizeRatio;
    }

    protected void AdjustBoardPosition()
    {
        transform.position = transform.position - new Vector3(_width * 0.5f * _ratio, -_height * 0.5f * _ratio, 0.0f);
    }
}
