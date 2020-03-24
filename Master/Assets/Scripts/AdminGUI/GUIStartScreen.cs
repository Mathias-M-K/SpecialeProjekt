using System;
using UnityEngine;
using UnityEngine.UI;

namespace AdminGUI
{
    public class GUIStartScreen : MonoBehaviour
    {
        public GameObject background;
        public GameObject currentPlayerPanel;

        private void Start()
        {
            GUIEvents.current.onManualOverride += OnManualOverride;
        }

        private void OnManualOverride()
        {
            float backgroundWidth = background.GetComponent<RectTransform>().sizeDelta.x;
            float backgroundHeight = background.GetComponent<RectTransform>().sizeDelta.y;
            float currentPlayerPanelWidth = currentPlayerPanel.GetComponent<RectTransform>().sizeDelta.x;
            float currentPlayerPanelHeight = currentPlayerPanel.GetComponent<RectTransform>().sizeDelta.y;
            
            LeanTween.value(backgroundWidth, currentPlayerPanelWidth, 1).setOnUpdate(SetWidth).setEase(LeanTweenType.easeOutExpo);
            LeanTween.value(backgroundHeight, currentPlayerPanelHeight, 1).setOnUpdate(SetHeight).setEase(LeanTweenType.easeOutExpo);

            LeanTween.moveX(background, 30, 0.5f).setEase(LeanTweenType.easeOutExpo);;
            LeanTween.moveY(background, backgroundHeight-70, 0.5f).setEase(LeanTweenType.easeOutExpo);;
        }

        private void SetWidth(float width)
        {
            background.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,width);
        }
        private void SetHeight(float width)
        {
            background.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,width);
        }
    }
}