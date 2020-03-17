using System;
using System.Collections;
using System.Collections.Generic;
using Container;
using CoreGame;
using CoreGame.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace AdminGUI
{
    public class SequenceController : MonoBehaviour, ISequenceObserver
    {
        private int nrOfMovesInSequence = 0;
        private Color32 idleColor = new Color32(7, 153, 146,255);

        private void Start()
        {
            GameHandler.current.AddSequenceObserver(this);
            ClearSequence();
        }

        public void SequenceUpdate(SequenceActions sequenceAction, StoredPlayerMove move)
        {
            switch (sequenceAction)
            {
                case SequenceActions.NewMoveAdded:
                    AddMove(move);
                    break;
                case SequenceActions.MoveRemoved:
                    RemoveMove(move);
                    break;
                case SequenceActions.SequencePlayed:
                    ClearSequence();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(sequenceAction), sequenceAction, null);
            }
        }

        

        private void RemoveMove(StoredPlayerMove move)
        {
            nrOfMovesInSequence--;
        }

        private void AddMove(StoredPlayerMove move)
        {
            Image img = transform.GetChild(nrOfMovesInSequence).GetComponent<Image>();
            img.color = GameHandler.current.GetPlayerMaterial(move.Player).color;

            int rotation = GameHandler.current.GetDirectionRotation(move.Direction);
            //img.transform.localRotation = Quaternion.Euler(0,0,rotation);
            LeanTween.color(img.rectTransform, GameHandler.current.GetPlayerMaterial(move.Player).color, 0.3f);

            LeanTween.rotateLocal(img.gameObject, new Vector3(0, 0, rotation), 0.3f);
            nrOfMovesInSequence++;
        }
        

        private void ClearSequence()
        {
            foreach (Transform t in transform)
            {
                t.GetComponent<Image>().color = idleColor;
            }
            nrOfMovesInSequence = 0;
        }
    }
}