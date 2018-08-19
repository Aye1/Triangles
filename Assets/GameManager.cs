using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    private Board _board;
    private PieceManager _pieceManager;
    public Piece piece;

    private Piece _draggedPiece;

    public bool debugPieceDraggedPosition = false;

	// Use this for initialization
	void Start () {
        _board = FindObjectOfType<Board>();
        _pieceManager = FindObjectOfType<PieceManager>();
        piece.PieceDraggedHandler += OnPieceDragged;
        piece.PieceReleasedHandler += OnPieceReleased;
	}
	
	// Update is called once per frame
	void Update () {
		if(debugPieceDraggedPosition && _draggedPiece != null)
        {
            Debug.Log("Piece dragged at pos " + _draggedPiece.transform.position.ToString());
        }
        tmppiecedragged();
	}

    private void tmppiecedragged()
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
            CleanDestroyPiece(_draggedPiece);
            Piece newPiece = _pieceManager.GetNextPiece();
            newPiece.transform.parent = transform;
            newPiece.transform.position = new Vector3(-4.0f, 0.0f, 0.0f);
            ListenToPieceEvent(newPiece);
        }
        _draggedPiece = null;
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
}
