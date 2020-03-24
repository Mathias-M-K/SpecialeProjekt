using System;
using UnityEngine;
using UnityEngine.UI;

namespace AdminGUI
{
    public class GUIGateElement : MonoBehaviour
    {
        [SerializeField]private float animationTime = 2;
        [SerializeField] private LeanTweenType easeIn = LeanTweenType.easeInQuad;
        [SerializeField] private LeanTweenType easeOut = LeanTweenType.easeOutQuad;
        
        private Image image;
        private Color32 dimmerColor;
        private Color32 originalColor;



        private void Start()
        {
            image = GetComponent<Image>();
            State1();

            originalColor = image.color;
            dimmerColor = image.color*0.7f;
        }
        
        private void State1()
        {
            LeanTween.color(image.rectTransform, dimmerColor, animationTime).setEase(easeIn).setOnComplete(State2);
        }

        private void State2()
        {
            LeanTween.color(image.rectTransform, originalColor, animationTime).setEase(easeOut).setOnComplete(State1);
        }
        
    }
}