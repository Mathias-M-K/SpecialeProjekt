using System;
using CoreGame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AdminGUI
{
    //Parent Class, not to be used directly
    public abstract class _PrimeTradeElement : MonoBehaviour
    {
        public GameObject FirstChoice;
        
        [SerializeField] protected bool firstChoiceActive;
        
        protected int TradeId;

        protected virtual void Start()
        {
            GUIEvents.current.OnButtonHit += GUIButtonPressed;
        }

        protected abstract void GUIButtonPressed(Button button);
        
        protected void SetFirstChoiceActive()
        {
            LeanTween.moveLocalX(FirstChoice, 0, 0.5f).setEase(LeanTweenType.easeOutExpo).setEase(LeanTweenType.easeOutExpo);
            firstChoiceActive = true;
        }
        
        protected void SetFirstChoiceInactive()
        {
            LeanTween.moveLocalX(FirstChoice, 264, 0.5f).setEase(LeanTweenType.easeOutExpo).setEase(LeanTweenType.easeOutExpo);
            firstChoiceActive = false;
        }

        public void SetTradeId(int newTradeId)
        {
            TradeId = newTradeId;
        }

        public void SetName(string name)
        {
            transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
        }

        public int GetTradeId()
        {
            return TradeId;
        }
        
        public void ButtonPress(Button b)
        {
            GUIEvents.current.BtnHit(b);
        }

        
    }
}