using System;
using System.IO;
using Container;
using CoreGame.Interfaces;
using CoreGame.Strategies.Interfaces;
using DefaultNamespace;
using UnityEngine;

namespace CoreGame
{
    public class StatTracker : MonoBehaviour, ISequenceObserver, ITradeObserver, IFinishPointObserver
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
            gh.AddSequenceObserver(this);
            gh.AddTradeObserver(this);
            gh.AddGameProgressObserver(this);

            CreateFile();

            _textWriter.WriteLine("{0},{1}", DateTime.Now, gh.GetPlayers().Count);
        }

        public void OnSequenceChange(SequenceActions sequenceAction, StoredPlayerMove move)
        {
            //Type | Time | Player | Direction
            switch (sequenceAction)
            {
                case SequenceActions.NewMoveAdded:
                    //Type | Time | Player | Direction
                    _textWriter.WriteLine("{0},{1},{2},{3}",sequenceAction,Time.realtimeSinceStartup,move.Player,move.Direction);
                    nrOfMoves++;
                    break;
                case SequenceActions.MoveRemoved:
                    //Type | Time | Player | Direction
                    _textWriter.WriteLine("{0},{1},{2},{3}",sequenceAction,Time.realtimeSinceStartup,move.Player,move.Direction);
                    break;
                case SequenceActions.SequenceStarted:
                    //Type | Time 
                    _textWriter.WriteLine("{0},{1}",sequenceAction,Time.realtimeSinceStartup);
                    break;
                case SequenceActions.SequenceEnded:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(sequenceAction), sequenceAction, null);
            }
        }
    
        public void OnNewTradeActivity(PlayerTrade playerTrade, TradeActions tradeAction)
        {
            if (tradeAction == TradeActions.TradeOffered) _nrOfTrades++;
            //Type | Time | ID | offering player | receiving player | direction | Status | Counter Offer (If Available)
            _textWriter.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7}",
                "Trade",
                Time.realtimeSinceStartup,
                playerTrade.TradeID,
                playerTrade.OfferingPlayer,
                playerTrade.ReceivingPlayer,
                playerTrade.DirectionOffer,
                tradeAction,
                playerTrade.DirectionCounterOffer);
        }
    
        public void OnGameProgressUpdate(Player player)
        {
            //Type | Time | Player
            _textWriter.WriteLine("{0},{1},{2}","Player Finished",Time.realtimeSinceStartup,player);
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
}