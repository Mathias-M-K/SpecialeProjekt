using Container;
using CoreGame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AdminGUI.PanelControllers
{
    public class ObserverTradeElement : MonoBehaviour
    {
        public Image arrow;
        public Image counterMove;
        
        public TextMeshProUGUI offeringPlayer;
        public Image offeringPlayerColor;

        public TextMeshProUGUI receivingPlayer;
        public Image receivingPlayerColor;
        
        public int TradeId;
        
        
        
        public void SetInfo(PlayerTrade playerTrade)
        {
            TradeId = playerTrade.TradeID;
            offeringPlayer.text = playerTrade.OfferingPlayerTags.ToString();
            receivingPlayer.text = playerTrade.ReceivingPlayerTags.ToString();

            offeringPlayerColor.color = ColorPalette.current.GetPlayerColor(playerTrade.OfferingPlayerTags);
            receivingPlayerColor.color = ColorPalette.current.GetPlayerColor(playerTrade.ReceivingPlayerTags);

            arrow.color = ColorPalette.current.GetPlayerColor(playerTrade.OfferingPlayerTags);
            RotateArrow(playerTrade.DirectionOffer);
        }

        private void RotateArrow(Direction move)
        {
            int rotationZ = GlobalMethods.GetDirectionRotation(move);
            Vector3 rotation = new Vector3(0,0,rotationZ);

            Vector3 reverseRotation;

            if (rotationZ == 90 || rotationZ == -90)
            {
                reverseRotation = new Vector3(0,0,rotationZ*-1);
            }

            if (rotationZ == 180)
            {
                reverseRotation = new Vector3(0,0,0);
            }
            else
            {
                reverseRotation = new Vector3(0,0,180);
            }
      

            arrow.gameObject.transform.rotation = Quaternion.Euler(reverseRotation);
            
            LeanTween.rotateLocal(arrow.gameObject, rotation, 0.3f).setEase(LeanTweenType.easeOutSine);
        }
        
        public void RemoveTrade(TradeActions tradeAction, PlayerTrade trade)
        {
            if (tradeAction == TradeActions.TradeAccepted)
            {
                LeanTween.moveLocalX(arrow.gameObject, 43, 0.3f).setEase(LeanTweenType.easeInOutQuad);
                LeanTween.moveLocalX(counterMove.gameObject, 90, 0.5f).setEase(LeanTweenType.easeInQuad);
                counterMove.color = ColorPalette.current.GetPlayerColor(trade.ReceivingPlayerTags);
                LeanTween.rotateLocal(counterMove.gameObject,
                    new Vector3(0, 0, GlobalMethods.GetDirectionRotation(trade.DirectionCounterOffer)), 0);
                
                LeanTween.color(GetComponent<Image>().rectTransform, new Color32(120, 224, 143,255), 0.5f)
                    .setEase(LeanTweenType.easeInOutBounce).setRepeat(3).setRecursive(false).setOnComplete(()=>
                        {
                            LeanTween.moveLocalX(gameObject, -400, 0.4f).setEase(LeanTweenType.easeInQuad).setOnComplete(()=> Destroy(gameObject));
                        });
            }
            else
            {
                LeanTween.color(GetComponent<Image>().rectTransform, new Color32(229, 80, 57, 255), 0.5f)
                    .setEase(LeanTweenType.easeInOutBounce).setRepeat(3).setRecursive(false).setOnComplete(()=>
                    {
                        LeanTween.moveLocalX(gameObject, -400, 0.4f).setEase(LeanTweenType.easeInQuad).setOnComplete(()=> Destroy(gameObject));
                    });
            }
        }
    }
}