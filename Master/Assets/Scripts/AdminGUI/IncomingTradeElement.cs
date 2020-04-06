using System;
using System.Runtime.InteropServices;
using Container;
using CoreGame;
using UnityEngine;
using UnityEngine.UI;

namespace AdminGUI
{
    public class IncomingTradeElement : _PrimeTradeElement, IInventoryObserver
    {
        public GameObject SecondChoice;
        [SerializeField]private bool secondChoiceActive;



        protected override void Start()
        {
            base.Start();
            GUIEvents.current.onPlayerChange += OnPlayerChange;
        }

        private void OnPlayerChange(PlayerTags newPlayerTags)
        {
            if (_playerController != null)
            {
                _playerController.RemoveInventoryObserver(this);
            }
            
            _playerController = GameHandler.current.GetPlayerController(newPlayerTags);
            _playerController.AddInventoryObserver(this);
            
            GUIMethods.UpdateArrows(SecondChoice.transform,_playerController);
        }
        
        protected override void GUIButtonPressed(Button button)
        {
            string key = button.name;
            if (key.Equals(name))
            {
                if (!firstChoiceActive)
                {
                    if (secondChoiceActive)
                    {
                        SetSecondChoiceInactive();
                    }
                    else
                    {
                        SetFirstChoiceActive();
                    }
                }
                else
                {
                    SetFirstChoiceInactive();
                }
            }else if (key.Equals("AcceptBtn"))
            {
                if (firstChoiceActive)
                {
                    SetFirstChoiceInactive();
                    SetSecondChoiceActive();
                }
                
            }else if(key.Equals("RejectBtn"))
            {
                if (firstChoiceActive)
                {
                    GUIEvents.current.TradeActionNotify(gameObject.GetComponent<Button>(),TradeActions.TradeRejected,Direction.Blank);
                    SetFirstChoiceInactive();
                }
            }else if (key.Substring(0, 5).Equals("Arrow"))
            {
                if (secondChoiceActive)
                {
                    int.TryParse(key.Substring(5, key.Length - 5), out int indexFetchValue);
                    Direction d = _playerController.GetMoves()[indexFetchValue];
                    
                    GUIEvents.current.TradeActionNotify(gameObject.GetComponent<Button>(),TradeActions.TradeAccepted,d);
                    
                }
            }
            else
            {
                SetFirstChoiceInactive();
                SetSecondChoiceInactive();
            }
        }

        private void SetSecondChoiceActive()
        {
            LeanTween.moveLocalX(SecondChoice, 0, 0.5f).setEase(LeanTweenType.easeOutExpo).setEase(LeanTweenType.easeOutExpo);
            secondChoiceActive = true;
        }

        private void SetSecondChoiceInactive()
        {
            LeanTween.moveLocalX(SecondChoice, 265, 0.5f).setEase(LeanTweenType.easeOutExpo).setEase(LeanTweenType.easeOutExpo);
            secondChoiceActive = false;
        }

        public void OnMoveInventoryChange(Direction[] directions)
        {
            GUIMethods.UpdateArrows(SecondChoice.transform,_playerController);
        }
    }
}