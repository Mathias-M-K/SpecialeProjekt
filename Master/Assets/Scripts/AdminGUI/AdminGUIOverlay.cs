using System;
using Michsky.UI.ModernUIPack;
using UnityEditorInternal.VersionControl;
using UnityEngine;
using UnityEngine.UI;

namespace AdminGUI
{
    public class AdminGUIOverlay : MonoBehaviour
    {
        public ModalWindowManager SettingsOverlay;
        private bool OverLayActive;

        private new bool enabled;

        private void Start()
        {
            AdminGUIEvents.current.onManualOverride += OnManualControl;
        }

        private void OnManualControl()
        {
            enabled = true;
        }
        
        private void Update()
        {
            if (!enabled) return;
            
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

        public void ApplicationQuit()
        {
            print("Application Quit");
            Application.Quit();
        }
    }
}