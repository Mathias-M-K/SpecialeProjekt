using System;
using AdminGUI;
using AdminGUI;
using UnityEngine;
using UnityEngine.UI;

namespace CoreGame
{
    public class ColorPalette : MonoBehaviour
    {
        public static ColorPalette current;
        
        private void Awake()
        {
            current = this;
        }

        [Header("Player Colors")] 
        public Color32 playerRed;
        public Color32 playerBlue;
        public Color32 playerGreen;
        public Color32 playerYellow;
        
        [Header(("Red Button"))]
        public ColorBlock redButton;
        
        [Header(("green Button"))]
        public ColorBlock greenButton;

        public Color32 GetPlayerColor(Player player)
        {
            switch (player)
            {
                case Player.Red:
                    return playerRed;
                case Player.Blue:
                    return playerBlue;
                case Player.Green:
                    return playerGreen;
                case Player.Yellow:
                    return playerYellow;
                default:
                    throw new ArgumentOutOfRangeException(nameof(player), player, null);
            }
        }
    }
}