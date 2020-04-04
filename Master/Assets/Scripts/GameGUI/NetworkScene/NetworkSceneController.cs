using System;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GameGUI.NetworkScene
{
    public class NetworkSceneController : MonoBehaviourPunCallbacks
    {
        [Header("Assistant Classes")] 
        public CreateRoomPanelUIController CreateRoomUI;
        public JoinRoomPanelUIController JoinRoomUI;
        
        [Header("Content")] 
        public GameObject mainContent;
        public float contentAnimationTime;
        public LeanTweenType contentEaseInType;
        public LeanTweenType contentEaseOutType;

        [Header("Buttons")] 
        public GameObject backBtn;
        public GameObject createRoomBtn;
        public GameObject loadingBtn;
        
        public float buttonAnimationTime;
        public LeanTweenType buttonEaseType;
        

        private void Awake()
        {
            LeanTween.moveLocalX(mainContent, 1243, 0);
            if (Application.internetReachability == NetworkReachability.NotReachable)           
            {
                print("No network detected");
            }
            if (!PhotonNetwork.IsConnected)
            {
                LeanTween.moveLocalY(backBtn, backBtn.transform.localPosition.y + 55+10, 0);
                LeanTween.moveLocalY(createRoomBtn, createRoomBtn.transform.localPosition.y + 55 + 10, 0);
            }
            else
            {
                loadingBtn.SetActive(false);
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
        

        /*
         * Network Stuff
         */
        public override void OnConnectedToMaster()
        {
            if (GlobalValues.connected) return;
            
            GlobalValues.SetConnected(true);
            
            PhotonNetwork.AutomaticallySyncScene = true;
            
            LeanTween.moveLocalY(backBtn, backBtn.transform.localPosition.y - 55 - 10, buttonAnimationTime)
                .setEase(buttonEaseType);

            LeanTween.moveLocalY(createRoomBtn, createRoomBtn.transform.localPosition.y - 55 - 10, buttonAnimationTime)
                .setEase(buttonEaseType);
            
            LeanTween.alpha(loadingBtn.GetComponent<Image>().rectTransform, 0, buttonAnimationTime)
                .setEase(buttonEaseType).destroyOnComplete =true;

            TextMeshProUGUI loadingBtnText = loadingBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

            LeanTween.value(loadingBtnText.gameObject, a =>
            {
                Color32 textColor = loadingBtnText.color;
                textColor.a = (byte) a;
                loadingBtnText.color = textColor;
            }, 255, 0, 0.2f);
        }
        
        public void CreateRoom()
        {
            int.TryParse(CreateRoomUI.GetSizeField(), out int roomSize);
            RoomOptions roomOps = new RoomOptions(){IsVisible = true,IsOpen = true,MaxPlayers = (byte) roomSize};

            PhotonNetwork.LocalPlayer.NickName = "Mr. Host";
            PhotonNetwork.CreateRoom(CreateRoomUI.GetNameField(), roomOps);
        }
        
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            CreateRoomUI.RunRoomFailedAnimation();  
        }

        public void JoinRoom()
        {
            Debug.Log("Joining Room...");

            PhotonNetwork.LocalPlayer.NickName = JoinRoomUI.GetNicknameField();
            PhotonNetwork.JoinRoom(JoinRoomUI.GetNameField());
            
        }
        
        
        public override void OnJoinedRoom()
        {
            SceneManager.LoadScene(GlobalValues.waitingRoomScene);
        }
        
        
    }
}