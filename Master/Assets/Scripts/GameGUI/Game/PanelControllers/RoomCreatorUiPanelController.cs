using System;
using CoreGame;
using Michsky.UI.ModernUIPack;
using UnityEngine;
using UnityEngine.UI;

namespace AdminGUI.PanelControllers
{
    public class RoomCreatorUiPanelController : MonoBehaviour
    {
        public GameObject spawnButton;
        public GameObject startGameBtn;
        
        private void Start()
        {
            GUIEvents.current.OnButtonHit += OnButtonHit;
        }

        private void OnButtonHit(Button button)
        {
            if (button.name.Equals("SpawnPlayers"))
            {
                LeanTween.moveLocalY(startGameBtn, 0, 0.4f).setEase(LeanTweenType.easeOutQuint);
            }

            if (button.name.Equals("StartGame"))
            {
                LeanTween.moveLocalY(startGameBtn, 54, 0.4f).setEase(LeanTweenType.easeInQuint);
            }

            if (button.name.Equals("TerminateGame"))
            {
                print("Terminate Game");
            }
        }
    }
}