﻿using System;
using GameGUI;
using Michsky.UI.ModernUIPack;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AdminGUI
{
    public class ExitGameMenu : MonoBehaviourPunCallbacks
    {
        private bool _open;
        public ModalWindowManager exitWindow;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_open)
                {
                    exitWindow.CloseWindow();
                    _open = false;
                }
                else
                {
                    exitWindow.OpenWindow();
                    _open = true;
                }
            }
        }

        public void Disconnect()
        {
            _open = false;
            GlobalValues.SetConnected(false);
            PhotonNetwork.Disconnect();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            SceneManager.LoadScene(GlobalValues.NetworkScene);
        }

        public void Exit()
        {
            Application.Quit();
        }

        public void Cancel()
        {
            _open = false;
        }
    }
}