using System;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public TextMeshProUGUI textScore;
    public TextMeshProUGUI gameOverScore;
    public int _globalScore;

    private Board _board;
    private PieceManager _pieceManager;
    private Piece _draggedPiece;

    private Vector3[] _piecePositions = {new Vector3(-1.5f, -2.5f, -1.0f),
        new Vector3(0.0f, -2.5f, -1.0f),
        new Vector3(1.5f, -2.5f, -1.0f) };

    private Piece[] _pieceSlots;

    public bool debugPieceDraggedPosition = false;


	// Use this for initialization
	void Start () {
        _board = FindObjectOfType<Board>();
        _pieceManager = FindObjectOfType<PieceManager>();
        _pieceSlots = new Piece[3];
        gameOverScore.alpha = 0.0f;
        GetThreePieces();
        ComputeScore();
	}
	
	// Update is called once per frame
	void Update () {
		if(debugPieceDraggedPosition && _draggedPiece != null)
        {
            Debug.Log("Piece dragged at pos " + _draggedPiece.transform.position.ToString());
        }
        DisplayPieceHover();
	}

    void ComputeScore()
    {
        _globalScore += _board.numberFlippedShapes;
            _board.numberFlippedShapes = 0;
        textScore.text = "Score : " + _globalScore;
    }

    private void DisplayPieceHover()
    {
        if (_draggedPiece != null)
        {
            _board.DisplayPieceHover(_draggedPiece);
        }
    }

    private void OnPieceDragged(object sender, EventArgs e)
    {
        _draggedPiece = sender as Piece;
        _board.FindPlayableShapes(_draggedPiece);
    }

    private void OnPieceReleased(object sender, EventArgs e)
    {
        if(_board.PutPiece(_draggedPiece))
        {
            int idPos = GetPieceSlotId(_draggedPiece);
            CleanDestroyPiece(_draggedPiece);
            Piece newPiece = GetNewPiece();
            newPiece.transform.position = _piecePositions[idPos];
            _pieceSlots[idPos] = newPiece;

            ComputeScore();
            ManageGameOver();
        }
        else
        {
            _draggedPiece.transform.position = _piecePositions[GetPieceSlotId(_draggedPiece)];
        }
        _draggedPiece = null;
    }

    private void GetThreePieces()
    {
        for(int i=0; i<_piecePositions.Length; i++)
        {
            Piece newPiece = GetNewPiece();
            newPiece.transform.position = _piecePositions[i];
            _pieceSlots[i] = newPiece;
        }
    }

    private int GetPieceSlotId(Piece piece)
    {
        for(int i=0; i<_pieceSlots.Length; i++)
        {
            if (_pieceSlots[i] == piece)
            {
                return i;
            }
        }
        return -1;
    }

    private Piece GetNewPiece()
    {
        Piece newPiece = _pieceManager.GetNextPiece();
        newPiece.transform.parent = transform;
        newPiece.transform.localScale = Vector3.Scale(newPiece.transform.localScale, _board.transform.lossyScale);
        ListenToPieceEvent(newPiece);
        return newPiece;
    }

    private void ManageGameOver()
    {
        if (!CheckCanPlay())
        {
            Debug.Log("Game Over");
            ChangeVisibility(gameOverScore);
        }
    }

    private bool CheckCanPlay()
    {
        bool canPlay = false;
        foreach (Piece p in _pieceSlots)
        {
            if (p != null)
            {
                canPlay = canPlay || _board.CheckCanPlay(p);
            }
        }
        return canPlay;
    }

    private void CleanDestroyPiece(Piece piece)
    {
        piece.PieceDraggedHandler -= OnPieceDragged;
        piece.PieceReleasedHandler -= OnPieceReleased;
        piece.DestroyPiece();
    }

    private void ListenToPieceEvent(Piece newPiece)
    {
        newPiece.PieceDraggedHandler += OnPieceDragged;
        newPiece.PieceReleasedHandler += OnPieceReleased;
    }

    private void ChangeVisibility(TextMeshProUGUI text)
    {
        text.alpha = 1.0f - text.alpha;
    }

    public void ShuffleShapes()
    {
        foreach(Piece p in _pieceSlots)
        {
            CleanDestroyPiece(p);
        }
        GetThreePieces();
    }
}
