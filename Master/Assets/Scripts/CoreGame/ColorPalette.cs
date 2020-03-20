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

        [Space(50)]
        [Header(("Red Button"))]
        public ColorBlock colorBlock;


    }

    public enum Test
    {
        uno,does,thres
    }
}