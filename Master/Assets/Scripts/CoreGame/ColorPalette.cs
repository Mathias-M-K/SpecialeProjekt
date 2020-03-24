using System;
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

        [Header("Player Colors")] public Color32 playerRed;
        public Color32 playerBlue;
        public Color32 playerGreen;
        public Color32 playerYellow;

        [Header("Scene Elements")] public Color32 floorColor;
        public Color32 wallsColor;
        public Color32 finishPointColor;

        [Header(("Yellow Button"))] public ColorBlock yellowButton;

        [Header(("Green Button"))] public ColorBlock greenButton;


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
        public Color32 GetPlayerColor(string firstLetter)
        {
            switch (firstLetter)
            {
                case "r":
                    return playerRed;
                case "b":
                    return playerBlue;
                case "g":
                    return playerGreen;
                case "y":
                    return playerYellow;
                default:
                    throw new ArgumentException($"{firstLetter} is not recognized");
            }
        }
        
    }
}