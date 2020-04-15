using System;
using System.Net.Configuration;
using Container;
using CoreGame;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AdminGUI
{
    public class IncomingTrade : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler, IInventoryObserver
    {
        [Header("Panels")] 
        public GameObject arrowPanel;
        public GameObject tradePanel;

        [Header("Info")] 
        public Image incomingDirection;
        public Image offeringPlayerBorder;

        public Image counterOfferImage;
        public Image receivingPlayerBorder;

        public TextMeshProUGUI text;

        [Header("Buttons")] 
        public GameObject acceptButton;
        public GameObject rejectButton;

        [Header("Sprites")] 
        public Sprite directionArrowSprite;

        //Private variables
        private bool _arrowPanelOut;
        private bool _rejectButtonHideOnMouseExit = true;
        
        //Info
        private PlayerTrade _trade;
        private PlayerController _playerController;
        private Direction _counterOffer;

        public void SetInfo(PlayerTrade trade, PlayerController playerController)
        {
            _trade = trade;
            _playerController = playerController;
            GlobalMethods.UpdateArrows(arrowPanel.transform,_playerController);
            _playerController.AddInventoryObserver(this);

            text.text = $"{_trade.OfferingPlayerTags}\nTrading\n{_trade.DirectionOffer}";

            offeringPlayerBorder.color = ColorPalette.current.GetPlayerColor(_trade.OfferingPlayerTags);
            receivingPlayerBorder.color = ColorPalette.current.GetPlayerColor(_trade.ReceivingPlayerTags);
            
            Vector3 rotation = new Vector3(0,0,GlobalMethods.GetDirectionRotation(_trade.DirectionOffer));
            LeanTween.rotateLocal(incomingDirection.gameObject, rotation, 0.3f).setEase(LeanTweenType.easeOutSine);
        }


        public void ShowArrows()
        {
            _arrowPanelOut = true;
            LeanTween.moveLocalX(arrowPanel, 0, 0.5f).setEase(LeanTweenType.easeOutQuad);
        }

        private void HideArrows()
        {
            _arrowPanelOut = false;
            LeanTween.moveLocalX(arrowPanel, 226.7f, 0.5f).setEase(LeanTweenType.easeInQuad);
        }
        
        public void RejectButtonHit()
        {
            if (_arrowPanelOut)
            {
                HideArrows();
                return;
            }
            
            _trade.RejectTrade(_playerController);
        }

        public void AcceptButton()
        {
            _trade.AcceptTrade(_counterOffer,_playerController);
        }

        public void OnArrowButtonHit(int nr)
        {
            counterOfferImage.sprite = directionArrowSprite;
            
            Vector3 rotation = new Vector3(0,0,GlobalMethods.GetDirectionRotation(_playerController.GetMoves()[nr]));
            LeanTween.rotateLocal(counterOfferImage.gameObject, rotation, 0);

            _counterOffer = _playerController.GetMoves()[nr];
            
            HideArrows();
            
            //Moving buttons into pos
            LeanTween.moveLocalX(rejectButton, -10, 0.4f).setEase(LeanTweenType.easeOutQuad);
            
            //LeanTween.moveLocalY(rejectButton, 17, 0.4f).setEase(LeanTweenType.easeOutQuad);
            
            LeanTween.moveLocalX(acceptButton, 17, 0.4f).setEase(LeanTweenType.easeOutQuad);
            
            LeanTween.moveLocalX(tradePanel, 15, 0.4f).setEase(LeanTweenType.easeInOutQuad);
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_rejectButtonHideOnMouseExit) LeanTween.moveLocalX(rejectButton, 17, 0.4f).setEase(LeanTweenType.easeOutQuad);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_rejectButtonHideOnMouseExit) LeanTween.moveLocalX(rejectButton, 46, 0.4f).setEase(LeanTweenType.easeInQuad);
        }
        
        public void OnMoveInventoryChange(Direction[] directions)
        {
            GlobalMethods.UpdateArrows(arrowPanel.transform,_playerController);
        }

        private void OnDestroy()
        {
            if(_playerController != null) _playerController.RemoveInventoryObserver(this);
        }
    }
}