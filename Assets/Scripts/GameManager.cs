using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;

public class GameManager : MonoBehaviour {

    public TextMeshProUGUI textScore;
    public TextMeshProUGUI textShuffle;
    public TextMeshProUGUI textHighScore;
    public GameObject endGamePopup;
    public GameObject pausePopup;
    public GameObject highScorePopupText;

    public int globalScore;

    private int _shuffleCount = 0;
    private int _highScore = 0;
    private string highScoreKey = "highScore";

    private Board _board;
    private PieceManager _pieceManager;
    private AbstractPiece _draggedPiece;
    private LeaderboardManager _leaderboardManager;

    private Timer _helpTimer;

    private Vector3[] _piecePositions;


    private Piece[] _pieceSlots;
    private Vector3 _pieceBonusDestroyPosition = new Vector3(-2f, -4f, -1.0f);
    public bool debugPieceDraggedPosition = false;

    #region Properties
    public int ShuffleCount
    {
        get
        {
            return _shuffleCount;
        }

        set
        {
            _shuffleCount = value;
        }
    }

    public int HighScore
    {
        get
        {
            return _highScore;
        }

        set
        {
            _highScore = value;
        }
    }
    #endregion


    // Use this for initialization
    void Start () {
        _board = FindObjectOfType<Board>();
        _pieceManager = FindObjectOfType<PieceManager>();
        _leaderboardManager = FindObjectOfType<LeaderboardManager>();
        _pieceSlots = new Piece[3];
        InitPiecePositions();
        GetSavedHighScore();
        GetThreePieces();
        //GetBonusPiece();
        ComputeScore();
        UIHelper.HideGameObject(endGamePopup);
        UIHelper.HideGameObject(pausePopup);
        LaunchHelpTimer();
	}

