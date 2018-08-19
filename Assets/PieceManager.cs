﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Random = System.Random;

public class PieceManager : MonoBehaviour {

    private List<Piece> pieces;
    private Random _randomGen;

	// Use this for initialization
	void Awake () {
        Piece[] objects = Resources.LoadAll<Piece>("Pieces");
        pieces = new List<Piece>();
        foreach (Piece obj in objects)
        {
            pieces.Add(obj);
        }
        _randomGen = new Random();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public Piece GetNextPiece()
    {
        if (pieces != null)
        {
            int index = _randomGen.Next(0, pieces.Count);
            Piece newPiece = Instantiate(pieces.ToArray()[index]);
            return newPiece;
        }
        return null;
    }
}
