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
        //private Color32 idleColor = new Color32(7, 153, 146,255);
        private Color32 idleColor = new Color32(56, 173, 169,255);
        

        private void Start()
        {
            GameHandler.current.AddSequenceObserver(this);
            
            StartCoroutine(ClearSequence(0.02f));
        }

        public void OnSequenceChange(SequenceActions sequenceAction, StoredPlayerMove move)
        {
            switch (sequenceAction)
            {
                case SequenceActions.NewMoveAdded:
                    AddMove(move);
                    break;
                case SequenceActions.MoveRemoved:
                    RemoveMove(move);
                    break;
                case SequenceActions.SequenceStarted:
                    float delay = GameHandler.current.delayBetweenMoves;
                    StartCoroutine(ClearSequence(delay));
                    break;
                case SequenceActions.SequenceEnded:
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
            img.color = ColorPalette.current.GetPlayerColor(move.Player);

            
            LeanTween.color(img.rectTransform, ColorPalette.current.GetPlayerColor(move.Player), 0.3f).setEase(LeanTweenType.easeOutSine);
            
            int rotationZ = GUIMethods.GetDirectionRotation(move.Direction);
            Vector3 rotation = new Vector3(0,0,rotationZ);

            Vector3 reverseRotation;

            if (rotationZ == 90 || rotationZ == -90)
            {
                reverseRotation = new Vector3(0,0,rotationZ*-1);
            }

            if (rotationZ == 180)
            {
                reverseRotation = new Vector3(0,0,0);
            }
            else
            {
                reverseRotation = new Vector3(0,0,180);
            }
      

            img.gameObject.transform.rotation = Quaternion.Euler(reverseRotation);
            
            LeanTween.rotateLocal(img.gameObject, rotation, 0.3f).setEase(LeanTweenType.easeOutSine);
            nrOfMovesInSequence++;
        }

        private IEnumerator ClearSequence(float delay)
        {
            nrOfMovesInSequence = 0;
            foreach (Transform t in transform)
            {
                Image img = t.GetComponent<Image>();

                LeanTween.color(img.rectTransform, idleColor, delay);
                yield return new WaitForSeconds(delay);
            }
            
        }
    }
}