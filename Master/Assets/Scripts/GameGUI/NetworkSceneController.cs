using System;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GameGUI
{
    public class NetworkSceneController : MonoBehaviourPunCallbacks
    {
        [Header("Content")] 
        public GameObject mainContent;
        public float contentAnimationTime;
        public LeanTweenType contentEaseInType;
        public LeanTweenType contentEaseOutType;

        [Header("Buttons")] 
        public GameObject BackBtn;
        public GameObject CreateRoomBtn;
        public GameObject LoadingBtn;
        
        public float buttonAnimationTime;
        public LeanTweenType buttonEaseType;
        

        private void Awake()
        {
            LeanTween.moveLocalX(mainContent, 1243, 0);
            
            if (!PhotonNetwork.IsConnected)
            {
                LeanTween.moveLocalY(BackBtn, BackBtn.transform.localPosition.y + 55+10, 0);
                LeanTween.moveLocalY(CreateRoomBtn, CreateRoomBtn.transform.localPosition.y + 55 + 10, 0);
            }
            else
            {
                LoadingBtn.SetActive(false);
            }
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

        public override void OnConnectedToMaster()
        {
            LeanTween.moveLocalY(BackBtn, BackBtn.transform.localPosition.y - 55 - 10, buttonAnimationTime)
                .setEase(buttonEaseType);

            LeanTween.moveLocalY(CreateRoomBtn, CreateRoomBtn.transform.localPosition.y - 55 - 10, buttonAnimationTime)
                .setEase(buttonEaseType);
            
            LeanTween.alpha(LoadingBtn.GetComponent<Image>().rectTransform, 0, buttonAnimationTime)
                .setEase(buttonEaseType);

            TextMeshProUGUI loadingBtnText = LoadingBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

            LeanTween.value(loadingBtnText.gameObject, a =>
            {
                Color32 textColor = loadingBtnText.color;
                textColor.a = (byte) a;
                loadingBtnText.color = textColor;
            }, 255, 0, 0.2f);



        }
        
        
        
    }
}