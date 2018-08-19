using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class PieceManager : MonoBehaviour {

    public List<Piece> pieces;
    private Random _randomGen;

	// Use this for initialization
	void Start () {
        _randomGen = new Random();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public Piece GetNextPiece()
    {
        if (pieces != null)
        {
            int index = _randomGen.Next(0, pieces.Count-1);
            Piece newPiece = Instantiate(pieces.ToArray()[index]);
            return newPiece;
        }
        return null;
    }
}
