using System;
using CoreGame;
using UnityEngine;
using UnityEngine.UI;

namespace AdminGUI
{
    //Parent Class, not to be used directly
    public abstract class _PrimeButton : MonoBehaviour, IInventoryObserver
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

        protected abstract void GUIButtonPressed(Button button);
        

        protected void PlayerChange(PlayerTags newPlayerTags)
        {
            if (_playerController != null)
            {
                _playerController.RemoveInventoryObserver(this);
            }
            

            _playerController = GameHandler.current.GetPlayerController(newPlayerTags);
            _playerController.AddInventoryObserver(this);
            
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