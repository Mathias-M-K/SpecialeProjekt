using System.Collections.Generic;
using Container;
using CoreGame;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AdminGUI
{
    public class ExternalPlayerControl : MonoBehaviour
    {
        [Header("Set Player")] public Player player;
        [Space] public GameObject buttons;

        public Button sendBtn;
        public Button tradeBtn;
        public Button acceptBtn;
        public Button rejectBtn;

        private GameObject _previouslySelected;

        List<Direction> _moves = new List<Direction>();


        // Start is called before the first frame update
        void Start()
        {
            sendBtn.interactable = false;
            rejectBtn.interactable = false;
            acceptBtn.interactable = false;
            tradeBtn.interactable = false;
        
            _moves.Add(Direction.Right);
            _moves.Add(Direction.Left);
            _moves.Add(Direction.Up);
            _moves.Add(Direction.Down);
        }

        private void Update()
        {
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                tradeBtn.interactable = false;
                sendBtn.interactable = false;
            }
        }


        public void OnButtonClick()
        {
            GameObject go = EventSystem.current.currentSelectedGameObject;
            StoredPlayerMove btnAttributes = go.GetComponent<StoredPlayerMove>();

            if (_previouslySelected == null)
            {
                _previouslySelected = go;
            }

            if (go != null)
            {
                if (btnAttributes != null)
                {
                    tradeBtn.interactable = true;
                    sendBtn.interactable = true;
                }
            }

            _previouslySelected = go;
        }
    }
}
    
