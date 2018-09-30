using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class PieceManager : MonoBehaviour {

    private List<Piece> pieces;
    private Dictionary<int, PiecePool> _manualPools;
    public PieceBonusDestroy pieceBonusDestroy;
    private Random _randomGen;
    public List<Color> colors;

	// Use this for initialization
	void Awake () {
        //LoadPieces();
        LoadPiecesWithPools();
        _randomGen = new Random();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// Loads all pieces, without putting them in pools
    /// </summary>
    private void LoadPieces() {
        Piece[] objects = Resources.LoadAll<Piece>("Pieces");
        pieces = new List<Piece>();
        foreach (Piece obj in objects)
        {
            pieces.Add(obj);
        }
    }

    /// <summary>
    /// Loads all pieces and dispatches them in pools
    /// </summary>
    private void LoadPiecesWithPools() {
        _manualPools = new Dictionary<int, PiecePool>();
        Piece[] objects = Resources.LoadAll<Piece>("Pieces");
        pieces = new List<Piece>();
        foreach (Piece obj in objects)
        {
            int poolIndex = obj.poolIndex;
            PiecePool pool;
            if (!_manualPools.ContainsKey(poolIndex)) {
                pool = new PiecePool();
                _manualPools.Add(poolIndex, pool);
            } else {
                _manualPools.TryGetValue(poolIndex, out pool);
            }
            pool.AddPiece(obj);
        }
    }

    /// <summary>
    /// Gets the next piece.
    /// To be used only if LoadPieces has been called
    /// </summary>
    /// <returns>The next piece.</returns>
    /// <param name="position">Position.</param>
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

    /// <summary>
    /// Gets the next piece from pools.
    /// To be used only if LoadPiecesWithPools has been called
    /// </summary>
    /// <returns>The next piece from pools.</returns>
    /// <param name="position">Position.</param>
    public Piece GetNextPieceFromPools(Vector3 position) {
        if(_manualPools != null) {
            int index = _randomGen.Next(0, _manualPools.Count);
            PiecePool currentPool; 
            _manualPools.TryGetValue(index, out currentPool);
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
