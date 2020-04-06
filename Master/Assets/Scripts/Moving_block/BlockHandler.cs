using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Container;
using CoreGame;
using UnityEngine;

public class BlockHandler : MonoBehaviour, ITradeObserver, IInventoryObserver
{
    private List<Block> listOfBlock = new List<Block>();
    private int maxAmountOfBlocks = 4;
    public bool scanByColor;
    private PlayerController _playerController;

    private void Start()
    {
        _playerController = GetComponent<PlayerController>();
        _playerController.AddInventoryObserver(this);
    }

    public void AddBlock(string id)
    {
        //Had to trim this fucking string - caused random errors. Fuck unity.
        string input = id.Trim();
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


            PlayerTags p = _playerController.playerTags;
            
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

    public Direction GetDirectionFromId(string id)
    {
        foreach (Block block in listOfBlock)
        {
            if (id.Equals(block.Id)) return block.State;
        }

        return Direction.Blank;
    }

    public int GetIndexFromId(string Id)
    {
        int i = 0;
        foreach (Block block in listOfBlock)
        {
            if (block.Id.Equals(Id))
            {
                return i;
            }

            i++;
        }
        return -1;
    }

    public void OnNewTradeActivity(PlayerTrade playerTrade ,TradeActions tradeAction)
    {
        throw new NotImplementedException();
    }

    public void OnMoveInventoryChange(Direction[] directions)
    {
        print($"{GetComponent<PlayerController>().playerTags} got updated");

        
        
        for (int i = 0; i < listOfBlock.Count; i++)
        {
            listOfBlock[i].State = directions[i];
        }
        
        foreach (Block block in listOfBlock)
        {
            print($"{block.Id} | {block.State}");
        }
    }
}