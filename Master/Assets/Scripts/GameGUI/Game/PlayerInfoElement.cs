using CoreGame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AdminGUI
{
    public class PlayerInfoElement : MonoBehaviour
    {
        public Image playerColor;
        public TextMeshProUGUI text;

        public void SetInfo(string playerName, PlayerTags playerTag)
        {
            playerColor.color = ColorPalette.current.GetPlayerColor(playerTag);
            text.text = $"{playerName} | {playerTag}";

        }
    }
}