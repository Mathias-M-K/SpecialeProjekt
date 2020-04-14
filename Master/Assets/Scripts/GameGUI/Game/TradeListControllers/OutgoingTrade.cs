using System;
using System.Collections.Generic;
using System.Net.Configuration;
using System.Numerics;
using Container;
using CoreGame;
using GameGUI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

namespace AdminGUI
{
    public class OutgoingTrade : MonoBehaviour, IInventoryObserver, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Panels")] public GameObject selectPlayerPanel;
        public GameObject selectMovePanel;
        public GameObject confirmationScreen;

        [Header("Buttons")] public GameObject confirmButton;
        public GameObject rejectButton;
        public GameObject arrows;
        public GameObject playerColorsPanel;

        [Header("Info")] public TextMeshProUGUI playerTag;
        public TextMeshProUGUI statusText;
        public Image playerTagColor;
        public GameObject directionObj;
        public Image directionImage;

        //TradeInfo
        private PlayerTags _receivingPlayer;
        private Direction _directionOffer;

        [HideInInspector] public PlayerTrade trade;
        private bool _tradeConfirmed;
        private bool _mouseOver;
        private PlayerController _playerController;
        private Dictionary<int, PlayerTags> _playerTagDictionary = new Dictionary<int, PlayerTags>();

        private void Start()
        {
            GUIEvents.current.onPlayerChange += OnPlayerChange;
        }

        private void OnPlayerChange(PlayerTags playerTags)
        {
            _playerController = GameHandler.Current.GetPlayerController(playerTags);
            _playerController.AddInventoryObserver(this);
            GlobalMethods.UpdateArrows(arrows.transform, _playerController);
            UpdatePlayerSelectionPanel();
        }

        private void UpdatePlayerSelectionPanel()
        {
            int i = 0;
            foreach (PlayerController player in GameHandler.Current.GetPlayers())
            {
                if (player == _playerController) continue;

                playerColorsPanel.transform.GetChild(i).GetComponent<Image>().color =
                    ColorPalette.current.GetPlayerColor(player);
                _playerTagDictionary.Add(i, player.playerTag);
                i++;
            }

            for (int j = i; j < 16; j++)
            {
                Destroy(playerColorsPanel.transform.GetChild(j).gameObject);
            }
        }

        /*
         * Panels
         */
        public void SelectPlayerPanelIn()
        {
            LeanTween.moveLocalX(selectPlayerPanel, 0, 0.5f).setEase(LeanTweenType.easeOutQuad);
        }

        public void SelectMovePanelIn()
        {
            LeanTween.moveLocalX(selectMovePanel, 0, 0.5f).setEase(LeanTweenType.easeOutQuad);
        }

        public void ConfirmationScreenIn()
        {
            LeanTween.moveLocalY(confirmationScreen, 0, 0.5f).setEase(LeanTweenType.easeOutQuad);
        }

        /*
         * Buttons
         */
        public void PlayerButtonHit(int nr)
        {
            SelectMovePanelIn();

            playerTag.text = _playerTagDictionary[nr].ToString();
            playerTagColor.color = ColorPalette.current.GetPlayerColor(_playerTagDictionary[nr]);
            _receivingPlayer = _playerTagDictionary[nr];
        }

        public void PlayerDirectionHit(int nr)
        {
            ConfirmationScreenIn();
            Vector3 rotation = new Vector3(0, 0, GlobalMethods.GetDirectionRotation(_playerController.GetMoves()[nr]));
            LeanTween.rotateLocal(directionImage.gameObject, rotation, 0.3f).setEase(LeanTweenType.easeOutSine);
            _directionOffer = _playerController.GetMoves()[nr];
        }

        public void ConfirmInfo()
        {
            statusText.text = $"Waiting on {_receivingPlayer}";
            statusText.color = Color.white;

            LeanTween.moveLocalX(confirmButton, -96, 0.4f).setEase(LeanTweenType.easeInQuad);

            _tradeConfirmed = true;
            _playerController.CreateTrade(_directionOffer, _receivingPlayer);
        }

        public void RejectInfo()
        {
            if (_tradeConfirmed)
            {
                trade.CancelTrade(_playerController.playerTag);
                return;
            }

            LeanTween.moveLocalX(selectPlayerPanel, 168, 0);
            LeanTween.moveLocalX(selectMovePanel, 168, 0);
            LeanTween.moveLocalX(confirmationScreen, 168, 0.5f).setEase(LeanTweenType.easeInQuad).setOnComplete(() =>
            {
                LeanTween.moveLocalY(confirmationScreen, 96.87f, 0);
                LeanTween.moveLocalX(confirmationScreen, 0, 0);
                //LeanTween.moveLocalY(confirmButton, -9.645f, 0);
                //LeanTween.moveLocalX(confirmButton, -58.1f, 0);
                //LeanTween.moveLocalY(rejectButton, -9.645f, 0);
                //LeanTween.moveLocalX(rejectButton, 58.1f, 0);
            });
        }

        public void OnMoveInventoryChange(Direction[] directions)
        {
            GlobalMethods.UpdateArrows(arrows.transform, _playerController);
        }

        public void ManualSetup(PlayerController newPlayerController)
        {
            _playerController = newPlayerController;
            _playerController.AddInventoryObserver(this);
            GlobalMethods.UpdateArrows(arrows.transform, _playerController);
            UpdatePlayerSelectionPanel();
        }

        private void OnDestroy()
        {
            _playerController.RemoveInventoryObserver(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _mouseOver = true;
            if (!_tradeConfirmed) return;
            LeanTween.moveLocalX(rejectButton, 58.2f, 0.4f).setEase(LeanTweenType.easeInOutQuad);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _mouseOver = false;
            if (!_tradeConfirmed) return;
            LeanTween.moveLocalX(rejectButton, 96.31f, 0.4f).setEase(LeanTweenType.easeInOutQuad);
        }
    }
}