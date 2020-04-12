using CoreGame;
using UnityEngine;

namespace GameGUI.WaitingRoomScene
{
    public class HamburgerMenuController : MonoBehaviour
    {
        [Header("Animation Values")] 
        public float animationTime;
        public LeanTweenType easeInType;
        public LeanTweenType easeOutType;
        
        
        private bool _open;
        
        private void Open()
        {
            LeanTween.moveLocalY(gameObject, 0, animationTime).setEase(easeInType);
            _open = true;
        }

        private void Close()
        {
            LeanTween.moveLocalY(gameObject, 141.3f, animationTime).setEase(easeOutType);
            _open = false;
        }

        public void BtnHit()
        {
            if (_open)
            {
                Close();
            }
            else
            {
                Open();
            }
        }
    }
}