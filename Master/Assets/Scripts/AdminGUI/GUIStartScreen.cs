using System;
using System.Collections;
using Michsky.UI.ModernUIPack;
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

        private void Start()
        {
            GUIEvents.current.onManualOverride += OnManualOverride;
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
            LeanTween.moveLocalY(background, -Screen.height, backgroundSpeed).setEase(backgroundEase);
        }
    }
}