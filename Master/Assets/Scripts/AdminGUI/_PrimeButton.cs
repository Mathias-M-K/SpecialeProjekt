using System;
using CoreGame;
using UnityEngine;

namespace AdminGUI
{
    //Parent Class, not to be used directly
    public abstract class _PrimeButton : MonoBehaviour, IMoveObserver
    {
        
        public bool enabledAndActive; //True after manual control have been initiated
        public bool arrowsActive;
        public GameObject arrows;
        
        [Header("Animation Speed")] [Range(0, 5)]
        public float animationSpeed;
        
        protected PlayerController _playerController;

        protected virtual void Start()
        {
            GUIEvents.current.onButtonHit += GUIButtonPressed;
            GUIEvents.current.onPlayerChange += PlayerChange;
            GUIEvents.current.onManualOverride += ManualControl;
        }
        
        private void ManualControl()
        {
            enabledAndActive = true;
        }

        protected abstract void GUIButtonPressed(string key);
        

        protected void PlayerChange(Player newPlayer)
        {
            if (_playerController != null)
            {
                _playerController.RemoveMoveObserver(this);
            }
            

            _playerController = GameHandler.current.GetPlayerController(newPlayer);
            _playerController.AddMoveObserver(this);
            
            GUIMethods.UpdateArrows(arrows.transform.GetChild(0),_playerController);
        }
        
        protected void SetArrowsActive()
        {
            LeanTween.moveLocalY(arrows, 0, animationSpeed).setEase(LeanTweenType.easeOutSine);
            arrowsActive = true;
        }

        protected void SetArrowsInactive()
        {
            LeanTween.moveLocalY(arrows, 300, animationSpeed).setEase(LeanTweenType.easeOutSine);
            arrowsActive = false;
        }

        public void OnMoveInventoryChange(Direction[] directions)
        {
            GUIMethods.UpdateArrows(arrows.transform.GetChild(0),_playerController);
        }
        
    }
}