using System;
using Photon.Pun;
using UnityEngine;

namespace GameGUI
{
    public class NetworkScreenController : MonoBehaviourPunCallbacks
    {
        [Header("Content")] 
        public GameObject mainContent;
        public float contentAnimationTime;
        public LeanTweenType contentEaseType;
        
        private void Start()
        {
            LeanTween.moveLocalX(mainContent, 0, contentAnimationTime).setEase(contentEaseType);
        }
    }
}