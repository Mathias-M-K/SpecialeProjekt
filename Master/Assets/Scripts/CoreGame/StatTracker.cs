using System;
using System.IO;
using Container;
using CoreGame;
using CoreGame.Interfaces;
using CoreGame.Strategies.Interfaces;
using DefaultNamespace;
using UnityEngine;

public class StatTracker : MonoBehaviour, IStatObserver
{
    [Header("Strategy")] public FileNameStrategy fileNameStrategy;
    
    [Header("Stats")]
    [SerializeField] private int nrOfMoves;
    private int _nrOfTrades;
    private string _directoryPath = "C:/MasterData/";
    private TextWriter _textWriter;

    public FileNamingStrategy fileCreationStrategy;

    private void Start()
    {
        //initializing fileCreationStrategy
        switch (fileNameStrategy)
        {
            case FileNameStrategy.NumberBased:
                fileCreationStrategy = new NumberBasedFileCreation();
                break;
            case FileNameStrategy.DateBased:
                fileCreationStrategy = new DateFileNamingStrategy();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        
        GameHandler gh = GetComponent<GameHandler>();
        gh.AddStatObserver(this);

        CreateFile();

        _textWriter.WriteLine("{0},{1}", DateTime.Now, gh.GetPlayers().Count);
    }

    public void NewMoveAdded(StoredPlayerMove move)
    {
        //Type | Time | Player | Direction
        _textWriter.WriteLine("{0},{1},{2},{3}", "Move", Time.realtimeSinceStartup, move.Player, move.Direction);
    }

    public void NewTradeActivity(PlayerTrade playerTrade, string status)
    {
        //Type | Time | ID | offering player | receiving player | direction | Status | Counter Offer (If Available)
        _textWriter.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7}",
            "Trade",
            Time.realtimeSinceStartup,
            playerTrade.TradeID,
            playerTrade.OfferingPlayer,
            playerTrade.ReceivingPlayer,
            playerTrade.DirectionOffer,
            status,
            playerTrade.DirectionCounterOffer);
    }

    private void CreateFile()
    {
        if (!Directory.Exists(_directoryPath))
        {
            Debug.Log($"{_directoryPath} does not exist, creating directory..");
            Directory.CreateDirectory(_directoryPath);
        }

        string filePath = fileCreationStrategy.CreateFile(_directoryPath);

        _textWriter = new StreamWriter(filePath);
    }

    private void OnApplicationQuit()
    {
        //Type | Time Elapsed | Nr of trades | Nr of Moves
        _textWriter.WriteLine("{0},{1},{2},{3}", "Summary:", Time.realtimeSinceStartup, _nrOfTrades, nrOfMoves);

        _textWriter.Flush();
        _textWriter.Close();
    }
}

public enum FileNameStrategy
{
    NumberBased,
    DateBased
}
