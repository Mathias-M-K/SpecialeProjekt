using System;
using System.Collections;
using System.Runtime.InteropServices;
using Michsky.UI.ModernUIPack;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace GameGUI.TutorialScene
{
    public class TutorialSceneController : MonoBehaviour
    {
        [Header("UI elements")]
        public GameObject background;
        public GameObject overlay;
        public GameObject title;
        public GameObject subtitle;
        public GameObject infoText;
        public GameObject auLogo;
        public GameObject auSegl;
        public ProgressBar progressBar;
        
        [Header("Content")]
        public GameObject content;
        
        [Header("Video Player")]
        public VideoPlayer player;
        
        //Other
        private bool _videoLoaded;
        private bool _loadingBarDone;
        


        private void Start()
        {
            LeanTween.moveLocalY(content, -780, 0);
            LeanTween.moveLocalY(content, 0, 0.7f).setEase(LeanTweenType.easeOutQuad);
            
            progressBar.currentPercent = 0;
            StartCoroutine(StartLoadingAnimation(99));
            player.Prepare();
            
            player.prepareCompleted += OnPrepareComplete;
            player.started += OnPlayerStarted;
        }

        private void OnPlayerStarted(VideoPlayer source)
        {
            print("Player started");
            
            //Removing the logos, since they are on the movie
            auLogo.SetActive(false);
            auSegl.SetActive(false);
            title.SetActive(false);
            subtitle.SetActive(false);
            
            LeanTween.moveLocalX(infoText, 1637, 0.4f).setEase(LeanTweenType.easeInQuad);
            
            LeanTween.alpha(background.GetComponent<Image>().rectTransform, 0, 0.7f);
            LeanTween.alpha(overlay.GetComponent<Image>().rectTransform, 0, 0.7f);
        }
        private void OnPrepareComplete(VideoPlayer source)
        {
            print("Player prepared!");
            _videoLoaded = true;
            progressBar.speed = 60;
            player.StepForward();
        }

        private void ShowVideoPlayerTutorial()
        {
            subtitle.GetComponent<TextMeshProUGUI>().text = "Video ready!";
            
            LeanTween.moveLocalX(infoText, 0, 0.5f).setEase(LeanTweenType.easeOutQuad);

            LeanTween.moveLocalX(progressBar.gameObject, 1400, 0.5f).setEase(LeanTweenType.easeInQuad);
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && _videoLoaded && _loadingBarDone)
            {
                if (player.isPlaying)
                {
                    player.Pause();
                }
                else
                {
                    player.Play();
                }
            }
        }
        
        private IEnumerator StartLoadingAnimation(int targetValue)
        {
            progressBar.isOn = true;
            if (targetValue < progressBar.currentPercent)
            {
                progressBar.invert = true;
                while (progressBar.currentPercent > targetValue+1)
                {
                    yield return null;
                }
            }
            else
            {
                progressBar.invert = false;
                while (progressBar.currentPercent < targetValue)
                {
                    yield return null;
                }
                progressBar.isOn = false;
                
                
                _loadingBarDone = true;
                
                if (!player.isPrepared)
                {
                    subtitle.GetComponent<TextMeshProUGUI>().text = "Oh no, there seems to be an error loading the tutorial video!";
                }
                else
                {
                    ShowVideoPlayerTutorial();
                }

                

            }
            progressBar.isOn = false;
        }
    }
}