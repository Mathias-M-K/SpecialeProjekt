using System;
using System.Collections;
using System.Collections.Generic;
using Container;
using CoreGame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class TradeUi : MonoBehaviour, ITradeObserver
{
    [SerializeField] private TextMeshProUGUI[] buttons = new TextMeshProUGUI[5];
    public GameHandler gameHandler;
    private PlayerController _playerController;
    private List<PlayerTrade> trades = new List<PlayerTrade>();
    private Image _panel;
    public int panelValue;
    public int increment;
    public float speed;

    //Pending, Rejected, Accepted
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 1; i <= 5; i++)
        {
            buttons[i - 1] = transform.GetChild(i).GetComponent<TextMeshProUGUI>();
        }

        _panel = transform.GetChild(6).GetComponent<Image>();
        _playerController = gameHandler.GetPlayerController(Player.Red);
        _playerController.AddTradeObserver(this);

        UpdateList();
    }
    

    private IEnumerator GetAttention()
    {
        for (int i = 0; i < 6; i++)
        {
            while (panelValue < 250)
            {
                Color32 color = new Color32(0, 0, (byte) panelValue, 75);
                panelValue += increment;
                _panel.color = color;
                yield return new WaitForSeconds(speed);
                
            }

            while (panelValue > 50)
            {
                Color32 color = new Color32(0, 0, (byte) panelValue, 75);
                panelValue += -increment;
                _panel.color = color;
                yield return new WaitForSeconds(speed);
            }  
        }
       
    }

    private void UpdateList()
    {
        int i = 0;

        foreach (TextMeshProUGUI text in buttons)
        {
            text.text = "";
        }

        foreach (PlayerTrade trade in trades)
        {
            buttons[i].text = $"{trade.DirectionOffer} : {trade.Status}";
            i++;
        }
    }

    private void AddToList(PlayerTrade trade)
    {
        trade.Status = "Pending";
        trades.Add(trade);

        UpdateList();
    }

    private PlayerTrade GetTradeById(int id)
    {
        foreach (PlayerTrade trade in trades)
        {
            if (trade.TradeID == id)
            {
                return trade;
            }
        }

        return null;
    }

    private IEnumerator RemoveFromList(PlayerTrade playerTrade, TradeActions action)
    {
        PlayerTrade currentTrade = GetTradeById(playerTrade.TradeID);


        switch (action)
        {
            case TradeActions.TradeOffered:
                break;
            case TradeActions.TradeRejected:
                currentTrade.Status = "Rejected";
                break;
            case TradeActions.TradeAccepted:
                currentTrade.Status = $"Accepted! You got: {playerTrade.DirectionCounterOffer}";

                break;
            case TradeActions.TradeCanceled:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(action), action, null);
        }
        
        UpdateList();
        StartCoroutine(GetAttention());
        yield return new WaitForSeconds(10);
        trades.Remove(currentTrade);
        UpdateList();
    }

    public void TradeUpdate(PlayerTrade playerTrade, TradeActions tradeAction)
    {
        if (playerTrade.OfferingPlayer != Player.Red) return;


        switch (tradeAction)
        {
            case TradeActions.TradeOffered:
                print("TRADEOFFERED");
                AddToList(playerTrade);
                break;
            case TradeActions.TradeRejected:
                print("TRADEREJECTED");
                StartCoroutine(RemoveFromList(playerTrade, tradeAction));
                break;
            case TradeActions.TradeAccepted:
                print("TRADEACCEPTED");
                StartCoroutine(RemoveFromList(playerTrade, tradeAction));
                break;
            case TradeActions.TradeCanceled:
                print("TRADECANCELED");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(tradeAction), tradeAction, null);
        }
    }
}