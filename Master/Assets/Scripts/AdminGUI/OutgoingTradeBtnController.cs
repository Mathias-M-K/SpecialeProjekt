using UnityEngine.UI;

namespace AdminGUI
{
    public class OutgoingTradeBtnController : IncomingTradeController
    {
        
        protected override void GUIButtonPressed(string key)
        {
            if (key.Equals("OutgoingTradeBtn" + btnNr))
            {
                if (!firstChoiceActive) 
                {
                       SetFirstChoiceActive();
                }
                else
                {
                    SetFirstChoiceInactive();
                }
            }else if (key.Equals("CancelBtn"))
            {
                if (firstChoiceActive)
                {
                    GUIEvents.current.TradeActionNotify(gameObject.GetComponent<Button>(),TradeActions.TradeCanceled);
                }
            }
            else
            {
                SetFirstChoiceInactive();
            }
        }
    }
}