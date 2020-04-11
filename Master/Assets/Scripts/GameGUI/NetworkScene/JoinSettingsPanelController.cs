using System;
using CoreGame;
using UnityEngine;

namespace GameGUI.NetworkScene
{
    public class JoinSettingsPanelController : MonoBehaviour
    {
        [Header("Animation Settings")] 
        public float animationTime;
        public LeanTweenType easeIn;
        public LeanTweenType easeOut;
        
        private bool _open;    
        
        private void Update()
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.U))
            {
                Open();
            }
        }

        public void Open()
        {
            if(_open) return;
            _open = true;
            LeanTween.moveLocalY(gameObject, -129.8f, animationTime).setEase(easeIn);
        }

        public void Close()
        {
            if (!_open) return;
            
            LeanTween.moveLocalY(gameObject, 0, animationTime).setEase(easeOut);

            _open = false;
        }
    }
}