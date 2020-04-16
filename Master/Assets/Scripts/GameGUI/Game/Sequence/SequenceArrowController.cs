using System;
using Container;
using CoreGame;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AdminGUI.Sequence
{
    public class SequenceArrowController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        /*
         * Very simple, when mouse is over the object, the object will change it's sprite, and then change it back when mouse is gone. 
         */

        public Sprite onMouseOverSprite;
        private Sprite _onMouseGoneSprite;

        private Button _button;
        private GameObject _cancelButton;
        [HideInInspector] public Image image;
        [HideInInspector] public int index;
        [HideInInspector] public StoredPlayerMove move;
        
        //Private variables
        private bool _mouseHoverEnabled;

        private void Start()
        {
            _cancelButton = transform.GetChild(1).gameObject;
            _button = GetComponent<Button>();
            image = transform.GetChild(0).GetComponent<Image>();
            _onMouseGoneSprite = image.sprite;
        }

        private void ChangeSprite(Sprite sprite)
        {
            LeanTween.alpha(image.rectTransform, 0, 0.3f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() =>
            {
                image.sprite = sprite;
                LeanTween.alpha(image.rectTransform, 1, 0.3f).setEase(LeanTweenType.easeInOutQuad);
            });
        }

        private void MouseEnterSpriteChange()
        {
            ChangeSprite(onMouseOverSprite);
        }
        private void MouseExitSpriteChange()
        {
            ChangeSprite(_onMouseGoneSprite);
        }

        private void MouseEnter()
        {
            LeanTween.alpha(image.rectTransform, 0.1f,0.3f);
            LeanTween.moveLocalY(_cancelButton, 0, 0.3f);
            LeanTween.scale(_cancelButton,new Vector3(1.5f,1.5f,1.5f), 0.3f);
        }

        private void MouseExit()
        {
            LeanTween.alpha(image.rectTransform, 1,0.3f);
            LeanTween.moveLocalY(_cancelButton, 39.5f, 0.3f);
            LeanTween.scale(_cancelButton,new Vector3(1,1,1), 0.3f);
        }

        public void OnArrowButtonPressed()
        {
            MouseExit();
            GameHandler.Current.RemoveMoveFromSequence(move);
            LeanTween.alpha(image.rectTransform, 1, 0.3f);
            MouseExitSpriteChange();
        }

        public void SetMouseHover(bool newBoolValue)
        {
            _mouseHoverEnabled = newBoolValue;
        }

        public void SetCancelButtonActive(bool newBoolValue)
        {
            _cancelButton.SetActive(newBoolValue);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_mouseHoverEnabled) return;
            //MouseEnterSpriteChange();
            MouseEnter();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_mouseHoverEnabled) return;
            //MouseExitSpriteChange();
            MouseExit();
        }
    }
}