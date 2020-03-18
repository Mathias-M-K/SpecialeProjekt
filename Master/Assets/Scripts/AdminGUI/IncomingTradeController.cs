using System;
using Container;
using CoreGame;
using UnityEngine;

namespace AdminGUI
{
    public class IncomingTradeController : MonoBehaviour, ITradeObserver
    {
        [Range(1, 4)] public int btnNr;
        public GameObject FirstChoice;
        public GameObject SecondChoice;

        protected bool firstChoiceActive;
        private bool secondChoiceActive;

        private PlayerController _playerController;
        

        private void Start()
        {
            GUIEvents.current.onButtonHit += GUIButtonPressed;
        }
        
        protected virtual void GUIButtonPressed(string key)
        {
            if (key.Equals("IncomingTradeBtn"+btnNr))
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
                    SetFirstChoiceInactive();
                }
            }else if (key.Substring(0, 5).Equals("Arrow"))
            {
                if (secondChoiceActive)
                {
                    print("TradeCompleted");
                }
            }
            else
            {
                SetFirstChoiceInactive();
                SetSecondChoiceInactive();
            }
        }
        
        protected void SetFirstChoiceActive()
        {
            LeanTween.moveLocalX(FirstChoice, 0, 0.5f).setEase(LeanTweenType.easeOutExpo);
            firstChoiceActive = true;
        }

        protected void SetFirstChoiceInactive()
        {
            LeanTween.moveLocalX(FirstChoice, 264, 0.5f).setEase(LeanTweenType.easeOutExpo);
            firstChoiceActive = false;
        }
        
        private void SetSecondChoiceActive()
        {
            LeanTween.moveLocalX(SecondChoice, 0, 0.5f).setEase(LeanTweenType.easeOutExpo);
            secondChoiceActive = true;
        }

        private void SetSecondChoiceInactive()
        {
            LeanTween.moveLocalX(SecondChoice, 265, 0.5f).setEase(LeanTweenType.easeOutExpo);
            secondChoiceActive = false;
        }
        
        public void TradeUpdate(PlayerTrade playerTrade, TradeActions tradeAction)
        {
            throw new NotImplementedException();
        }
    }
}