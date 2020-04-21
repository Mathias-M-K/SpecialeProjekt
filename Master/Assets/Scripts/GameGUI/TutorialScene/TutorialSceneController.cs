using System;
using System.Collections;
using Michsky.UI.ModernUIPack;
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
        public ProgressBar progressBar;
        public GameObject content;
        private bool _videoLoaded;
        
        public VideoPlayer player;

        private void Start()
        {
            LeanTween.moveLocalY(content, -780, 0);
            LeanTween.moveLocalY(content, 0, 0.7f).setEase(LeanTweenType.easeOutQuad);
            
            progressBar.currentPercent = 0;
            StartCoroutine(StartLoadingAnimation(99));
            player.Prepare();
        }

        private void Update()
        {
            if (player.isPrepared)
            {
                progressBar.speed = 60;
            }

            if (player.isPlaying)
            {
                progressBar.gameObject.SetActive(false);
                title.SetActive(false);
                subtitle.SetActive(false);
                LeanTween.alpha(background.GetComponent<Image>().rectTransform, 0, 0.7f);
                LeanTween.alpha(overlay.GetComponent<Image>().rectTransform, 0, 0.7f);
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

                if (!player.isPrepared)
                {
                    subtitle.GetComponent<Text>().text = "Could not load video :(";
                    yield break;
                }
                player.Play();

            }
            progressBar.isOn = false;
        }
    }
}