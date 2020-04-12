using System;
using System.Collections.Generic;
using AdminGUI;
using CoreGame;
using TMPro;
using UnityEngine;
using Player = Photon.Realtime.Player;

namespace GameGUI.WaitingRoomScene
{
    public class WaitingRoomUIController : MonoBehaviour
    {
        public WaitingRoomController roomController;
        public GameObject playerList;
        public GameObject playerNameElement;
        private readonly Dictionary<string, GameObject> _activePlayers = new Dictionary<string, GameObject>();

        public void AddPlayerToList(Player player)
        {
            string playerNickname = player.NickName;
            
            GameObject go = Instantiate(playerNameElement, playerList.transform, false);
            TextMeshProUGUI text = go.GetComponent<TextMeshProUGUI>();
            
            if (playerNickname.Equals("Host"))
            {
                text.text = playerNickname;
                text.color = new Color32(255, 177, 66,255);
            }
            else if(GlobalMethods.IsObserver(playerNickname))
            {
                text.text = GlobalMethods.CleanNickname(playerNickname);
                text.color = new Color32(255, 121, 63, 255);
            }
            else
            {
                text.text = player.NickName;
                text.color = new Color32(255, 255, 255,255);
            }

            _activePlayers.Add(player.NickName, go);
        }

        public void RemovePlayerFromList(Player player)
        {
            GameObject go = _activePlayers[player.NickName];
            _activePlayers.Remove(player.NickName);
            DestroyImmediate(go, true);
            Destroy(go);
        }
    }
}