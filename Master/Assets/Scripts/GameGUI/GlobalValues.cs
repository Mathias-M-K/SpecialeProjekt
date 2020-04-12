using UnityEngine;

namespace GameGUI
{
    public static class GlobalValues
    {
        /*
         * This class will never be destroyed, and is therefore able to pass values between scenes
         * It's static, and can be called on GlobalValues.current
         */

        [Header("Scenes")] 
        public const int NetworkScene = 1;
        public const int WaitingRoomScene = 2;
        public const int GameScene = 4;

        [Header("Welcome Screen")] 
        public static bool StartBtnInteractable = true;

        [Header("Network Values")] 
        public static bool Connected;
        public static string NetworkSceneFlyInDirection;

        [Header("Game Scene")] 
        public static string Role;
        public static int MapIndex;

        [Header("Tags")] 
        public static string ObserverTag = "%Obs";
        public static string HostTag = "Host";

        public static void SetConnected(bool connectedStatus)
        {
            Connected = connectedStatus;
        }
        
        public static void SetStartBtnInteractable(bool newValue)
        {
            StartBtnInteractable = newValue;
        }

        public static void SetRole(string newRole)
        {
            Role = newRole;
        }

        public static void SetMapIndex(int index)
        {
            MapIndex = index;
        }
    }
}