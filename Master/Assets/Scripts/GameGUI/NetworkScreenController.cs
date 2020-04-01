using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameGUI
{
    public class NetworkScreenController : MonoBehaviourPunCallbacks
    {
        [Header("Content")] 
        public GameObject mainContent;
        public float contentAnimationTime;
        public LeanTweenType contentEaseInType;
        public LeanTweenType contentEaseOutType;
        

        private void Awake()
        {
            LeanTween.moveLocalX(mainContent, 1243, 0);
        }

        private void Start()
        {
            LeanTween.moveLocalX(mainContent, 0, contentAnimationTime).setEase(contentEaseInType);
        }

        public void Back()
        {
            LeanTween.moveLocalX(mainContent, 1243, contentAnimationTime).setEase(contentEaseOutType).setOnComplete(
                () => SceneManager.LoadScene(0));
        }
    }
}