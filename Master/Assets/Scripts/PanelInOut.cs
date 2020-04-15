using CoreGame;
using UnityEngine;

namespace AdminGUI
{
    public class PanelInOut : MonoBehaviour
    {
        [Header("Positions")] 
        [SerializeField] private Vector3 hidePos = new Vector3(0,0,0);
        [SerializeField] private Vector3 showPos = new Vector3(0,0,0);
        
        [Header("Animation Values")] 
        [SerializeField] private float animationTime = 0.5f;
        [SerializeField] private LeanTweenType easeIn = LeanTweenType.easeOutQuad;
        [SerializeField] private LeanTweenType easeOut = LeanTweenType.easeInQuad;
        
        //Private Values
        private bool _hidden = true;

        public void Show()
        {
            _hidden = false;
            LeanTween.moveLocalY(gameObject, showPos.y,animationTime).setEase(easeIn);
        }
        
        public void Hide()
        {
            _hidden = true;
            LeanTween.moveLocalY(gameObject, hidePos.y,animationTime).setEase(easeOut);
        }

        public void Toggle()
        {
            print($"Show: {showPos}");
            if (_hidden)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }
    }
}