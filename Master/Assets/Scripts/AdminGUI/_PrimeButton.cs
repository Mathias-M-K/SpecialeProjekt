using CoreGame;
using UnityEngine;

namespace AdminGUI
{
    public class _PrimeButton : MonoBehaviour, IMoveObserver
    {
        
        public bool enabledAndActive; //True after manual control have been initiated
        public bool arrowsActive;
        public GameObject arrows;
        
        [Header("Animation Speed")] [Range(0, 5)]
        public float animationSpeed;
        
        protected PlayerController _playerController;
        
        
        
        private void ManualControl()
        {
            enabledAndActive = true;
        }

        private void PlayerChange(Player newPlayer)
        {
            if (_playerController == null)
            {
                _playerController = GameHandler.current.GetPlayerController(newPlayer);
            }
            else
            {
                _playerController.RemoveMoveObserver(this);
            }

            _playerController = GameHandler.current.GetPlayerController(newPlayer);
            _playerController.AddMoveObserver(this);

            GUIMethods.UpdateArrows(arrows,_playerController);
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

        public void MoveInventoryUpdate(Direction[] directions)
        {
            GUIMethods.UpdateArrows(arrows,_playerController);
        }
        
    }
}