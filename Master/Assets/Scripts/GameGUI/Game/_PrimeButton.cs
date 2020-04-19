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
        public GameObject dropdownArrow;
        
        [Header("Animation Speed")] [Range(0, 5)]
        public float animationSpeed;
        
        protected PlayerController _playerController;

        protected virtual void Start()
        {
            GUIEvents.current.OnButtonHit += GUIButtonPressed;
            GUIEvents.current.onPlayerChange += PlayerChange;
            GUIEvents.current.OnManualOverride += ManualControl;
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
            

            _playerController = GameHandler.Current.GetPlayerController(newPlayerTags);
            _playerController.AddInventoryObserver(this);
            
            GlobalMethods.UpdateArrows(arrows.transform.GetChild(0),_playerController);
        }
        
        protected void SetArrowsActive()
        {
            LeanTween.rotate(dropdownArrow, new Vector3(0, 0, -91), animationSpeed).setEase(LeanTweenType.easeOutSine);
            LeanTween.moveLocalY(arrows, 0, animationSpeed).setEase(LeanTweenType.easeOutSine);
            arrowsActive = true;
        }

        protected void SetArrowsInactive()
        {
            LeanTween.rotate(dropdownArrow, new Vector3(0, 0, 91), animationSpeed).setEase(LeanTweenType.easeOutSine);
            LeanTween.moveLocalY(arrows, 55, animationSpeed).setEase(LeanTweenType.easeOutSine);
            arrowsActive = false;
        }

        public void OnMoveInventoryChange(Direction[] directions)
        {
            GlobalMethods.UpdateArrows(arrows.transform.GetChild(0),_playerController);
        }
        
    }
}