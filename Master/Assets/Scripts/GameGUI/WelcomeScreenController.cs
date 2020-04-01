using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameGUI
{
    public class WelcomeScreenController : MonoBehaviour
    {
        [Header("Buttons")] 
        public Button startBtn;
        public Button OnlineBtn;
        public Button LocalBtn;

        public Button CreateRoomBtn;
        public Button JoinRoomBtn;

        [Header("Network / Local")] 
        public GameObject networkLocalButtons;
        public float animationTime1;
        public LeanTweenType easeType1;


        [Header("Online")] 
        public GameObject createJoinButtons;
        public float animationTime2;
        public LeanTweenType easeType2;

        [Header("Local")] 
        public GameObject StartGameBtn;
        public float animationTime3;
        public LeanTweenType easeType3;


        public void ButtonHit(Button b)
        {
            print(b.name);
            if (b.name.Equals("StartBtn"))
            {
                startBtn = b;
                b.interactable = false;
                LeanTween.moveLocalY(networkLocalButtons, 0, animationTime1).setEase(easeType1);
            }

            if (b.name.Equals("OnlineBtn"))
            {
                b.interactable = false;
                LocalChoicesClose();
                    
                LeanTween.moveLocalX(createJoinButtons, -84, animationTime2).setEase(easeType2);
            }
            if (b.name.Equals("LocalBtn"))
            {
                b.interactable = false;
                OnlineChoicesClose();

                LeanTween.moveLocalX(StartGameBtn, 0, animationTime3).setEase(easeType3);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                if (!OnlineBtn.interactable)
                {
                    OnlineChoicesClose();
                    return;
                }

                if (!LocalBtn.interactable)
                {
                    LocalChoicesClose();
                    return;
                }
                if (!startBtn.interactable)
                {
                    startBtn.interactable = true;
                    LeanTween.moveLocalY(networkLocalButtons, 140, animationTime1).setEase(easeType1);
                }
            }
        }

        private void OnlineChoicesClose()
        {
            OnlineBtn.interactable = true;
            LeanTween.moveLocalX(createJoinButtons, -512, animationTime2).setEase(easeType2);
        }

        private void LocalChoicesClose()
        {
            LocalBtn.interactable = true;
            LeanTween.moveLocalX(StartGameBtn, -235, animationTime3).setEase(easeType3);
        }
    }
}