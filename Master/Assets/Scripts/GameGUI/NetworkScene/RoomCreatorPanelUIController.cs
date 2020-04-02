using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameGUI.NetworkScene
{
    public class RoomCreatorPanelUIController : MonoBehaviour
    {
        public TMP_InputField roomNameField;
        public TMP_InputField roomSizeField;
        public GameObject createRoomBtn;

        public NetworkSceneController NetworkSceneController;

        private void Update()
        {
            if (!roomNameField.text.Equals("") && !roomSizeField.text.Equals(""))
            {
                LeanTween.alpha(createRoomBtn.GetComponent<Image>().rectTransform, 1, 0.4f).setOnComplete(() => createRoomBtn.GetComponent<Button>().interactable = true);
            }
            else
            {
                LeanTween.alpha(createRoomBtn.GetComponent<Image>().rectTransform, 0, 0.4f).setOnComplete(() => createRoomBtn.GetComponent<Button>().interactable = false);
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (roomNameField.isFocused)
                {
                    roomSizeField.Select();
                }
                else
                {
                    roomNameField.Select();
                }
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                NetworkSceneController.CreateRoom();
            }
        }
        
        public string GetNameField()
        {
            return roomNameField.text;
        }
        public string GetSizeField()
        {
            return roomSizeField.text;
        }

        public void RunRoomFailedAnimation()
        {
            LeanTween.rotateAroundLocal(createRoomBtn, new Vector3(0, 0, 1), 5, 0.2f)
                .setEase(LeanTweenType.easeShake)
                .setRepeat(3);
        }

        
    }
}