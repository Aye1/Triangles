using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{

    private readonly int _minWidth = 9;
    private readonly float sqr3 = Mathf.Sqrt(3);
    private readonly string boardTag = "Board";

    private List<Shape> shapes;
    private IEnumerable<Shape> unfilledShapes;
    private IEnumerable<Shape> playableShapes;
    private List<Shape> _currentHoveredPlayablePositions;

    public Shape basicShape;
    public int numberFlippedShapes = 0;
    public int numberFlippedLines = 0;
    public int lastNumberValidatedLines = 0;
    public AbstractPiece currentDraggedPiece;

    public bool debugHoveredPlayablePositions;

    // Use this for initialization
    void Awake()
    {
        shapes = new List<Shape>();
        _currentHoveredPlayablePositions = new List<Shape>();
        CreateBoard();
    }

    // Update is called once per frame
    void Update()
    {
        DisplayClosestPlayablePiece();
    }

    // Debug method
    // When called, displays all the playable shapes in color
    private void DebugDisplayAllPlayableShapes() {
        if(debugHoveredPlayablePositions) {
            foreach(Shape sh in _currentHoveredPlayablePositions) {
                sh.isPlayable = true;
                sh.HoveredColor = Color.magenta;
            }
            Shape closest = GetClosestPlayableShape();
            if (closest != null)
            {
                closest.HoveredColor = Color.yellow;
            }
        }
    }

    /// <summary>
    /// Checks if a line in complete
    /// </summary>
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
                newShape.gameObject.tag = boardTag;
                Vector2 vecB = new Vector2(sqr3 * i / 2, -j / 2);
                Vector2 vecC = new Vector2(-sqr3 * i / 2, -j / 2);
                int A = j;
                int B = (int)Mathf.Ceil((-i - j) / 2.0f);
                int C = (int)Mathf.Floor(-(i - j) / 2.0f);
                newShape.PositionABC = new Vector3Int(A, B, C);
                newShape.PosXY = new Vector2(i, j);

                newShape.IsUpsideDown = ((i + j) % 2 == 0);

                Vector3 locScale = transform.lossyScale;

                newShape.transform.localScale = Vector3.Scale(locScale, newShape.transform.localScale);

                Vector3 shapePosition = transform.position + new Vector3(i * Config.paddingX,
                    -j * Config.paddingY,
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
    public IEnumerable<Shape> FindPlayableShapes(AbstractPiece piece)
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
    public bool IsPiecePlayableOnShape(Shape shape, AbstractPiece piece)
    {
        IEnumerable<Shape> listShapes;
        if (piece is Piece)
        {
            listShapes = shapes.Where(s => !s.isFilled);
        }
        else
        {
            // Probably for DetroyPiece, to check with Jade
            listShapes = shapes;
        }
        IEnumerable<Shape> necessaryShapes = GetNecessaryShapesForPieceWithFirstShape(shape, piece, listShapes);
        return necessaryShapes.Count() == piece.pieceShapes.Count;
    }

    /// <summary>
    /// Returns the list of shapes necessary to fit the piece, starting with the current shape
    /// /!\ You probably want to use GetNecessaryShapesForPieceWithFirstShape
    /// </summary>
    /// <param name="shape">The base shape to create the piece</param>
    /// <param name="piece">The piece to fit</param>
    /// <param name="shapeList">The list of shapes in which to pick</param>
    /// <returns>A (possibly incomplete) list of shapes fitting the piece</returns>
    private IEnumerable<Shape> GetNecessaryShapesForPiece(Shape shape, AbstractPiece piece, IEnumerable<Shape> shapeList)
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

    /// <summary>
    /// Returns the list of shapes necessary to fit the piece, starting with the current shape on the board
    /// and the first shape of the pice
    /// </summary>
    /// <param name="shape">The base shape to create the piece</param>
    /// <param name="piece">The piece to fit</param>
    /// <param name="shapeList">The list of shapes in which to pick</param>
    /// <returns>A (possibly incomplete) list of shapes fitting the piece</returns>
    private IEnumerable<Shape> GetNecessaryShapesForPieceWithFirstShape(Shape shape, AbstractPiece piece, IEnumerable<Shape> shapeList)
    {
        List<Shape> necessaryShapes = new List<Shape>();
        Shape firstShape = piece.firstShape;

        foreach (Shape s in piece.pieceShapes)
        {
            Vector2 searchPos = s.PosXY + shape.PosXY - firstShape.PosXY;
            IEnumerable<Shape> found = shapeList.Where(us => us.PosXY == searchPos
                                                            && us.IsUpsideDown == s.IsUpsideDown);
            if (found.Count() != 0)
            {
                necessaryShapes.Add(found.First());
            }
        }
        return necessaryShapes;
    }

    /* To remove when we are sure it's useless
     * public void DisplayPieceHover(AbstractPiece piece)
    {
        Shape firstShape = piece.pieceShapes.First();
        Shape hoveredShape = GetShapeAtPos(piece.transform.position, false);
        if (piece is Piece && playableShapes.Contains(hoveredShape))
        {
            //IEnumerable<Shape> hoveredShapes = GetNecessaryShapesForPiece(hoveredShape, piece, shapes);
            IEnumerable<Shape> hoveredShapes = GetNecessaryShapesForPieceWithFirstShape(hoveredShape, piece, shapes);

            hoveredShapes.ToList().ForEach(s => s.isPlayable = true);
            hoveredShapes.ToList().ForEach(s => s.FilledColor = piece.PieceColor);
            hoveredShapes.ToList().ForEach(s => s.HoveredColor = piece.PieceColor);
        }

        else if (piece is PieceBonusDestroy && shapes.Contains(hoveredShape))
        {
            IEnumerable<Shape> hoveredShapes = GetNecessaryShapesForPiece(hoveredShape, piece, shapes);
            hoveredShapes.ToList().ForEach(s => s.isPlayable = true);
            hoveredShapes.ToList().ForEach(s => s.HoveredColor = piece.PieceColor);

        }
    }*/

    /*public void DisplayFirstPlayablePieceHover(AbstractPiece piece)
    {
        Shape playableShape = GetPlayableShapeInNeighbourhood(piece as Piece, false);
        if (playableShape != null) {
            IEnumerable<Shape> hoveredShapes = GetNecessaryShapesForPiece(playableShape, piece, shapes);
            hoveredShapes.ToList().ForEach(s => s.isPlayable = true);
            hoveredShapes.ToList().ForEach(s => s.FilledColor = piece.PieceColor);
            hoveredShapes.ToList().ForEach(s => s.HoveredColor = piece.PieceColor);
        }
    }*/

    public void DisplayPieceOnShape(AbstractPiece piece, Shape sh) {
        IEnumerable<Shape> hoveredShapes = GetNecessaryShapesForPieceWithFirstShape(sh, piece, shapes);

        hoveredShapes.ToList().ForEach(s => s.isPlayable = true);
        hoveredShapes.ToList().ForEach(s => s.FilledColor = piece.PieceColor);
        hoveredShapes.ToList().ForEach(s => s.HoveredColor = piece.PieceColor);
    }

    public void AddShapeToCurrentlyPlayable(Shape sh) {
        _currentHoveredPlayablePositions.Add(sh);
    }

    public void RemoveShapeToCurrentPlayable(Shape sh) {
        if(_currentHoveredPlayablePositions.Contains(sh)) {
            _currentHoveredPlayablePositions.Remove(sh);
        }
    }

    public void DisplayClosestPlayablePiece() {
        Shape playableShape = GetClosestPlayableShape();
        if(playableShape != null) {
            DisplayPieceOnShape(currentDraggedPiece, playableShape);
        }
    }

    private Shape GetClosestPlayableShape() {
        if (_currentHoveredPlayablePositions.Count > 0)
        {
            Shape playableShape = _currentHoveredPlayablePositions.First();
            float minDist = Vector3.Distance(GetShapeCenterPosition(playableShape), GetShapeCenterPosition(currentDraggedPiece.firstShape));
            foreach (Shape sh in _currentHoveredPlayablePositions)
            {
                float currentDist = Vector3.Distance(GetShapeCenterPosition(sh), GetShapeCenterPosition(currentDraggedPiece.firstShape));
                if (currentDist < minDist)
                {
                    minDist = currentDist;
                    playableShape = sh;
                }
            }
            return playableShape;
        }
        return null;
    }

    // Gets the position of the center of the piece
    // The center is different from the pivot center
    private Vector3 GetShapeCenterPosition(Shape sh) {
        Vector3 pos = sh.transform.position;
        float offset = 0.16f;
        if (sh.IsUpsideDown) {
            pos = new Vector3(sh.transform.position.x, sh.transform.position.y - offset, sh.transform.position.z);
        } else {
            pos = new Vector3(sh.transform.position.x, sh.transform.position.y + offset, sh.transform.position.z);
        }
        return pos;
    }

    /// <summary>
    /// Puts the piece on the board
    /// </summary>
    /// <param name="piece">The piece to put</param>
    /// <returns>True if the piece is really put, false else</returns>
    public bool PutPiece(AbstractPiece piece)
    {
        //Shape hoveredShape = GetShapeAtPos(piece.transform.position, false);
        if (piece is Piece) {
            Shape playableShape = GetClosestPlayableShape();
            if (playableShape != null) {
                IEnumerable<Shape> addedShapes = GetNecessaryShapesForPieceWithFirstShape(playableShape, piece, shapes);

                _currentHoveredPlayablePositions.Clear();

                Color pColor = piece.PieceColor;
                addedShapes.ToList().ForEach(s => s.isFilled = true);
                addedShapes.ToList().ForEach(s => s.IsVisuallyFilled = true);
                addedShapes.ToList().ForEach(s => s.FilledColor = pColor);
                ValidateUniqueLines(addedShapes);
                return true;
            }
        }       
        /*else if(piece is PieceBonusDestroy && shapes.Contains(hoveredShape))
        {
            IEnumerable<Shape> addedShapes = GetNecessaryShapesForPiece(hoveredShape, piece, shapes);
            addedShapes.ToList().ForEach(s => s.isFilled = false);
            addedShapes.ToList().ForEach(s => s.IsVisuallyFilled = false);
            return true;
        }*/
        return false;
    }

    private IEnumerable<Vector2Int> LinesToValidate(IEnumerable<Shape> addedShapes) {
        List<Vector2Int> lines = new List<Vector2Int>();
        foreach (Shape sh in addedShapes) {
            Vector2Int lineA = new Vector2Int(0, sh.PositionABC.x);
            Vector2Int lineB = new Vector2Int(1, sh.PositionABC.y);
            Vector2Int lineC = new Vector2Int(2, sh.PositionABC.z);
            if(!lines.Contains(lineA)) {
                lines.Add(lineA);
            }
            if (!lines.Contains(lineB))
            {
                lines.Add(lineB);
            }
            if (!lines.Contains(lineC))
            {
                lines.Add(lineC);
            }
        }
        return lines;
    }

    private void ValidateUniqueLines(IEnumerable<Shape> addedShapes) {
        IEnumerable<Vector2Int> linesToValidate = LinesToValidate(addedShapes);
        List<IEnumerable<Shape>> validatedShapes = new List<IEnumerable<Shape>>();
        int validatedLines = 0;

        foreach (Vector2Int line in linesToValidate) {
            IEnumerable<Shape> curShapes = CheckLine(line[1], line[0]);
            if (curShapes != null)
            {
                IEnumerable<Shape> validLine = curShapes.OrderBy(s => -s.PositionABC[(line[0] + 1) % 3]);
                if (!validatedShapes.Contains(validLine))
                {
                    validatedLines++;

                }
                validatedShapes.Add(validLine);
            }            
        }
        validatedShapes.Where(ls => ls != null).ToList().ForEach(ls => FlipValidatedLines(ls));
        lastNumberValidatedLines = validatedLines;
    }

    private int ValidateLines(IEnumerable<Shape> addedShapes)
    {
        LinesToValidate(addedShapes);
        List<IEnumerable<Shape>> validatedShapes = new List<IEnumerable<Shape>>();
        int validatedLines = 0;
        foreach (Shape sh in addedShapes)
        {
            validatedLines = validatedLines + (ValidateOneLine(sh, validatedShapes, validatedLines, 0) ? 1 : 0);
            validatedLines = validatedLines + (ValidateOneLine(sh, validatedShapes, validatedLines, 1) ? 1 : 0);
            validatedLines = validatedLines + (ValidateOneLine(sh, validatedShapes, validatedLines, 2) ? 1 : 0);
        }

        validatedShapes.Where(ls => ls != null).ToList().ForEach(ls => FlipValidatedLines(ls));
        lastNumberValidatedLines = validatedLines;
        return validatedLines;
    }

    private bool ValidateOneLine(Shape sh, List<IEnumerable<Shape>> validatedShapes, int validatedLines, int orderIndex){
        IEnumerable<Shape> curShapes = CheckLine(sh.PositionABC[orderIndex], orderIndex);
        if (curShapes != null)
        {
            IEnumerable<Shape> validLine = curShapes.OrderBy(s => -s.PositionABC[(orderIndex+1)%3]);
            if (!validatedShapes.Contains(validLine))
            {
                validatedLines++;

            }
            validatedShapes.Add(validLine);
            return true;
        }
        return false;
    }

    public bool CheckCanPlay(Piece piece)
    {
        return FindPlayableShapes(piece).Count() > 0;
    }

    public void ResetBoard()
    {
        shapes.ForEach(s => s.isFilled = false);
        shapes.ForEach(s => s.IsVisuallyFilled = false);
        unfilledShapes = shapes;
        playableShapes = null;
    }
}
