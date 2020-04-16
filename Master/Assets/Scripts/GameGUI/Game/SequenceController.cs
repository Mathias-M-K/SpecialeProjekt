using System;
using System.Collections;
using System.Collections.Generic;
using AdminGUI.Sequence;
using Container;
using CoreGame;
using CoreGame.Interfaces;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace AdminGUI
{
    public class SequenceController : MonoBehaviour, ISequenceObserver
    {
        private int _nrOfMovesInSequence = 0;

        private int _nrOfRows = 0;
        
        [SerializeField] private Color32 idleColor = new Color32(56, 173, 169,255);
        
        [Header("Prefabs")] 
        public GameObject rowPrefab;
        

        private void Start()
        {
            GameHandler.Current.AddSequenceObserver(this);
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
                    float delay = GameHandler.Current.delayBetweenMoves;
                    StartCoroutine(ClearSequence(delay));
                    break;
            }
        }
        
        private void RemoveMove(StoredPlayerMove move)
        {
            List<StoredPlayerMove> moves = GameHandler.Current.GetSequence();
            int i = 0;
            foreach (StoredPlayerMove playerMove in moves)
            {
                SequenceArrowController arrowController = GetElement(i).GetComponent<SequenceArrowController>();
                Transform element = GetElement(i);
                i++;
                if(arrowController.move.Id == playerMove.Id) continue;

                arrowController.move = playerMove;

                Image img = arrowController.image;
                
                element.GetComponent<Button>().interactable = GUIEvents.current.GetCurrentPlayer() == playerMove.PlayerTags;
                element.GetComponent<SequenceArrowController>().SetCancelButtonActive(GUIEvents.current.GetCurrentPlayer() == playerMove.PlayerTags);
                element.GetComponent<SequenceArrowController>().SetMouseHover(GUIEvents.current.GetCurrentPlayer() == playerMove.PlayerTags);
                
                //Rotation stuff
                LeanTween.color(img.rectTransform, ColorPalette.current.GetPlayerColor(playerMove.PlayerTags), 0.3f).setEase(LeanTweenType.easeOutSine);
            
                
                Vector3 rotation = new Vector3(0,0,GlobalMethods.GetDirectionRotation(playerMove.Direction));
                LeanTween.rotateLocal(img.gameObject, rotation, 0.3f).setEase(LeanTweenType.easeOutSine);
            }

            for (int j = i; j < _nrOfMovesInSequence; j++)
            {
                Image img = GetElement(i).GetComponent<SequenceArrowController>().image;
                GetElement(i).GetComponent<SequenceArrowController>().SetCancelButtonActive(false);
                GetElement(i).GetComponent<Button>().interactable = false;
                
                LeanTween.color(img.rectTransform, idleColor, 0.3f);
            }
            
            _nrOfMovesInSequence--;
        }

        private void AddMove(StoredPlayerMove move)
        {
            //Image img = transform.GetChild(_nrOfMovesInSequence).GetComponent<Image>();
            //img.color = ColorPalette.current.GetPlayerColor(move.PlayerTags);

            Image img = GetElement(_nrOfMovesInSequence).GetComponent<SequenceArrowController>().image;
            img.color = ColorPalette.current.GetPlayerColor(move.PlayerTags);

            GetElement(_nrOfMovesInSequence).GetComponent<SequenceArrowController>().index = _nrOfMovesInSequence;
            GetElement(_nrOfMovesInSequence).GetComponent<SequenceArrowController>().move = move;
            if (GUIEvents.current.GetCurrentPlayer() == move.PlayerTags)
            {
                GetElement(_nrOfMovesInSequence).GetComponent<Button>().interactable = true;
                GetElement(_nrOfMovesInSequence).GetComponent<SequenceArrowController>().SetMouseHover(true);
                GetElement(_nrOfMovesInSequence).GetComponent<SequenceArrowController>().SetCancelButtonActive(true);
            }
            

            
            LeanTween.color(img.rectTransform, ColorPalette.current.GetPlayerColor(move.PlayerTags), 0.3f).setEase(LeanTweenType.easeOutSine);
            
            int rotationZ = GlobalMethods.GetDirectionRotation(move.Direction);
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
            _nrOfMovesInSequence++;
        }
        
        private IEnumerator ClearSequence(float delay)
        {
            for (int i = 0; i < _nrOfMovesInSequence; i++)
            {
                Image img = GetElement(i).GetComponent<Image>();
                
                LeanTween.color(img.rectTransform, idleColor, delay);
                yield return new WaitForSeconds(delay);
            }

            for (int i = 1; i <= _nrOfRows; i++)
            {
                print($"Removing child {i}");
                Transform child = transform.GetChild(1);
                child.SetParent(null);
                Destroy(child.gameObject);
            }
            
            _nrOfMovesInSequence = 0;
        }
        
        private void AddRow()
        {
            Instantiate(rowPrefab, transform, false);
        }

        private Transform GetElement(float i)
        {
            const int elementsInRow = 24;

            int row = Mathf.FloorToInt(i / elementsInRow);
            int element = Mathf.FloorToInt(i % elementsInRow);
            
            //Checking if we need to add more rows
            if (row > _nrOfRows)
            {
                AddRow();
                _nrOfRows++;
            }
            
            Transform tRow = transform.GetChild(row);
            Transform tElement = tRow.GetChild(element);
            
            //print($"row: {row}, Element: {element}");

            return tElement;
        }
    }
}