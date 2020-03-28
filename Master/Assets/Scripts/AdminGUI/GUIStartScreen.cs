using System;
using System.Collections;
using Michsky.UI.ModernUIPack;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AdminGUI
{
    public class GUIStartScreen : MonoBehaviour
    {
        [Header("Background Settings")]
        public GameObject background;
        public float backgroundSpeed;
        public LeanTweenType backgroundEase;
        
        [Header("sandGlass Settings")]
        public AnimatedIconHandler sandGlass;
        public float sandGlassSpeed;
        public LeanTweenType sandGlassEase;

        [Header("Error Message")] 
        public TextMeshProUGUI errorText;

        private bool animationRunning;
        private bool gameStarted;

        private void Start()
        {
            GUIEvents.current.onButtonHit += OnButtonHit;
            GUIEvents.current.onManualOverride += OnManualOverride;
            GUIEvents.current.onGameStart += OnGameStart;
        }

        private void OnButtonHit(Button button)
        {
            string key = button.name;

            if (key.Equals("StartGUI"))
            {
                if (!gameStarted && !animationRunning)
                {
                    animationRunning = true;
                    button.interactable = false;
                    LeanTween.moveLocalY(errorText.gameObject, 0, backgroundSpeed).setEase(backgroundEase);
                    LeanTween.rotateAroundLocal(button.gameObject, new Vector3(0, 0, 1), 5, 0.2f)
                        .setEase(LeanTweenType.easeShake)
                        .setRepeat(3)
                        .setOnComplete(() =>
                        {
                            LeanTween.moveLocalY(errorText.gameObject, -130, 0.3f).setDelay(2).setOnComplete(()=> animationRunning = false);
                            button.interactable = true;
                        });
                    
                }
                else if(gameStarted)
                {
                    button.interactable = false;
                    GUIEvents.current.OnManualControl();
                }
            }
        }

        private void OnGameStart()
        {
            gameStarted = true;
        }

        private void OnManualOverride()
        {
            
                LeanTween.moveLocalY(sandGlass.gameObject, 0, sandGlassSpeed).setEase(sandGlassEase).setOnComplete(() => sandGlass.ClickEvent());
                StartCoroutine(RemoveBackground());
        }

        private IEnumerator RemoveBackground()
        {
            yield return new WaitForSeconds(2);
            
            sandGlass.ClickEvent();
            LeanTween.moveLocalY(background, -1440, backgroundSpeed).setEase(backgroundEase).destroyOnComplete = true;
        }
    }
}