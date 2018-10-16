using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    private List<Shape> _shapes;
    private List<Shape> _currentHoveredPlayablePositions;
    private Level _currentLevel;

    public Shape basicShape;
    public int numberFlippedShapes = 0;
    public int numberFlippedLines = 0;
    public int lastNumberValidatedLines = 0;
    public AbstractPiece currentDraggedPiece;

    public bool debugHoveredPlayablePositions;

    public Level CurrentLevel {
        get { return _currentLevel; }
    }

    // Use this for initialization
    void Start()
    {
        _shapes = new List<Shape>();
        _currentHoveredPlayablePositions = new List<Shape>();
        SelectBoard(PlayerSettingsManager.Instance.CurrentLevel);
        ClearCurrentPiece();
        _shapes = _currentLevel.Shapes;
    }

    // Update is called once per frame
    void Update()
    {
        if (debugHoveredPlayablePositions)
        {
            DebugDisplayAllPlayableShapes();
        }
        else
        {
            DisplayClosestPlayablePiece();
        }
    }

    private int[,] GetBoardData()
    {
        int[,] data = new int[,] {
            {0,0,1,1,1,1,1,1,1,1,1,0,0},
            {0,1,1,1,1,1,1,1,1,1,1,1,0},
            {1,1,1,0,0,1,1,1,0,0,1,1,1},
            {1,1,1,1,1,1,1,1,1,1,1,1,1},
            {0,1,1,1,1,1,1,1,1,1,1,1,0},
            {0,0,1,1,1,0,1,0,1,1,1,0,0},
            {0,0,0,1,1,1,1,1,1,1,0,0,0}
        };
        return data;
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
        IEnumerable<Shape> toValidate = _shapes.Where<Shape>(shape => shape.PositionABC[pos] == index);
        bool lineValidated = toValidate.All(shape => shape.isFilled);
        if (lineValidated && toValidate.ToList().Count() >=5)
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

    public void GenerateNewBoard() {

    }

    private void SelectBoard(int boardIndex)
    {
        Level currentLevel = Instantiate(LevelManager.Instance.GetLevelAtIndex(boardIndex));

        currentLevel.transform.SetParent(transform);
        currentLevel.transform.localScale = transform.lossyScale;
        currentLevel.GenerateBoard();
        _currentLevel = currentLevel;
    }

    /*
    public void GenerateNewBoard() {
        ResetBoard();
        foreach(Shape s in _shapes) {
            Destroy(s.gameObject);
        }
        _shapes.Clear();
        ClearCurrentPiece();
        SelectBoard(2);
    }

    private void CreateBoardFromArray(int[,] data) {
        for (int j = 0; j < data.GetLength(1); j++) {
            for (int i = 0; i < data.GetLength(0); i++)
            {
                if (data[i,j] == 1){
                    CreateBoardShape(j, i);
                } 
            }
        }
        ComputeBoardDimensions(data.GetLength(1), data.GetLength(0));
        AdjustBoardPosition();
    }*/

    /// <summary>
    /// Returns all playable shapes for the current piece
    /// </summary>
    /// <param name="piece">The currently selected piece</param>
    /// <returns></returns>
    public IEnumerable<Shape> FindPlayableShapes(AbstractPiece piece)
    {
        IEnumerable<Shape> resShapes = new List<Shape>();
        resShapes = _shapes.Where(s => IsPiecePlayableOnShape(s, piece));
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
            listShapes = _shapes.Where(s => !s.isFilled);
        }
        else
        {
            // Probably for DetroyPiece, to check with Jade
            listShapes = _shapes;
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

    public void DisplayPieceOnShape(AbstractPiece piece, Shape sh) {
        IEnumerable<Shape> hoveredShapes = GetNecessaryShapesForPieceWithFirstShape(sh, piece, _shapes);

        hoveredShapes.ToList().ForEach(s => s.isPlayable = true);
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
        if (currentDraggedPiece != null && _currentHoveredPlayablePositions.Count > 0)
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
        // Offset ~= 2/3 - 0.5
        float offset = 0.16f;
        if (sh.IsUpsideDown) {
            pos = new Vector3(sh.transform.position.x, sh.transform.position.y - offset, sh.transform.position.z);
        } else {
            pos = new Vector3(sh.transform.position.x, sh.transform.position.y + offset, sh.transform.position.z);
        }
        return pos;
    }

    public void ClearCurrentPiece()
    {
        currentDraggedPiece = null;
        _currentHoveredPlayablePositions.Clear();
    }

    /// <summary>
    /// Puts the piece on the board
    /// </summary>
    /// <param name="piece">The piece to put</param>
    /// <returns>True if the piece is really put, false else</returns>
    public bool PutPiece(AbstractPiece piece)
    {
        //Shape hoveredShape = GetShapeAtPos(piece.transform.position, false);
        Shape playableShape = GetClosestPlayableShape();
        if (playableShape != null)
        {
            IEnumerable<Shape> addedShapes = GetNecessaryShapesForPieceWithFirstShape(playableShape, piece, _shapes);
            if (piece is Piece)
            {
                Color pColor = piece.PieceColor;
                addedShapes.ToList().ForEach(s => s.isFilled = true);
                addedShapes.ToList().ForEach(s => s.IsVisuallyFilled = true);
                addedShapes.ToList().ForEach(s => s.FilledColor = pColor);
                ValidateUniqueLines(addedShapes);
            }
            else if (piece is PieceBonusDestroy)
            {
                addedShapes.ToList().ForEach(s => s.isFilled = false);
                addedShapes.ToList().ForEach(s => s.IsVisuallyFilled = false);
            }
            return true;
        }
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
        _shapes.ForEach(s => s.isFilled = false);
        _shapes.ForEach(s => s.IsVisuallyFilled = false);
        ClearCurrentPiece();
    }
}
