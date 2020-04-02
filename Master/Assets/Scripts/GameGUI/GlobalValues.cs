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
        public const int networkScene = 1;
        public const int waitingRoomScene = 2;
        public const int gameScene = 3;
    }
}