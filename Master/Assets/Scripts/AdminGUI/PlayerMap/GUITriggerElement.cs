using System;
using UnityEngine;
using UnityEngine.UI;

namespace AdminGUI.PlayerMap
{
    public class GUITriggerElement : MonoBehaviour
    {
        private float animationTime = 1;
        private Image image;
        private Color32 color;



        private void Start()
        {
            image = GetComponent<Image>();
            State1();

            color = image.color*1.2f;
            
        }
        

        private void State1()
        {
            LeanTween.color(image.rectTransform, color, animationTime).setEase(LeanTweenType.punch).setOnComplete(State1);
        }

    }
}