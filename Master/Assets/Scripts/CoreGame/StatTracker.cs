using System;
using System.IO;
using Container;
using CoreGame;
using CoreGame.Interfaces;
using UnityEngine;

public class StatTracker : MonoBehaviour,IStatObserver
{
    [SerializeField] private int nrOfMoves;
    [SerializeField] private int nrOfTrades;
    private string _savePath = "C:/Users/mathi/desktop";
    private TextWriter tw;
     
    private void Start()
    {
        GetComponent<GameHandler>().AddStatObserver(this);
        tw = new StreamWriter(_savePath+"/data.csv");
    }

    public void NewMoveAdded()
    {
        nrOfMoves++;
        tw.WriteLine("{0},{1},{2}","Move",Time.realtimeSinceStartup,nrOfMoves.ToString("F2"));
    }

    public void NewTradeAdded(PlayerTrade playerTrade)
    {
        nrOfTrades++;
        
        //Type(trade,move) | Time | offering player | Receiving player | Offer(Move)
        tw.WriteLine("{0},{1},{2},{3},{4}","Trade",Time.realtimeSinceStartup,playerTrade.OfferingPlayer.ToString(),playerTrade.ReceivingPlayer.ToString(),playerTrade.Direction.ToString());
    }

    private void OnApplicationQuit()
    {
        tw.Flush();
        tw.Close();
    }
}
