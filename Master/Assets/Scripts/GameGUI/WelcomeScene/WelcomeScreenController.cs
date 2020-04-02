using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GameGUI
{
    public class WelcomeScreenController : MonoBehaviour
    {
        [Header("Content")] 
        public GameObject mainContent;
        public float contentAnimationTime;
        public LeanTweenType contentEaseOutType;
        public LeanTweenType contentEaseInType;
        
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


        private void Start()
        {
            LeanTween.moveLocalX(mainContent, -1236, 0);
            LeanTween.moveLocalX(mainContent, 0, contentAnimationTime).setEase(contentEaseInType);
        }

        public void ButtonHit(Button b)
        {
            print(b.name);
            if (b.name.Equals("StartBtn"))
            {
                startBtn = b;
                b.interactable = false;
                LeanTween.moveLocalY(networkLocalButtons, 0, animationTime1).setEase(easeType1);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                if (!startBtn.interactable)
                {
                    startBtn.interactable = true;
                    LeanTween.moveLocalY(networkLocalButtons, 140, animationTime1).setEase(easeType1);
                }
            }
        }

        //Btn method
        public void OnlineGame()
        {
            LeanTween.moveLocalX(mainContent, -1236, contentAnimationTime).
                setEase(contentEaseOutType).
                setOnComplete(() => SceneManager.LoadScene(GlobalValues.networkScene));
        }

        public void LocalGame()
        {
            SceneManager.LoadScene(GlobalValues.gameScene);
        }
    }
}