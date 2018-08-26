using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;

public class GameManager : MonoBehaviour {

    public TextMeshProUGUI textScore;
    public GameObject endGamePopup;
    public GameObject pausePopup;
    public int _globalScore;

    private Board _board;
    private PieceManager _pieceManager;
    private Piece _draggedPiece;

    private Timer _helpTimer;

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
        GetThreePieces();
        ComputeScore();
        HideGameObject(endGamePopup);
        HideGameObject(pausePopup);
        LaunchHelpTimer();
	}
	
	// Update is called once per frame
	void Update () {
		if(debugPieceDraggedPosition && _draggedPiece != null)
        {
            Debug.Log("Piece dragged at pos " + _draggedPiece.transform.position.ToString());
        }
        DisplayPieceHover();
        textScore.text = "Score : " + _globalScore;
    }

    void ComputeScore()
    {
        _globalScore += _board.numberFlippedShapes;
        _board.numberFlippedShapes = 0;
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
        ResetHelpTimer();
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
        ResetHelpTimer();
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
            DisplayGameObject(endGamePopup);
        }
    }

    private bool CheckCanPlay(bool shouldHighlight=false)
    {
        bool canPlay = false;
        foreach (Piece p in _pieceSlots)
        {
            if (p != null)
            {
                bool canPlayPiece = _board.CheckCanPlay(p);
                if (shouldHighlight)
                {
                    p.Highlight(canPlayPiece);
                }
                canPlay = canPlay || canPlayPiece;
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

    private void DisplayGameObject(GameObject obj)
    {
        obj.SetActive(true);
    }

    private void HideGameObject(GameObject obj)
    {
        obj.SetActive(false);
    }

    public void ShuffleShapes()
    {
        foreach(Piece p in _pieceSlots)
        {
            CleanDestroyPiece(p);
        }
        GetThreePieces();
        ManageGameOver();
        ResetHelpTimer();
    }

    public void ShuffleShapesInPopup()
    {
        HideGameObject(endGamePopup);
        ShuffleShapes();
    }

    public void Restart()
    {
        _globalScore = 0;
        ShuffleShapes();
        _board.ResetBoard();
        HideGameObject(endGamePopup);
    }

    public void GoToMainMenuScreen()
    {
        SceneManager.LoadScene(0);
    }

    public void DisplayPauseMenu()
    {
        DisplayGameObject(pausePopup);
        Time.timeScale = 0.0f;
    }

    public void HidePauseMenu()
    {
        HideGameObject(pausePopup);
        Time.timeScale = 1.0f;
    }

    private void LaunchHelpTimer()
    {
        _helpTimer = new Timer(OnHelpTimerFinished, null, 5000, 0);
        Debug.Log("Launch timer");
    }

    private void ResetHelpTimer()
    {
        _helpTimer.Dispose();
        LaunchHelpTimer();
        UnHighlightAllPieces();
    }

    private void OnHelpTimerFinished(object state)
    {
        Debug.Log("Timer finished");
        CheckCanPlay(true);
    }

    private void UnHighlightAllPieces()
    {
        foreach (Piece p in _pieceSlots)
        {
            if (p != null)
            {
                p.Highlight(false);
            }
        }
    }
}
