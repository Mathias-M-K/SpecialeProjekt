using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ExternalPlayerControl : MonoBehaviour
{
    [Header("Set Player")] public PlayerColor player;
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
        
        _moves.Add(Direction.RIGHT);
        _moves.Add(Direction.LEFT);
        _moves.Add(Direction.UP);
        _moves.Add(Direction.DOWN);
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
        PlayerMove btnAttributes = go.GetComponent<PlayerMove>();

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
    
