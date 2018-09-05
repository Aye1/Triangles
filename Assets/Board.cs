using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{

    private readonly int _minWidth = 9;
    private readonly float sqr3 = Mathf.Sqrt(3);

    private List<Shape> shapes;
    private IEnumerable<Shape> unfilledShapes;
    private IEnumerable<Shape> playableShapes;

    public Shape basicShape;
    public int numberFlippedShapes = 0;
    public int numberFlippedLines = 0;

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

    /// <summary>
    /// Checks if a line in complete
    /// </summary>
    /// <param name="valShapes">List of the shapes which create a line and should be removed</param>
    /// <param name="index">Index of the current shape on the line analysed</param>
    /// <param name="pos">Id of the line analysed (0=A, 1=B, 2=C)</param>
    /// <returns></returns>
    private IEnumerable<Shape> CheckLine(int index, int pos)
    {
        IEnumerable<Shape> toValidate = shapes.Where<Shape>(shape => shape.PositionABC[pos] == index);
        bool lineValidated = toValidate.All(shape => shape.isFilled);
        if (lineValidated)
        {
            return toValidate;
        }
        return null;
    }

    /// <summary>
    /// Puts shapes which form lines to their basic state
    /// </summary>
    /// <param name="valShapes">The list of shapes which form lines</param>
    private void FlipValidatedLines(IEnumerable<Shape> valShapes)
    {
        valShapes.ToList().ForEach(shape => shape.isFilled = false);
        StartCoroutine(VisuallyFlipShapes(valShapes));

        numberFlippedShapes = valShapes.ToList().Count;
    }

    private IEnumerator VisuallyFlipShapes(IEnumerable<Shape> flipShapes)
    {
        foreach (Shape s in flipShapes)
        {
            s.IsVisuallyFilled = false;
            yield return new WaitForSeconds(0.02f);
        }
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
                newShape.PositionABC = new Vector3Int(A, B, C);
                newShape.PosXY = new Vector2(i, j);

                newShape.IsUpsideDown = ((i + j) % 2 == 0);

                Vector3 locScale = transform.lossyScale;
               
                newShape.transform.localScale = Vector3.Scale(locScale,newShape.transform.localScale);

                Vector3 shapePosition = transform.position + new Vector3(i*Config.paddingX, 
                    -j*Config.paddingY, 
                    newShape.transform.position.z);
                newShape.transform.position = Vector3.Scale(shapePosition, locScale);
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
        IEnumerable<Shape> resShapes = new List<Shape>();
        resShapes = shapes.Where(s => IsPiecePlayableOnShape(s, piece));
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
        unfilledShapes = shapes.Where(s => !s.isFilled);
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

    public void DisplayPieceHover(Piece piece)
    {
        Shape hoveredShape = GetShapeAtPos(piece.transform.position, false);
        if(playableShapes.Contains(hoveredShape))
        {
            IEnumerable<Shape> hoveredShapes = GetNecessaryShapesForPiece(hoveredShape, piece, shapes);
            hoveredShapes.ToList().ForEach(s => s.isPlayable = true);
            hoveredShapes.ToList().ForEach(s => s.FilledColor = piece.PieceColor);
        }
    } 
    
    /*public Shape GetShapeAtPos(Vector3 pos)
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
    }*/

    public Shape GetShapeAtPos(Vector3 pos, bool shouldConvertToWorldPosition)
    {
        Shape resShape = null;
        Vector3 realPos = shouldConvertToWorldPosition ? Camera.main.ScreenToWorldPoint(pos) : pos;
        Vector2 pos2D = new Vector2(realPos.x, realPos.y);
        RaycastHit2D hit = Physics2D.Raycast(pos2D, Vector2.zero, Mathf.Infinity, Physics.DefaultRaycastLayers, 0.0f);
        if(hit.collider != null)
        {
            resShape = hit.collider.gameObject.GetComponent<Shape>();
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
        Shape hoveredShape = GetShapeAtPos(piece.transform.position, false);
        if (playableShapes.Contains(hoveredShape))
        {
            IEnumerable<Shape> addedShapes = GetNecessaryShapesForPiece(hoveredShape, piece, shapes);
            Color pColor = piece.PieceColor;
            addedShapes.ToList().ForEach(s => s.isFilled = true);
            addedShapes.ToList().ForEach(s => s.IsVisuallyFilled = true);
            addedShapes.ToList().ForEach(s => s.FilledColor = pColor);
            ValidateLines(addedShapes);
            return true;
        }
        return false;
    }

    private void ValidateLines(IEnumerable<Shape> addedShapes)
    {
        List<IEnumerable<Shape>> validatedShapes = new List<IEnumerable<Shape>>();
        foreach (Shape sh in addedShapes)
        {
            IEnumerable<Shape> curShapes = CheckLine(sh.PositionABC.x, 0);
            if (curShapes != null)
            {
                validatedShapes.Add(curShapes.OrderBy(s => -s.PositionABC.y));
            }
            curShapes = CheckLine(sh.PositionABC.y, 1);
            if (curShapes != null)
            {
                validatedShapes.Add(curShapes.OrderBy(s => s.PositionABC.z));
            }
            curShapes = CheckLine(sh.PositionABC.z, 2);
            if (curShapes != null)
            {
                validatedShapes.Add(curShapes.OrderBy(s => s.PositionABC.x));
            }
        }

        
        if(validatedShapes.Count - 1 >= 0)
        {
            numberFlippedLines = validatedShapes.Count - 1;
        }

        validatedShapes.Where(ls => ls != null).ToList().ForEach(ls => FlipValidatedLines(ls));
    }

    public bool CheckCanPlay(Piece piece)
    {
        return FindPlayableShapes(piece).Count() > 0;
    }

    public void ResetBoard()
    {
        shapes.ForEach(s => s.isFilled = false);
        shapes.ForEach(s => s.IsVisuallyFilled = false);
    }
}
