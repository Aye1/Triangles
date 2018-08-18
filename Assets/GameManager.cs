using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    private Board _board;
    public Piece piece;

    private Piece _draggedPiece;

	// Use this for initialization
	void Start () {
        _board = FindObjectOfType<Board>();
        piece.PieceDraggedHandler += OnPieceDragged;
        piece.PieceReleasedHandler += OnPieceReleased;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnPieceDragged(object sender, EventArgs e)
    {
        _draggedPiece = sender as Piece;
        _board.GetPlayableShapes(_draggedPiece);
    }

    private void OnPieceReleased(object sender, EventArgs e)
    {
        _draggedPiece = null;
    }

}
