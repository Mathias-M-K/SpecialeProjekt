using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GameGUI
{
    public class WelcomeScreenController : MonoBehaviour
    {
        [Header("Scene Index")] 
        public int NetworkScene;
        public int LocalScene;

        [Header("Content")] 
        public GameObject mainContent;
        public float contentAnimationTime;
        public LeanTweenType contentEaseType;
        
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
                setEase(contentEaseType).
                setOnComplete(() => SceneManager.LoadScene(NetworkScene));
        }

        public void LocalGame()
        {
            SceneManager.LoadScene(LocalScene);
        }
    }
}