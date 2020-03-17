using System;
using CoreGame;
using UnityEngine;

namespace AdminGUI
{
    public class TradeController : MonoBehaviour
    {
        [Range(1, 4)] public int tradeBtnNr;
        public GameObject FirstChoice;
        public GameObject SecondChoice;

        private bool firstChoiceActive;
        private bool secondChoiceActive;
        

        private void Start()
        {
            GUIEvents.current.onButtonHit += GUIButtonPressed;
        }

        private void GUIButtonPressed(string key)
        {
            if (key.Equals("IncomingTradeBtn"+tradeBtnNr))
            {
                if (!firstChoiceActive)
                {
                    if (secondChoiceActive)
                    {
                        SetSecondChoiseInactive();
                    }
                    else
                    {
                        SetFirstChoiceActive();
                    }
                }
                else
                {
                    SetFirstChoiseInactive();
                }
            }else if (key.Equals("AcceptBtn"))
            {
                if (firstChoiceActive)
                {
                    SetFirstChoiseInactive();
                    SetSecondChoiceActive();
                }
            }else if(key.Equals("RejectBtn"))
            {
                if (firstChoiceActive)
                {
                    SetFirstChoiseInactive();
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
                SetFirstChoiseInactive();
                SetSecondChoiseInactive();
            }
        }

        private void SetFirstChoiceActive()
        {
            LeanTween.moveLocalX(FirstChoice, 0, 0.2f);
            firstChoiceActive = true;
        }

        private void SetFirstChoiseInactive()
        {
            LeanTween.moveLocalX(FirstChoice, 264, 0.2f);
            firstChoiceActive = false;
        }
        
        private void SetSecondChoiceActive()
        {
            LeanTween.moveLocalX(SecondChoice, 0, 0.2f);
            secondChoiceActive = true;
        }

        private void SetSecondChoiseInactive()
        {
            LeanTween.moveLocalX(SecondChoice, 265, 0.2f);
            secondChoiceActive = false;
        }
        
        
    }
}