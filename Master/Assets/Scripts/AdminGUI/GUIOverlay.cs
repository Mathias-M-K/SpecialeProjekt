using System;
using Michsky.UI.ModernUIPack;
using UnityEngine;

namespace AdminGUI
{
    public class GUIOverlay : MonoBehaviour
    {
        public ModalWindowManager SettingsOverlay;
        private bool OverLayActive;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (OverLayActive)
                {
                    SettingsOverlay.CloseWindow();
                }
                else
                {
                    SettingsOverlay.OpenWindow();
                }
                OverLayActive = !OverLayActive;
            }
        }
    }
}