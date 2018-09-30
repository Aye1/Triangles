using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using Random = System.Random;

public class PieceManager : MonoBehaviour {

    private List<Piece> pieces;
    private List<PiecePool> _pools;
    public PieceBonusDestroy pieceBonusDestroy;
    private Random _randomGen;
    public List<Color> colors;

	// Use this for initialization
	void Awake () {
        //LoadPieces();
        LoadPiecePools();
        _randomGen = new Random();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void LoadPieces() {
        Piece[] objects = Resources.LoadAll<Piece>("Pieces");
        pieces = new List<Piece>();
        foreach (Piece obj in objects)
        {
            pieces.Add(obj);
        }
    }

    private void LoadPiecePools() {
        string path = Application.dataPath + "/Prefabs/Resources/Pieces";
        _pools = new List<PiecePool>();
        //pieces = new List<Piece>();
        foreach(string dir in Directory.GetDirectories(path)) {
            Debug.Log(dir);
            string[] pathElements = dir.Split(new char[] {'/'});
            string localPath = pathElements[pathElements.Length - 2] + "/" + pathElements[pathElements.Length - 1];
            Piece[] objects = Resources.LoadAll<Piece>(localPath);
            PiecePool pool = new PiecePool();
            pool.AddPieces(objects);
            _pools.Add(pool);
            /*oreach(Piece obj in objects) {
                pieces.Add(obj);
            }*/
        }
    }

    public Piece GetNextPiece(Vector3 position)
    {
        if (pieces != null)
        {
            int index = _randomGen.Next(0, pieces.Count);
            Piece newPiece = Instantiate(pieces.ToArray()[index], position, Quaternion.identity);
            newPiece.PieceColor = GetNextColor();
            return newPiece;
        }
        return null;
    }

    public Piece GetNextPieceFromPools(Vector3 position) {
        if(_pools != null) {
            int index = _randomGen.Next(0, _pools.Count);
            PiecePool currentPool = _pools.ToArray()[index];
            Piece newPiece = Instantiate(currentPool.GetRandomPiece(), position, Quaternion.identity);
            newPiece.PieceColor = GetNextColor();
            return newPiece;
        }
        return null;
    }

    public PieceBonusDestroy GetBonusDestroyPiece()
    {
        if (pieceBonusDestroy != null)
        {
            PieceBonusDestroy newPiece = Instantiate(pieceBonusDestroy);
            return newPiece;
        }
        return null;
    }

    private Color GetNextColor()
    {
        if (colors != null)
        {
            int index = _randomGen.Next(0, colors.Count);
            return colors[index];
        }
        return Color.white;
    }
}
