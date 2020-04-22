using System;
using System.IO;
using AdminGUI;
using Container;
using CoreGame.Interfaces;
using CoreGame.Strategies.Interfaces;
using DefaultNamespace;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace CoreGame
{
    public class StatTracker : MonoBehaviour, ISequenceObserver, ITradeObserver, IFinishPointObserver
    {
        [Header("Strategy")] public FileNameStrategy fileNameStrategy;
    
        [Header("Stats")]
        [SerializeField] private int nrOfMoves = 0;
        private int _nrOfTrades = 0;
        private string _directoryPath = "C:/MasterData/";
        private TextWriter _textWriter;
        private float _startTime;
        private bool _fileDone;

        public _FileNamingStrategy fileCreationStrategy;

        private void Start()
        {
            GUIEvents.current.OnGameStart += OnGameStart;
        }

        private void OnGameStart()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Destroy(gameObject);
                return;
            }

            _startTime = Time.realtimeSinceStartup;
            
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
            
            GameHandler.Current.AddSequenceObserver(this);
            GameHandler.Current.AddTradeObserver(this);
            GameHandler.Current.AddGameProgressObserver(this);
            
            GUIEvents.current.OnPlayerReady += OnReadyStateChanged;
            GUIEvents.current.OnGameDone += OnGameDone;
            
            CreateFile();

            _textWriter.WriteLine("{0},{1}", DateTime.Now, GameHandler.Current.GetPlayers().Count);
        }
        
        private void OnGameDone()
        {
            _textWriter.WriteLine("{0},{1},{2},{3}", "Summary:", GetTimeReadable(), _nrOfTrades, nrOfMoves);

            _textWriter.Flush();
            _textWriter.Close();

            _fileDone = true;
        }

        private float GetGameTime()
        {
            return Time.realtimeSinceStartup - _startTime;
        }

        private string GetTimeReadable()
        {
            TimeSpan ts = TimeSpan.FromSeconds(GetGameTime());

            return $"{ts.Hours:D2}:{ts.Minutes:D2}:{ts.Seconds:D2}";
        }

        public void OnSequenceChange(SequenceActions sequenceAction, StoredPlayerMove move)
        {
            //Type | Time | Player | Direction
            switch (sequenceAction)
            {
                case SequenceActions.NewMoveAdded:
                    //Type | Time | Player | Direction
                    _textWriter.WriteLine("{0},{1},{2},{3}",sequenceAction,GetTimeReadable(),move.PlayerTags,move.Direction);
                    nrOfMoves++;
                    break;
                case SequenceActions.MoveRemoved:
                    //Type | Time | Player | Direction
                    _textWriter.WriteLine("{0},{1},{2},{3}",sequenceAction,GetTimeReadable(),move.PlayerTags,move.Direction);
                    break;
                case SequenceActions.SequenceStarted:
                    //Type | Time 
                    _textWriter.WriteLine("{0},{1}",sequenceAction,GetTimeReadable());
                    break;
                case SequenceActions.SequenceEnded:
                    _textWriter.WriteLine("{0},{1}",sequenceAction,GetTimeReadable());
                    break;
                case SequenceActions.MovePerformed:
                    _textWriter.WriteLine("{0},{1},{2},{3}",sequenceAction,GetTimeReadable(),move.PlayerTags,move.Direction);
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
                GetTimeReadable(),
                playerTrade.TradeID,
                playerTrade.OfferingPlayerTags,
                playerTrade.ReceivingPlayerTags,
                playerTrade.DirectionOffer,
                tradeAction,
                playerTrade.DirectionCounterOffer);
        }
    
        public void OnGameProgressUpdate(PlayerTags playerTags)
        {
            //Type | Time | Player
            _textWriter.WriteLine("{0},{1},{2}","Player Finished",GetTimeReadable(),playerTags);
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
            if (_fileDone) return;
            
            //Type | Time Elapsed | Nr of trades | Nr of Moves
            _textWriter.WriteLine("{0},{1},{2},{3}", "Summary:", GetTimeReadable(), _nrOfTrades, nrOfMoves);

            _textWriter.Flush();
            _textWriter.Close();
        }

        public void OnObserverMark(string observer, float time)
        {
            TimeSpan ts = TimeSpan.FromSeconds(time);
            
            //Type | Time Elapsed | Observer | time
            _textWriter.WriteLine("{0},{1},{2},{3}", "Observer Mark:", GetTimeReadable(), observer,$"{ts.Hours:D2}:{ts.Minutes:D2}:{ts.Seconds:D2}");
        }

        public void OnPlayerDisconnect(string playerName, PlayerTags playerColor)
        {
            //Type | Time Elapsed | PlayerName | PlayerColor
            _textWriter.WriteLine("{0},{1},{2},{3}", "Disconnect", GetTimeReadable(), playerName,playerColor);
        }

        public void OnPlayerReconnect(string playerName, PlayerTags playerColor)
        {
            //Type | Time Elapsed | PlayerName | PlayerColor
            _textWriter.WriteLine("{0},{1},{2},{3}", "Reconnect", GetTimeReadable(), playerName,playerColor);
        }

        private void OnReadyStateChanged(bool state, PlayerTags player)
        {
            //Type | Time Elapsed | Player | State
            _textWriter.WriteLine("{0},{1},{2},{3}", "Ready State Changed:", GetTimeReadable(), player,state);
        }
    }

    public enum FileNameStrategy
    {
        NumberBased,
        DateBased
    }
}