    private void InitPiecePositions() {
        _piecePositions = new Vector3[3];
        _piecePositions[0] = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 0.25f, Screen.height * 0.2f, 0.0f));
        _piecePositions[1] = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 0.5f, Screen.height * 0.2f, 0.0f));
        _piecePositions[2] = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 0.75f, Screen.height * 0.2f, 0.0f));
        _piecePositions[0].z = -1.0f;
        _piecePositions[1].z = -1.0f;
        _piecePositions[2].z = -1.0f;
    }

    private void GetSavedHighScore() 
    {
        if(PlayerPrefs.HasKey(highScoreKey)) {
            HighScore = PlayerPrefs.GetInt(highScoreKey);
        }
    }
	
	// Update is called once per frame
	void Update () {
		if(debugPieceDraggedPosition && _draggedPiece != null)
        {
            Debug.Log("Piece dragged at pos " + _draggedPiece.transform.position.ToString());
        }
        textScore.text = "Score : " + globalScore;
        textShuffle.text = "Shuffle : " + _shuffleCount;
        textHighScore.text = "High Score : " + HighScore;
    }

    void ComputeScore()
    {
        globalScore += _board.numberFlippedShapes;
        _board.numberFlippedShapes = 0;
    }

    private void CheckHighScore() 
    {
        if(globalScore > HighScore) {
            HighScore = globalScore;
            PlayerPrefs.SetInt(highScoreKey, HighScore);
            _leaderboardManager.SendHighScore("Test", HighScore);
            highScorePopupText.GetComponentInChildren<TextMeshProUGUI>().text = "New high score - " + HighScore + "!";
            UIHelper.DisplayGameObject(highScorePopupText);
        } else {
            UIHelper.HideGameObject(highScorePopupText);
        }
    }

    private void OnPieceDragged(object sender, EventArgs e)
    {
        _draggedPiece = sender as AbstractPiece;
        _board.currentDraggedPiece = _draggedPiece;
        _board.FindPlayableShapes(_draggedPiece);
        ResetHelpTimer();
    }

    private void OnPieceReleased(object sender, EventArgs e)
    {
        if(_board.PutPiece(_draggedPiece))
        {
            _board.ClearCurrentPiece();

            int idPos = GetPieceSlotId(_draggedPiece as Piece);
            CleanDestroyPiece(_draggedPiece);
            Piece newPiece = GetNewPiece(_piecePositions[idPos]);
            _pieceSlots[idPos] = newPiece;

            if (_board.lastNumberValidatedLines > 1)
            {
                _shuffleCount += _board.lastNumberValidatedLines - 1;
            }
            ComputeScore();
            ManageGameOver();
        }
        else
        {
            _draggedPiece.transform.position = _piecePositions[GetPieceSlotId(_draggedPiece as Piece)];
        }
        _draggedPiece = null;
        _board.ClearCurrentPiece();
        ResetHelpTimer();
    }

    private void GetThreePieces()
    {
        for(int i=0; i<_piecePositions.Length; i++)
        {
            Piece newPiece = GetNewPiece(_piecePositions[i]);
            _pieceSlots[i] = newPiece;
        }
    }

    private void GetBonusPiece()
    {
        PieceBonusDestroy newPiece = _pieceManager.GetBonusDestroyPiece();
        newPiece.transform.parent = transform;
        newPiece.transform.localScale = Vector3.Scale(newPiece.transform.localScale, _board.transform.lossyScale);
        newPiece.transform.position = _pieceBonusDestroyPosition;
        ListenToPieceBonusDestroyEvent(newPiece);
    }

    private void OnPieceBonusDestroyDragged(object sender, EventArgs e)
    {
        _draggedPiece = sender as AbstractPiece;
        _board.currentDraggedPiece = _draggedPiece;
        _board.FindPlayableShapes(_draggedPiece);
    }

    private void OnPieceBonusDestroyReleased(object sender, EventArgs e)
    {
        if (_board.PutPiece(_draggedPiece))
        {
            CleanDestroyPiece(_draggedPiece);                    
            ManageGameOver();
        }
        else
        {
            _draggedPiece.transform.position = _pieceBonusDestroyPosition;
        }
    }

    private void OnPieceCollision(object sender, EventArgs e)
    {
        Shape.CollisionEventArgs args = e as Shape.CollisionEventArgs;
        Shape collisionShape = args.OtherObject.GetComponent<Shape>();
        if(_board.IsPiecePlayableOnShape(collisionShape, args.CurrentPiece)){
            _board.AddShapeToCurrentlyPlayable(collisionShape);
        }
    }

    private void OnPieceExitCollision(object sender, EventArgs e){
        Shape.CollisionEventArgs args = e as Shape.CollisionEventArgs;
        Shape collisionShape = args.OtherObject.GetComponent<Shape>();
        _board.RemoveShapeToCurrentPlayable(collisionShape);
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

    private Piece GetNewPiece(Vector3 position)
    {
        Piece newPiece = _pieceManager.GetNextPiece(position); 
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
            CheckHighScore();
            UIHelper.DisplayGameObject(endGamePopup);
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

    private void CleanDestroyPiece(AbstractPiece piece)
    {
        _board.ClearCurrentPiece();
        piece.PieceDraggedHandler -= OnPieceDragged;
        piece.PieceReleasedHandler -= OnPieceReleased;
        piece.PieceCollidingHandler -= OnPieceCollision;
        piece.PieceExitCollisionHandler -= OnPieceExitCollision;
        piece.DestroyPiece();
    }

    private void ListenToPieceEvent(Piece newPiece)
    {
        newPiece.PieceDraggedHandler += OnPieceDragged;
        newPiece.PieceReleasedHandler += OnPieceReleased;
        newPiece.PieceCollidingHandler += OnPieceCollision;
        newPiece.PieceExitCollisionHandler += OnPieceExitCollision;
    }
    private void ListenToPieceBonusDestroyEvent(PieceBonusDestroy newPiece)
    {
        newPiece.PieceDraggedHandler += OnPieceBonusDestroyDragged;
        newPiece.PieceReleasedHandler += OnPieceBonusDestroyReleased;
        newPiece.PieceCollidingHandler += OnPieceCollision;
        newPiece.PieceExitCollisionHandler += OnPieceExitCollision;
    }

    /// <summary>
    /// Shuffles shapes. 
    /// Should not be called directly. 
    /// Call ShuffleUntilPlayable instead.
    /// </summary>
    private void ShuffleShapes()
    {
        foreach(Piece p in _pieceSlots)
        {
            CleanDestroyPiece(p);
        }
        GetThreePieces();
    }

    public void ShuffleUntilPlayable(bool forceShuffle = false)
    {
        if(ShuffleCount > 0 || forceShuffle)
        {
            ShuffleShapes();
            while (!CheckCanPlay())
            {
                ShuffleShapes();
            }
            ShuffleCount--;
        }
        ResetHelpTimer();
    }

    public void ShuffleShapesInPopup()
    {
        UIHelper.HideGameObject(endGamePopup);
        ShuffleUntilPlayable();
    }

    public void Restart()
    {
        globalScore = 0;
        _shuffleCount = 0;
        ShuffleUntilPlayable(true);
        _board.ResetBoard();
        UIHelper.HideGameObject(endGamePopup);
        UIHelper.HideGameObject(pausePopup);
        FindObjectOfType<RewardedVideoManager>().Reset();
    }

    public void GoToMainMenuScreen()
    {
        SceneManager.LoadScene(1);
    }

    public void DisplayPauseMenu()
    {
        UIHelper.DisplayGameObject(pausePopup);
        Time.timeScale = 0.0f;
    }

    public void HidePauseMenu()
    {
        UIHelper.HideGameObject(pausePopup);
        Time.timeScale = 1.0f;
    }

    private void LaunchHelpTimer()
    {
        _helpTimer = new Timer(OnHelpTimerFinished, null, 5000, 0);
        //Debug.Log("Launch timer");
    }

    private void ResetHelpTimer()
    {
        if (_helpTimer != null)
        {
            _helpTimer.Dispose();
        }
        LaunchHelpTimer();
        UnHighlightAllPieces();
    }

    private void OnHelpTimerFinished(object state)
    {
        //Debug.Log("Timer finished");
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
