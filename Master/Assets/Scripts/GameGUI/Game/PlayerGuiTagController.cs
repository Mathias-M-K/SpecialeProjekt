using System;
using CoreGame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AdminGUI
{
    public class PlayerGuiTagController : MonoBehaviour
    {
        public TextMeshProUGUI text;
        private void Start()
        {
            GUIEvents.current.onPlayerChange += OnPlayerChange;
        }

        private void OnPlayerChange(PlayerTags playerTag)
        {
            text.text = playerTag.ToString();
            GetComponent<Image>().color = ColorPalette.current.GetPlayerColor(playerTag);
        }

        
    }
}