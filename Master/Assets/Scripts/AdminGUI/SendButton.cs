using System;
using Container;
using CoreGame;
using CoreGame.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace AdminGUI
{
    public class SendButton : _PrimeButton
    {
        protected override void GUIButtonPressed(Button button)
        {
            string key = button.name;
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
                    GameHandler.current.AddMoveToSequence(_playerController.player, directionToSend, indexFetchValue);
                    
                    PlayerChange(_playerController.player);
                }
            }
            else
            {
                SetArrowsInactive();
            }
        }
    }
}