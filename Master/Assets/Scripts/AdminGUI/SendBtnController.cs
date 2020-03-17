using System;
using Container;
using CoreGame;
using CoreGame.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace AdminGUI
{
    public class SendBtnController : MonoBehaviour, ISequenceObserver
    {
        public bool active;
        public GameObject arrows;

        [Header("Animation Speed")] [Range(0, 5)]
        public float animationSpeed;


        private PlayerController _playerController;


        private void Start()
        {
            GUIEvents.current.onButtonHit += GUIButtonPressed;
            GUIEvents.current.onPlayerChange += PlayerChange;
            GameHandler.current.AddSequenceObserver(this);
        }

        private void PlayerChange(Player p)
        {
            
            //Settings colors for arrows
            foreach (Transform t in arrows.transform.GetChild(0))
            {
                Button b = t.GetComponent<Button>();
                ColorBlock cb = b.colors;
                cb.selectedColor = GameHandler.current.GetPlayerMaterial(p).color;
                cb.highlightedColor = GameHandler.current.GetPlayerMaterial(p).color;

                b.colors = cb;
            }

            _playerController = GameHandler.current.GetPlayerController(p);
            int i = 0;
            foreach (Transform t in arrows.transform.GetChild(0))
            {
                Button b = t.GetComponent<Button>();
                Direction d = _playerController.GetMoves()[i];
                
                int rotation = GameHandler.current.GetDirectionRotation(d);
                if(d != Direction.Blank)b.transform.localRotation = Quaternion.Euler(0, 0, rotation);

                if (d == Direction.Blank)
                {
                    b.interactable = false;
                }
                else
                {
                    b.interactable = true;
                }


                i++;
            }
        }

        protected virtual void GUIButtonPressed(string key)
        {
            if (key.Equals("SendBtn"))
            {
                if (!active)
                {
                    SetActive();
                }
                else
                {
                    SetInactive();
                }
            }
            else if (key.Substring(0, 5).Equals("Arrow"))
            {
                print("ITS ARROW");
                Direction directionToSend = Direction.Blank;
                switch (key.Substring(5, key.Length-5))
                {
                    case "First":
                        directionToSend = _playerController.GetMoves()[0];
                        break;
                    case "Second":
                        directionToSend = _playerController.GetMoves()[1];
                        break;
                    case "Third":
                        directionToSend = _playerController.GetMoves()[2];
                        break;
                    case "Forth":
                        directionToSend = _playerController.GetMoves()[3];
                        break;
                }

                GameHandler.current.AddMoveToSequence(_playerController.player,directionToSend,_playerController.GetIndexForDirection(directionToSend));
                PlayerChange(_playerController.player);
            }
            else
            {
                SetInactive();
            }
        }

        public void SetActive()
        {
            LeanTween.moveLocalY(arrows, 0, animationSpeed).setEase(LeanTweenType.easeOutSine);
            active = true;
        }

        public void SetInactive()
        {
            LeanTween.moveLocalY(arrows, 300, animationSpeed).setEase(LeanTweenType.easeOutSine);
            active = false;
        }

        public void SequenceUpdate(SequenceActions sequenceAction, StoredPlayerMove move)
        {
            if (sequenceAction == SequenceActions.SequencePlayed)
            {
                PlayerChange(_playerController.player);
            }
        }
    }
}