using System;
using Container;
using CoreGame;
using CoreGame.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace AdminGUI
{
    public class SendBtnController : MonoBehaviour, IMoveObserver
    {
        public bool enabledAndActive; //True after manual control have been initiated
        public bool arrowsActive;
        public GameObject arrows;

        [Header("Animation Speed")] [Range(0, 5)]
        public float animationSpeed;


        protected PlayerController _playerController;


        private void Start()
        {
            GUIEvents.current.onButtonHit += GUIButtonPressed;
            GUIEvents.current.onPlayerChange += PlayerChange;
            GUIEvents.current.onManualOverride += ManualControl;
            //GameHandler.current.AddSequenceObserver(this);
            ExternalStartMethod();
        }

        protected virtual void ExternalStartMethod()
        {
            
        }

        private void ManualControl()
        {
            enabledAndActive = true;
        }

        private void PlayerChange(Player p)
        {
            if (_playerController == null)
            {
                _playerController = GameHandler.current.GetPlayerController(p);
            }
            else
            {
                _playerController.RemoveMoveObserver(this);
            }

            _playerController = GameHandler.current.GetPlayerController(p);
            _playerController.AddMoveObserver(this);

            GUIMethods.UpdateArrows(arrows,_playerController);
        }

        

        protected virtual void GUIButtonPressed(string key)
        {
            if (!enabledAndActive) return;

            if (key.Equals("SendBtn"))
            {
                if (!arrowsActive)
                {
                    SetArrowsActive();
                }
                else
                {
                    SetArrowsInactive();
                }
            }
            else if (key.Substring(0, 5).Equals("Arrow"))
            {
                if (arrowsActive)
                {
                    int.TryParse(key.Substring(5, key.Length - 5), out int indexFetchValue);
                    Direction directionToSend = _playerController.GetMoves()[indexFetchValue];
                    GameHandler.current.AddMoveToSequence(_playerController.player, directionToSend,
                        _playerController.GetIndexForDirection(directionToSend));
                    PlayerChange(_playerController.player);
                }
            }
            else
            {
                SetArrowsInactive();
            }
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