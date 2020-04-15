using System;
using System.Collections.Generic;
using Container;
using CoreGame;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace AdminGUI
{
    public class OutgoingTradeListController : MonoBehaviour, ITradeObserver
    {
        private float _bottomPos;
        public GameObject OutGoingTradePrefab;

        [Header("Title")] public GameObject titleObj;

        [Header("Animation Values")] public float animateInTime;
        public float animateOutTime;
        public float animationRepositionTime;
        public LeanTweenType easeInType;
        public LeanTweenType easeOutType;
        public LeanTweenType repositionType;

        [Header("Settings")] [SerializeField] private float elementHeight = 30;
        public float spacing = 5;
        public float elementStartPos;

        //Other
        private GameObject _currentInLine;
        private bool _maxedOut;    //True if there is no space left for anymore outgoing trades

        //Other
        private PlayerController _playerController;
        private Dictionary<PlayerTrade, GameObject> ElementDictionary = new Dictionary<PlayerTrade, GameObject>();
        private List<PlayerTrade> Elements = new List<PlayerTrade>();


        private void Start()
        {
            GUIEvents.current.onPlayerChange += OnPlayerChange;
            //GUIEvents.current.OnTradeAction += OnTradeBtn;
            GetButtonPos();
        }

        private void OnPlayerChange(PlayerTags obj)
        {
            if (_playerController != null)
            {
                _playerController.RemoveTradeObserver(this);
            }

            _playerController = GameHandler.Current.GetPlayerController(obj);
            _playerController.AddTradeObserver(this);
        }

        private void GetButtonPos()
        {
            _bottomPos = transform.GetChild(1).localPosition.y;
            _currentInLine = transform.GetChild(1).gameObject;
        }

        /*private void OnTradeBtn(Button button, TradeActions tradeAction, Direction direction)
        {
            switch (tradeAction)
            {
                case TradeActions.TradeCanceled:
                    foreach (KeyValuePair<PlayerTrade, GameObject> element in ElementDictionary)
                    {
                        if (element.Key.TradeID == button.GetComponent<OutgoingTradeElement>().GetTradeId())
                        {
                            element.Key.CancelTrade(_playerController.playerTag);
                            return;
                        }
                    }

                    break;
                case TradeActions.TradeCanceledByGameHandler:
                    foreach (KeyValuePair<PlayerTrade, GameObject> element in ElementDictionary)
                    {
                        if (element.Key.TradeID == button.GetComponent<IncomingTradeElement>().GetTradeId())
                        {
                            element.Key.CancelTrade(GameHandler.Current);
                            return;
                        }
                    }

                    break;
                default:
                    return;
            }
        }*/

        private void AddElement(PlayerTrade trade)
        {
            _currentInLine.GetComponent<OutgoingTrade>().trade = trade;
            Elements.Add(trade);
            ElementDictionary.Add(trade, _currentInLine);

            GameObject listElement = Instantiate(OutGoingTradePrefab, transform, false);

            LeanTween.moveLocalX(listElement, elementStartPos, 0);
            LeanTween.moveLocalX(listElement, 0, animateInTime).setEase(easeInType);

            _currentInLine = listElement;
            _currentInLine.GetComponent<OutgoingTrade>().ManualSetup(_playerController);

            if (Elements.Count == 4)
            {
                _maxedOut = true;
                _currentInLine.GetComponent<Button>().interactable = false;
            }

            UpdateElementPositions();
        }

        private void RemoveElement(PlayerTrade trade)
        {
            GameObject element = ElementDictionary[trade];
            Elements.Remove(trade);
            ElementDictionary.Remove(trade);


            LeanTween.moveLocalX(element, elementStartPos, animateOutTime).setEase(easeOutType)
                .setOnComplete(() => Destroy(element.gameObject));

            if (_maxedOut)
            {
                _currentInLine.GetComponent<Button>().interactable = true;
            }
            
            UpdateElementPositions();
        }

        private void SetButtonValues(GameObject trade)
        {
            //Do Nothing   
        }


        public void OnNewTradeActivity(PlayerTrade playerTrade, TradeActions tradeAction)
        {
            if (playerTrade.OfferingPlayerTags == _playerController.playerTag)
            {
                switch (tradeAction)
                {
                    case TradeActions.TradeOffered:
                        AddElement(playerTrade);
                        break;
                    default:
                        RemoveElement(playerTrade);
                        break;
                }
            }
        }

        private void UpdateElementPositions()
        {
            LeanTween.moveLocalY(titleObj, (_bottomPos + elementHeight) + ((Elements.Count) * elementHeight) + (spacing * (Elements.Count)), animationRepositionTime).setEase(repositionType);

            if (Elements.Count == 0)
            {
                LeanTween.moveLocalX(titleObj, elementStartPos, animateOutTime).setEase(easeOutType);
            }
            else
            {
                LeanTween.moveLocalX(titleObj, 0, animateInTime).setEase(easeInType);
            }

            int i = 1;
            foreach (PlayerTrade trade in Elements)
            {
                LeanTween.moveLocalY(ElementDictionary[trade],
                    (_bottomPos + elementHeight + spacing) + ((Elements.Count - i) * elementHeight) + (spacing * (Elements.Count - i)),
                    animationRepositionTime).setEase(repositionType);
                i++;
            }
        }
    }
}