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
        
        [Header("Player Colors")] 
        public Color32 playerRed;
        public Color32 playerBlue;
        public Color32 playerGreen;
        public Color32 playerYellow;
        public Color32 playerBlack;
        public Color32 playerSand;
        public Color32 playerPink;
        public Color32 playerGray;
        public Color32 playerPurple;
        public Color32 playerOrange;
        public Color32 playerNavyBlue;
        public Color32 playerDarkGreen;
        public Color32 playerLightBlue;
        public Color32 playerTurquoise;
        public Color32 playerWhite;
        public Color32 playerLightGreen;

        [Header("Role Colors")] 
        public Color32 participant;
        public Color32 observer;
        public Color32 host;

        [Header("Scene Elements")] public Color32 floorColor;
        public Color32 wallsColor;
        public Color32 finishPointColor;

        [Header(("Yellow Button"))] public ColorBlock yellowButton;

        [Header(("Green Button"))] public ColorBlock greenButton;


        public Color32 GetPlayerColor(PlayerController playerController)
        {
            return GetPlayerColor(playerController.playerTag);
        }
        public Color32 GetPlayerColor(PlayerTags playerTags)
        {
            switch (playerTags)
            {
                case PlayerTags.Red:
                    return playerRed;
                case PlayerTags.Blue:
                    return playerBlue;
                case PlayerTags.Green:
                    return playerGreen;
                case PlayerTags.Yellow:
                    return playerYellow;
                case PlayerTags.Black:
                    return playerBlack;
                case PlayerTags.Sand:
                    return playerSand;
                case PlayerTags.Pink:
                    return playerPink;
                case PlayerTags.Gray:
                    return playerGray;
                case PlayerTags.Orange:
                    return playerOrange;
                case PlayerTags.Purple:
                    return playerPurple;
                case PlayerTags.DarkGreen:
                    return playerDarkGreen;
                case PlayerTags.LightBlue:
                    return playerLightBlue;
                case PlayerTags.White:
                    return playerWhite;
                case PlayerTags.Turquoise:
                    return playerTurquoise;
                case PlayerTags.NavyBlue:
                    return playerNavyBlue;
                case PlayerTags.LightGreen:
                    return playerLightGreen;
                case PlayerTags.Blank:
                    return playerRed;
                default:
                    throw new ArgumentOutOfRangeException(nameof(playerTags), playerTags, null);
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