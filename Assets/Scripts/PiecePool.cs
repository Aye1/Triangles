using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class PiecePool {

	private List<Piece> _pieces;
    private Random _randomGen;

	public PiecePool () {
		_pieces = new List<Piece>();
        _randomGen = new Random();
    }

    public void AddPiece(Piece piece) {
        _pieces.Add(piece);
    }

    public void AddPieces(Piece[] pieces) {
        _pieces.AddRange(pieces);
    }

    public Piece GetRandomPiece() {
        int index = _randomGen.Next(0, _pieces.Count);
        return _pieces.ToArray()[index];
    }
}
