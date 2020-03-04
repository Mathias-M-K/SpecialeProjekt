﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CoreGame;
using UnityEngine;

public class BlockHandler : MonoBehaviour, ITradeObserver, IMoveObserver
{
    private List<Block> listOfBlock = new List<Block>();
    private int maxAmountOfBlocks = 4;
    public bool scanByColor;
    private PlayerController _playerController;

    private void Start()
    {
        _playerController = GetComponent<PlayerController>();
        _playerController.AddMoveObserver(this);
    }

    public void AddBlock(string id)
    {
        //Had to trim this fucking string - caused random errors. Fuck unity.
        string input = id.Trim();
        Debug.Log($"ContainsElement {ContainsElement(input)} || listOfBlock {listOfBlock.Count} || maxAmountOfBlocks {maxAmountOfBlocks}");
        if (!ContainsElement(input) && listOfBlock.Count < maxAmountOfBlocks)
        {
            Direction state;
            if (scanByColor)
            {
                state = DetermineMoveByColor(input);
            }
            else
            {
                state = DetermineMove();
            }


            Player p = _playerController.player;
            
            Block _block = new Block(input, state, p);
            listOfBlock.Add(_block);
            Debug.Log($"added block with id {input} and it's state is: {state}");
        }
    }
    

    private bool ContainsElement(string id)
    {
        return listOfBlock.Any(block => id.Equals(block.Id));
    }

    private Direction DetermineMoveByColor(string id)
    {
        switch (id)
        {
            //yellow is up
            case "6978301": return Direction.Up;
            //white is right
            case "6983496": return Direction.Right;
            //red is down
            case "1929590": return Direction.Down;
            //black is left
            case "6995009": return Direction.Left;
            default: return Direction.Blank;
        }
    }

    private Direction DetermineMove()
    {
        switch (listOfBlock.Count)
        {
            //yellow is up
            case 0: return Direction.Up;
            //white is right
            case 1: return Direction.Down;
            //red is down
            case 2: return Direction.Left;
            //black is left
            case 3: return Direction.Right;
            default: return Direction.Blank;
        }
    }

    public void TradeUpdate()
    {
        throw new NotImplementedException();
    }

    public void MoveInventoryUpdate(Direction[] directions)
    {
        for (int i = 0; i < listOfBlock.Count; i++)
        {
            Debug.Log($"Before - ListOfBlock {listOfBlock[i].State}");
            listOfBlock[i].State = directions[i];
            Debug.Log($"After - ListOfBlock {listOfBlock[i].State}");
        }
    }
}