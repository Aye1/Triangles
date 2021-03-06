﻿using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;
using System.Threading;
using System.Collections.Generic;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    [Header("UI objects")]
    public TextMeshProUGUI textScore;
    public TextMeshProUGUI textShuffleCount;
    public TextMeshProUGUI textHighScore;
    public TextMeshProUGUI textQuestsPoints;
    public TextMeshProUGUI textQuests;
    public EndGamePopup endGamePopup;
    public GameObject pausePopup;
    public GameObject highScorePopupText;
    public GameObject questsPopup;

    [Header("Readonly")]
    public List<Quest> currentQuests;
    public List<Quest> finishedQuests;
    private bool isCurrentQuestFinished = false;

    [Header("Game state")]
    private int globalScore;
    public int[] comboArray = new int[(int)Combo.Max];

    [Header("Debug")]
    public bool debugPieceDraggedPosition = false;
    public bool forceGameOver;


    private int _shuffleCount = 0;
    private int _highScore = 0;
    private Board _board;
    private PieceManager _pieceManager;
    private AbstractPiece _draggedPiece;
    private Timer _helpTimer;
    private Vector3[] _piecePositions;
    private Piece[] _pieceSlots;
    private Vector3 _pieceBonusDestroyPosition = new Vector3(-2f, -4f, -1.0f);

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
            if (value != _highScore)
            {
                _highScore = value;
                OnHighScoreChange(value);
            }
        }
    }

    public bool ForceGameOver
    {
        set
        {
            if (!forceGameOver && value)
            {
                forceGameOver = true;
                ManageGameOver();
            }
        }
    }
    public delegate void OnHighScoreChangeDelegate(int newVal);
    public event OnHighScoreChangeDelegate OnHighScoreChange;

    public int GlobalScore
    {
        get
        {
            return globalScore;
        }

        set
        {
            if(value!= globalScore)
            {
                globalScore = value;
                OnScoreChange(value);
            }
        }
    }

    public delegate void OnScoreChangeDelegate(int newVal);
    public event OnScoreChangeDelegate OnScoreChange;
    public bool IsCurrentQuestFinished
    {
        get
        {
            return isCurrentQuestFinished;
        }

        set
        {
            if(value != isCurrentQuestFinished)
            {
            isCurrentQuestFinished = value;
                IsCurrentQuestFinishedEvent(value);
            }
        }
    }
    public delegate void IsCurrentQuestFinishedDelegate(bool newVal);
    public event IsCurrentQuestFinishedDelegate IsCurrentQuestFinishedEvent;
    #endregion



    // Use this for initialization
    void Start()
    {
        _board = FindObjectOfType<Board>();
        _pieceManager = FindObjectOfType<PieceManager>();
        _pieceSlots = new Piece[3];
        InitPiecePositions();
        GetThreePieces();
        InitQuests();
        //GetBonusPiece();
        ComputeScore();
        UIHelper.HideGameObject(endGamePopup.gameObject);
        UIHelper.HideGameObject(pausePopup);
        LaunchHelpTimer();
        // Be sure the timeScale is at 1, in case there was an issue with the pause menu
        Time.timeScale = 1.0f;
        CleanComboArray();
        InitUI();
    }

    public void CleanComboArray()
    {
        for (Combo type = Combo.Combo1; type < Combo.Max; type++)
        {
            comboArray[(int)type] = 0;
        }
    }
    private void InitPiecePositions()
    {
        _piecePositions = new Vector3[3];
        _piecePositions[0] = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 0.25f, Screen.height * 0.2f, 0.0f));
        _piecePositions[1] = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 0.5f, Screen.height * 0.2f, 0.0f));
        _piecePositions[2] = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 0.75f, Screen.height * 0.2f, 0.0f));
        _piecePositions[0].z = -1.0f;
        _piecePositions[1].z = -1.0f;
        _piecePositions[2].z = -1.0f;
    }

    void InitUI()
    {
        _highScore = PlayerSettingsManager.Instance.HighScore;
        UpdateUI();
    }
    void UpdateUI()
    {
        if (debugPieceDraggedPosition && _draggedPiece != null)
        {
            Debug.Log("Piece dragged at pos " + _draggedPiece.transform.position.ToString());
        }
        string scoreText = LocalizationManager.Instance.GetLocString("score");
        string hsText = LocalizationManager.Instance.GetLocString("highscore");
        textScore.text = GlobalScore.ToString();
        textShuffleCount.text = _shuffleCount.ToString();
        textHighScore.text = HighScore.ToString();
        textQuestsPoints.text = PlayerSettingsManager.Instance.QuestsPoints.ToString();
        DisplayCurrentQuest();
    }

    #region Score Management
    void ComputeScore()
    {
        GlobalScore += _board.numberFlippedShapes;
        _board.numberFlippedShapes = 0;

        comboArray[(int)_board.lastNumberValidatedLines]++;

    }

    private void CheckHighScore()
    {
        bool newHighScore = GlobalScore > HighScore;
        if (newHighScore)
        {
            HighScore = GlobalScore;
            PlayerSettingsManager.Instance.HighScore = HighScore;
            LeaderboardManager.Instance.SendHighScore(PlayerSettingsManager.Instance.Name, HighScore);
        }
        endGamePopup.UpdateHighScoreInfo(newHighScore);
    }

    #endregion

    #region Quests Management
    private void InitQuests()
    {
        if (currentQuests == null)
        {
            currentQuests = new List<Quest>();
        }
        currentQuests.Clear();
        currentQuests.Add(QuestManager.Instance.GetQuest());

        if (finishedQuests == null)
        {
            finishedQuests = new List<Quest>();
        }
        finishedQuests.Clear();
    }
    public void DisplayCurrentQuest()
    {
        if (currentQuests.Count != 0)
        {
            textQuests.text = currentQuests[0].GetDescription() + " : " + currentQuests[0].SpecificInformation();
            IsCurrentQuestFinished = currentQuests[0].IsFinished;
        }
    }

    public void ReplaceQuests(List<Quest> questsToRemove)
    {
        int count = questsToRemove.Count;
        currentQuests.RemoveAll(q => questsToRemove.Contains(q));
        for (int i = 0; i < count; i++)
        {
            currentQuests.Add(QuestManager.Instance.GetQuest());
        }
    }
    public void ComputeQuest()
    {
        uint questPointGain = 0;
        foreach (Quest q in currentQuests)
        {
            if (!q.IsFinished && q.IsQuestCompleted())
            {
                finishedQuests.Add(q);
                q.IsFinished = true;
                questPointGain += q.questPointGain;
            }
        }
        
        PlayerSettingsManager.Instance.QuestsPoints += (int)questPointGain;
    }

    #endregion

    #region Pieces Management
    private int GetPieceSlotId(Piece piece)
    {
        for (int i = 0; i < _pieceSlots.Length; i++)
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

    private void GetThreePieces()
    {
        for (int i = 0; i < _piecePositions.Length; i++)
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

    #region Listeners
    private void OnPieceDragged(object sender, EventArgs e)
    {
        _draggedPiece = sender as AbstractPiece;
        _board.currentDraggedPiece = _draggedPiece;
        _board.FindPlayableShapes(_draggedPiece);
        ResetHelpTimer();
    }

    private void OnPieceReleased(object sender, EventArgs e)
    {
        if (_board.PutPiece(_draggedPiece))
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
            ComputeQuest();
            UpdateUI();
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
        if (_board.IsPiecePlayableOnShape(collisionShape, args.CurrentPiece))
        {
            _board.AddShapeToCurrentlyPlayable(collisionShape);
        }
    }

    private void OnPieceExitCollision(object sender, EventArgs e)
    {
        Shape.CollisionEventArgs args = e as Shape.CollisionEventArgs;
        Shape collisionShape = args.OtherObject.GetComponent<Shape>();
        _board.RemoveShapeToCurrentPlayable(collisionShape);
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
    #endregion
    #endregion

    #region Suffles
    /// <summary>
    /// Shuffles shapes. 
    /// Should not be called directly. 
    /// Call ShuffleUntilPlayable instead.
    /// </summary>
    private void ShuffleShapes()
    {
        foreach (Piece p in _pieceSlots)
        {
            CleanDestroyPiece(p);
        }
        GetThreePieces();
    }

    public void ShuffleUntilPlayable(bool forceShuffle = false, bool withRewarded = false)
    {
        if (ShuffleCount > 0 || forceShuffle)
        {
            ShuffleShapes();
            while (!CheckCanPlay())
            {
                ShuffleShapes();
            }
            ShuffleCount--;
            if (!forceShuffle)
            {
                // At the moment, the force shuffle is only done when restarting
                // If we change, that, we may want to add more precise information
                // of where the shuffle were made from
                IDictionary<string, object> args = new Dictionary<string, object>();
                args.Add("rewarded", withRewarded.ToString());
                AnalyticsEvent.Custom("shuffle_used", args);
            }
        }
        ResetHelpTimer();
    }

    public void ShuffleShapesInPopup(bool withRewarded = false)
    {
        UIHelper.HideGameObject(endGamePopup.gameObject);
        ShuffleUntilPlayable(false, withRewarded);
    }
    #endregion

    private void ManageGameOver()
    {
        if (!CheckCanPlay() || forceGameOver)
        {
            forceGameOver = false;
            Debug.Log("Game Over");
            UIHelper.DisplayGameObject(endGamePopup.gameObject);
            CheckHighScore();
            UpdateUI();
        }
    }

    private bool CheckCanPlay(bool shouldHighlight = false)
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

    public void Restart()
    {
        GlobalScore = 0;
        _board.ResetBoard();
        ShuffleUntilPlayable(true);
        UIHelper.HideGameObject(endGamePopup.gameObject);
        FindObjectOfType<RewardedVideoManager>().Reset();
        InitQuests();
        _shuffleCount = 0;
        HidePauseMenu();
        CleanComboArray();
        InitUI();
    }

    public void RestartWithNewBoard()
    {
        Restart();
        _board.GenerateNewBoard();
    }

    public void GoToMainMenuScreen()
    {
        HidePauseMenu();
        SceneManager.LoadScene(1);
    }

    #region UI
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

    public void DisplayQuestsPopup()
    {
        UIHelper.DisplayGameObject(questsPopup);
    }
    #endregion

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

#if UNITY_EDITOR

    #region Debug
    [MenuItem("Debug/Give 10 shuffles")]
    public static void GiveTenShuffles()
    {
        FindObjectOfType<GameManager>().ShuffleCount += 10;
    }

    [MenuItem("Debug/Force game over")]
    public static void GameOver()
    {
        FindObjectOfType<GameManager>().ForceGameOver = true;
    }

    [MenuItem("Debug/Set new highscore")]
    public static void SetNewHighscore()
    {
        GameManager gm = FindObjectOfType<GameManager>();
        gm.GlobalScore = gm.HighScore + 1;
        gm.ComputeQuest();
        gm.UpdateUI();
    }
    #endregion
#endif
}
