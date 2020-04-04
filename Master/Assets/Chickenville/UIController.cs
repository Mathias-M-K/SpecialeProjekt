using System;
using TMPro;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace DefaultNamespace
{
    public class UIController : MonoBehaviour
    {
        public static UIController current;

        private void Awake()
        {
            current = this;
        }

        private ChickenController chickenController;

        public GameObject messageInputObject;
        public TMP_InputField messageInput;

        private bool _messageFieldOpen;

        private void Start()
        {
            LeanTween.scale(messageInputObject, new Vector3(0, 0, 0), 0);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (_messageFieldOpen)
                {
                    if (messageInput.text != "")
                    {
                        chickenController.SendMessage(messageInput.text);
                        print("Sending message");
                    }

                    messageInput.text = "";
                    
                    LeanTween.scale(messageInputObject,new Vector3(0,0,0), 0.5f).setEase(LeanTweenType.easeInQuint).setOnComplete(()=> messageInputObject.SetActive(false));
                    _messageFieldOpen = false;
                }else{
                    messageInputObject.SetActive(true);
                    LeanTween.scale(messageInputObject, new Vector3(1, 1, 1), 0.5f).setEase(LeanTweenType.easeOutQuint);
                    _messageFieldOpen = true;
                    messageInput.Select();
                }
                
            }
        }

        public void SetChickenController(ChickenController newChickenController)
        {
            chickenController = newChickenController;
        }
    }
}