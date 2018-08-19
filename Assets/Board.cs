using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{

    private readonly int _minWidth = 7;
    private readonly float sqr3 = Mathf.Sqrt(3);

    private List<Shape> shapes;
    private IEnumerable<Shape> unfilledShapes;
    private IEnumerable<Shape> playableShapes;

    public Shape basicShape;
    public int numberFlippedShapes = 0;

    // Use this for initialization
    void Awake()
    {
        shapes = new List<Shape>();
        CreateBoard();
    }

    // Update is called once per frame
    void Update()
    {
    }

    // Called when any shape is clicked
    /*public void OnShapeClicked(object shapeSender, EventArgs e)
    {
        Shape shape = shapeSender as Shape;
        Debug.Log("Shape at pos " + shape.Position.ToString() + " clicked");
        List<Shape> validatedShapes = new List<Shape>();
        validatedShapes = CheckLine(validatedShapes, shape.Position.x, 0);
        validatedShapes = CheckLine(validatedShapes, shape.Position.y, 1);
        validatedShapes = CheckLine(validatedShapes, shape.Position.z, 2);
        FlipValidatedLines(validatedShapes);
    }*/

    /// <summary>
    /// Checks if a line in complete
    /// </summary>
    /// <param name="valShapes">List of the shapes which create a line and should be removed</param>
    /// <param name="index">Index of the current shape on the line analysed</param>
    /// <param name="pos">Id of the line analysed (0=A, 1=B, 2=C)</param>
    /// <returns></returns>
    private List<Shape> CheckLine(List<Shape> valShapes, int index, int pos)
    {
        IEnumerable<Shape> toValidate = shapes.Where<Shape>(shape => shape.Position[pos] == index);
        bool lineValidated = toValidate.All(shape => shape.isFilled);
        if (lineValidated)
        {
            valShapes.AddRange(toValidate);
        }
        return valShapes;
    }

    /// <summary>
    /// Puts shapes which form lines to their basic state
    /// </summary>
    /// <param name="valShapes">The list of shapes which form lines</param>
    private void FlipValidatedLines(List<Shape> valShapes)
    {
        valShapes.ForEach(shape => shape.isFilled = false);

        numberFlippedShapes = valShapes.Count;
    }

    /// <summary>
    /// Initializes the board
    /// </summary>
    private void CreateBoard()
    {
        int height = _minWidth - 1;
        int maxWidth = 2 * _minWidth - 3;

        // Décalage de 1 pour rester sur la bonne parité de grille
        int offset = height / 2 % 2 == 0 ? 1 : 0;

        for (int j = 0; j < height; j++)
        {
            int currentWidth = maxWidth - (int)Mathf.Abs(_minWidth / 2.0f - (j + 1)) * 2;
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

                newShape.transform.position = transform.position + new Vector3(i*Config.paddingX, -j*Config.paddingY, newShape.transform.position.z);
                newShape.transform.parent = transform;
                shapes.Add(newShape);
                unfilledShapes = shapes;
            }
        }
    }

    /// <summary>
    /// Returns all playable shapes for the current piece
    /// </summary>
    /// <param name="piece">The currently selected piece</param>
    /// <returns></returns>
    public IEnumerable<Shape> FindPlayableShapes(Piece piece)
    {
        unfilledShapes = shapes.Where(s => !s.isFilled);

        IEnumerable<Shape> resShapes = new List<Shape>();
        resShapes = unfilledShapes.Where(s => IsPiecePlayableOnShape(s, piece));
        playableShapes = resShapes;
        return resShapes;
    }

    /// <summary>
    /// Determines whether the piece can be played on a particular shape
    /// </summary>
    /// <param name="shape"></param>
    /// <param name="piece"></param>
    /// <returns></returns>
    private bool IsPiecePlayableOnShape(Shape shape, Piece piece)
    {
        IEnumerable<Shape> necessaryShapes = GetNecessaryShapesForPiece(shape, piece, unfilledShapes);
        return necessaryShapes.Count() == piece.pieceShapes.Count;
    }

    /// <summary>
    /// Returns the list of shapes necessary to fit the piece, starting with the current shape
    /// </summary>
    /// <param name="shape">The base shape to create the piece</param>
    /// <param name="piece">The piece to fit</param>
    /// <param name="shapeList">The list of shapes in which to pick</param>
    /// <returns>A (possibly incomplete) list of shapes fitting the piece</returns>
    private IEnumerable<Shape> GetNecessaryShapesForPiece(Shape shape, Piece piece, IEnumerable<Shape> shapeList)
    {
        List<Shape> necessaryShapes = new List<Shape>();
        foreach (Shape s in piece.pieceShapes)
        {
            Vector2 searchPos = s.PosXY + shape.PosXY;
            IEnumerable<Shape> found = shapeList.Where(us => us.PosXY == searchPos
                                                            && us.IsUpsideDown == s.IsUpsideDown);
            if (found.Count() != 0)
            {
                necessaryShapes.Add(found.First());
            }
        }
        return necessaryShapes;
    }

    /*private void ChangeShapesPlayableState(IEnumerable<Shape> playableShapes)
    {
        playableShapes.ToList().ForEach(s => s.isPlayable = true);
        shapes.Where(s => !playableShapes.Contains(s)).ToList().ForEach(s => s.isPlayable = false);
    }*/

    public void DisplayPieceHover(Piece piece)
    {
        Shape hoveredShape = GetShapeAtPos(piece.transform.position);
        if(playableShapes.Contains(hoveredShape))
        {
            GetNecessaryShapesForPiece(hoveredShape, piece, shapes).All(s => s.isPlayable = true);
        }
    } 
    
    public Shape GetShapeAtPos(Vector3 pos)
    {
        Shape resShape = null;
        Vector3 offset = transform.position;
        Vector2 posOnBoard = new Vector2((pos - offset).x/Config.paddingX, -(pos-offset).y/Config.paddingY);
        IEnumerable<Shape> foundShapes = shapes.Where(s => Mathf.Abs(s.PosXY.x - posOnBoard.x) < 0.5f
                                                        && Mathf.Abs(s.PosXY.y - posOnBoard.y) < 0.5f);
        if (foundShapes.Count() != 0)
        {
            foundShapes.All(s => s.tmpBool = true);
            resShape = foundShapes.First();
            //Debug.Log("Hover on shape at pos " + resShape.PosXY);
        }
        return resShape;
    }

    /// <summary>
    /// Puts the piece on the board
    /// </summary>
    /// <param name="piece">The piece to put</param>
    /// <returns>True if the piece is really put, false else</returns>
    public bool PutPiece(Piece piece)
    {
        Shape hoveredShape = GetShapeAtPos(piece.transform.position);
        if (playableShapes.Contains(hoveredShape))
        {
            IEnumerable<Shape> addedShapes = GetNecessaryShapesForPiece(hoveredShape, piece, shapes);
            addedShapes.All(s => s.isFilled = true);
            ValidateLines(addedShapes);
            return true;
        }
        return false;
    }

    private void ValidateLines(IEnumerable<Shape> addedShapes)
    {
        List<Shape> validatedShapes = new List<Shape>();
        foreach (Shape sh in addedShapes)
        {
            validatedShapes = CheckLine(validatedShapes, sh.Position.x, 0);
            validatedShapes = CheckLine(validatedShapes, sh.Position.y, 1);
            validatedShapes = CheckLine(validatedShapes, sh.Position.z, 2);
        }
        FlipValidatedLines(validatedShapes);
    }

    public bool CheckCanPlay(Piece piece)
    {
        return FindPlayableShapes(piece).Count() > 0;
    }
}
