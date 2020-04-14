using System;
using System.Runtime.InteropServices;
using Container;
using CoreGame;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

namespace AdminGUI
{
    public class IncomingTradeElement : _PrimeTradeElement, IInventoryObserver
    {
        public GameObject SecondChoice;
        [SerializeField] private bool secondChoiceActive;
        public GameObject arrow;
        
        private PlayerController _playerController;
        
        protected override void Start()
        {
            base.Start();
            GetComponent<Button>().onClick.AddListener(OnTradeElementClick);
        }

        private void OnTradeElementClick()
        {
            print(TradeId);
        }

        public void SetPlayerController(PlayerTags newPlayerTags)
        {
            if (_playerController != null)
            {
                _playerController.RemoveInventoryObserver(this);
            }
            
            _playerController = GameHandler.Current.GetPlayerController(newPlayerTags);
            _playerController.AddInventoryObserver(this);
            
            GlobalMethods.UpdateArrows(SecondChoice.transform,_playerController);
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
            arrow.SetActive(false);
            LeanTween.moveLocalX(SecondChoice, 0, 0.5f).setEase(LeanTweenType.easeOutExpo).setEase(LeanTweenType.easeOutExpo);
            secondChoiceActive = true;
        }

        private void SetSecondChoiceInactive()
        {
            arrow.SetActive(true);
            LeanTween.moveLocalX(SecondChoice, 265, 0.5f).setEase(LeanTweenType.easeOutExpo).setEase(LeanTweenType.easeOutExpo);
            secondChoiceActive = false;
        }

        public void OnMoveInventoryChange(Direction[] directions)
        {
            GlobalMethods.UpdateArrows(SecondChoice.transform,_playerController);
        }

        private void OnDestroy()
        {
            print("Removing from list");
            GUIEvents.current.OnButtonHit -= GUIButtonPressed;
            if(_playerController) _playerController.RemoveInventoryObserver(this);
        }
    }
}