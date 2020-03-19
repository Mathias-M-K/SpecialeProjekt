using CoreGame;
using UnityEngine;

namespace AdminGUI
{
    //Parent Class, not to be used directly
    public abstract class _PrimeTradeElement : MonoBehaviour
    {
        [Range(1, 4)] public int btnNr;
        public GameObject FirstChoice;
        
        [SerializeField] protected bool firstChoiceActive;
        
        protected PlayerController _playerController;

        protected virtual void Start()
        {
            GUIEvents.current.onButtonHit += GUIButtonPressed;
        }

        protected abstract void GUIButtonPressed(string key);
        
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
    }
}