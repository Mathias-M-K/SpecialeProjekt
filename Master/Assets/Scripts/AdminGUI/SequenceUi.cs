using System;
using System.Collections.Generic;
using CoreGame;
using CoreGame.Interfaces;
using UnityEngine;
using UnityEngine.UI;


namespace AdminGUI
{
    public class SequenceUi : MonoBehaviour,ISequenceObserver
    {
        public GameHandler gameHandler;

        private void Start()
        {
            gameHandler.AddSequenceObserver(this);
            UpdateSequence();
        }



        private void UpdateSequence()
        {
            List<GameHandler.PlayerMove> sequence = gameHandler.GetSequence();

            

            foreach (Transform t in transform)
            {
                t.GetComponent<Image>().color = new Color32(255,255,255,0);
                t.GetChild(0).GetComponent<Image>().sprite = gameHandler.GetSprite(Direction.Blank);
            }
            
            int i = 0;
            foreach (GameHandler.PlayerMove  move  in sequence)
            {
                Color32 color;
                switch (move.Player)
                {
                    case Player.Red:
                        color = new Color32(255,66,66,255);
                        break;
                    case Player.Blue:
                        color = new Color32(70,163,253,255);
                        break;
                    case Player.Green:
                        color = new Color32(100,241,181,255);
                        break;
                    case Player.Yellow:
                        color = new Color32(247,255,42,255);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                transform.GetChild(i).GetComponent<Image>().color = color;
                transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = gameHandler.GetSprite(move.Direction);

                i++;
            }
        }

        public void SequenceUpdate()
        {
            UpdateSequence();
        }
    }
}

// child.GetComponent<Image>().color = Color.blue;
// child.GetChild(0).GetComponent<Image>().sprite = testSprite;
