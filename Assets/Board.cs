using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{

    private readonly int minWidth = 9;
    public Shape basicShape;
    private List<Shape> shapes;
    private float sqr3 = Mathf.Sqrt(3);

    // Use this for initialization
    void Start()
    {
        shapes = new List<Shape>();
        CreateBoard();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnShapeClicked(object shapeSender, EventArgs e)
    {
        Shape shape = shapeSender as Shape;
        Debug.Log("Shape at pos " + shape.Position.ToString() + " clicked");
        List<Shape> validatedShapes = new List<Shape>();
        validatedShapes = CheckLine(validatedShapes, shape.Position.x, 0);
        validatedShapes = CheckLine(validatedShapes, shape.Position.y, 1);
        validatedShapes = CheckLine(validatedShapes, shape.Position.z, 2);
        FlipValidatedLines(validatedShapes);
    }

    private List<Shape> CheckLine(List<Shape> valShapes, int index, int pos)
    {
        IEnumerable<Shape> toValidate = shapes.Where<Shape>(shape => shape.Position[pos] == index);
        bool lineValidated = toValidate.All(shape => shape.IsSelected);
        if (lineValidated)
        {
            valShapes.AddRange(toValidate);
        }
        return valShapes;
    }

    private void FlipValidatedLines(List<Shape> valShapes)
    {
        valShapes.ForEach(shape => shape.IsSelected = false);
    }

    private void CreateBoard()
    {
        int height = minWidth - 1;
        int maxWidth = 2 * minWidth - 3;

        // Décalage de 1 pour rester sur la bonne parité de grille
        int offset = height / 2 % 2 == 0 ? 1 : 0;

        for (int j = 0; j < height; j++)
        {
            int currentWidth = maxWidth - (int)Mathf.Abs(minWidth / 2.0f - (j + 1)) * 2;
            int imin = (maxWidth - currentWidth) / 2 + offset;
            int imax = imin + currentWidth;

            for (int i = imin; i < imax; i++)
            {
                Shape newShape = Instantiate(basicShape);
                Vector2 vecB = new Vector2(sqr3 * i / 2, -j / 2);
                Vector2 vecC = new Vector2(-sqr3 * i / 2, -j / 2);
                int A = j;
                int B = (int)Mathf.Ceil((-i - j) / 2.0f);
                int C = (int)Mathf.Floor(-(i - j) / 2.0f);
                newShape.Position = new Vector3Int(A, B, C);
                newShape.PosXY = new Vector2(i, j);

                newShape.IsUpsideDown = ((i + j) % 2 == 0);

                newShape.transform.position = transform.position + new Vector3(i / 2.0f, -j * Mathf.Sqrt(2) / 2, newShape.transform.position.z);
                newShape.transform.parent = transform;
                newShape.ShapeClickedHandler += OnShapeClicked;
                shapes.Add(newShape);
            }
        }
    }

    public IEnumerable<Shape> GetPlayableShapes(Piece piece)
    {
        IEnumerable<Shape> unfilledShapes = new List<Shape>();
        unfilledShapes = shapes.Where(s => !s.isFilled);

        IEnumerable<Shape> resShapes = new List<Shape>();
        resShapes = unfilledShapes.Where(s => IsPiecePlayableOnShape(s, piece, unfilledShapes));
        ChangeShapesPlayableState(resShapes);
        return resShapes;
    }

    private bool IsPiecePlayableOnShape(Shape shape, Piece piece, IEnumerable<Shape> unfilledShapes)
    {
        List<Shape> necessaryShapes = new List<Shape>();
        foreach(Shape s in piece.pieceShapes)
        {
            Vector2 searchPos = s.PosXY + shape.PosXY;
            IEnumerable<Shape> found = unfilledShapes.Where(us => us.PosXY == searchPos 
                                                            && us.IsUpsideDown == s.IsUpsideDown);
            if(found.Count() == 0)
            {
                return false;
            }
            necessaryShapes.Add(found.First());
        }
        return true;
    }

    private void ChangeShapesPlayableState(IEnumerable<Shape> playableShapes)
    {
        playableShapes.ToList().ForEach(s => s.isPlayable = true);
        shapes.Where(s => !playableShapes.Contains(s)).ToList().ForEach(s => s.isPlayable = false);
    }
}
