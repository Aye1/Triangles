using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public TextMeshProUGUI textScore;
    public TextMeshProUGUI textShuffle;
    public TextMeshProUGUI textHighScore;
    public TextMeshProUGUI textQuestsPoints;
    public EndGamePopup endGamePopup;
    public GameObject pausePopup;
    public GameObject highScorePopupText;
    public GameObject questsPopup;
    public List<Quest> currentQuests;

    public int globalScore;

    private int _shuffleCount = 0;
    private int _highScore = 0;
    private string highScoreKey = "highScore";

    private Board _board;
    private PieceManager _pieceManager;
    private AbstractPiece _draggedPiece;

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
        _pieceSlots = new Piece[3];
        InitPiecePositions();
        GetSavedHighScore();
        GetThreePieces();
        InitQuests();
        //GetBonusPiece();
        ComputeScore();
        UIHelper.HideGameObject(endGamePopup.gameObject);
        UIHelper.HideGameObject(pausePopup);
        LaunchHelpTimer();
        // Be sure the timeScale is at 1, in case there was an issue with the pause menu
        Time.timeScale = 1.0f;
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

    private void InitQuests() {
        if(currentQuests == null) {
            currentQuests = new List<Quest>();
        }
        currentQuests.Clear();
        currentQuests.Add(QuestManager.Instance.GetQuest());
    }

    public void ReplaceQuests(List<Quest> questsToRemove) {
        int count = questsToRemove.Count;
        currentQuests.RemoveAll(q => questsToRemove.Contains(q));
        for (int i = 0; i < count; i++){
            currentQuests.Add(QuestManager.Instance.GetQuest());
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
        textQuestsPoints.text = PlayerSettingsManager.Instance.QuestsPoints.ToString();
    }

    void ComputeScore()
    {
        globalScore += _board.numberFlippedShapes;
        _board.numberFlippedShapes = 0;
    }

    private void CheckHighScore() 
    {
        bool newHighScore = globalScore > HighScore;
        if (newHighScore) {
            HighScore = globalScore;
            PlayerPrefs.SetInt(highScoreKey, HighScore);
            LeaderboardManager.Instance.SendHighScore(PlayerSettingsManager.Instance.Name, HighScore);
            //UIHelper.DisplayGameObject(highScorePopupText);
            //highScorePopupText.GetComponentInChildren<TextMeshProUGUI>().text = "New high score - " + HighScore + "!";
        }
        endGamePopup.DisplayHighScoreInfo(newHighScore);
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

    private PieceBonusDestroy GetBonusPiece()
    {
        PieceBonusDestroy newPiece = _pieceManager.GetBonusDestroyPiece();
        newPiece.transform.localScale = Vector3.Scale(newPiece.transform.localScale, _board.transform.lossyScale);
        ListenToPieceBonusDestroyEvent(newPiece);
        return newPiece;
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
        Piece newPiece = _pieceManager.GetNextPieceFromPools(position); 
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
            UIHelper.DisplayGameObject(endGamePopup.gameObject);
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
        UIHelper.HideGameObject(endGamePopup.gameObject);
        ShuffleUntilPlayable();
    }

    public void Restart()
    {
        globalScore = 0;
        _board.ResetBoard();
        ShuffleUntilPlayable(true);
        UIHelper.HideGameObject(endGamePopup.gameObject);
        FindObjectOfType<RewardedVideoManager>().Reset();
        InitQuests();
        _shuffleCount = 0;
        HidePauseMenu();
    }

    public void RestartWithNewBoard() {
        Restart();
        _board.GenerateNewBoard();
    }

    public void GoToMainMenuScreen()
    {
        HidePauseMenu();
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

    public void DisplayQuestsPopup() {
        UIHelper.DisplayGameObject(questsPopup);
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

    #region Timer
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
    #endregion

}
