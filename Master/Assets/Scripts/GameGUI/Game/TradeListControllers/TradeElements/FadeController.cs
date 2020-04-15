using System;
using System.Collections;
using CoreGame;
using UnityEngine;
using UnityEngine.UI;

namespace AdminGUI
{
    public class FadeController : MonoBehaviour
    {
        [Header("Settings")] 
        [SerializeField] private float upperLimit = 0.56f;
        [SerializeField] private float lowerLimit = 0;
        [SerializeField] private float speed = 0.02f;
        [SerializeField] private float delay = 0.03f;
        [SerializeField] private bool startAutomatically = false;

        [Header("Colors")] 
        public Color fadeColor;
        public Color pauseColor;
        
        
        private float _currentAlpha = 0;
        private Image _image;
        private bool _pause;

        private void Start()
        {
            _image = GetComponent<Image>();

            if (startAutomatically) StartFade();
        }

        private IEnumerator Fade()
        {
            _currentAlpha = _image.color.a;

            while (true && !_pause)
            {
                while (_currentAlpha < upperLimit && !_pause)
                {
                    _currentAlpha += speed;
                    Color col = _image.color;
                    
                    col.a = _currentAlpha;

                    _image.color = col;
                    
                    yield return new WaitForSeconds(delay);
                }
                while (_currentAlpha > lowerLimit && !_pause)
                {
                    _currentAlpha -= speed;
                    
                    Color col = _image.color;
                    
                    col.a = _currentAlpha;

                    _image.color = col;
                    
                    yield return new WaitForSeconds(delay);
                }
            }
        }

        public void SetPauseColor(Color newPauseColor)
        {
            pauseColor = newPauseColor;
        }

        public void ResetToWhite()
        {
            LeanTween.color(_image.rectTransform, new Color32(255, 255, 255, 110), 0.5f).setRecursive(false).setEase(LeanTweenType.easeInOutQuad);
        }
        
        public void StartFade()
        {
            Color col = fadeColor;
            col.a = _currentAlpha;
            
            LeanTween.color(_image.rectTransform, col, 0.2f).setRecursive(false).setEase(LeanTweenType.easeInOutQuad).setOnComplete(
                () =>
                {
                    _pause = false;
                    StartCoroutine(Fade());
                });
            
        }

        public void Pause()
        {
            LeanTween.color(_image.rectTransform, pauseColor, 0.4f).setRecursive(false).setEase(LeanTweenType.easeInOutQuad);
            _pause = true;
        }
    }
}