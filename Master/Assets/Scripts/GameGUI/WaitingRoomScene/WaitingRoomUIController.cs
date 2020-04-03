using System;
using System.Collections.Generic;
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
        private Dictionary<string, GameObject> activePlayers = new Dictionary<string, GameObject>();
        
        public void AddPlayerToList(Player player)
        {
            GameObject go = Instantiate(playerNameElement, playerList.transform, false);
            go.GetComponent<TextMeshProUGUI>().text = player.NickName;
            
            activePlayers.Add(player.NickName,go);
        }

        public void RemovePlayerFromList(Player player)
        {
            GameObject go = activePlayers[player.NickName];
            activePlayers.Remove(player.NickName);
            DestroyImmediate(go,true);
            Destroy(go);
        }
    }
}